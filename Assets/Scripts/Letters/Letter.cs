using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Letters.Throw;
using Letters.Animations;

namespace Letters
{
    public enum SymbolType { Square, Triangle, Circle, Diamond }
    public enum LetterState { Idle, Dragging, Throwing, Feedback }

    public class Letter : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Symbol")]
        [SerializeField] private SymbolType symbolType;
        public SymbolType Symbol => symbolType;

        [Header("Sprites")]
        [SerializeField] public Sprite squareSprite;
        [SerializeField] public Sprite triangleSprite;
        [SerializeField] public Sprite circleSprite;
        [SerializeField] public Sprite diamondSprite;
        [SerializeField] public Sprite neutralSprite;

        [Header("Components")]
        [SerializeField] public Image envelopeImage;
        [SerializeField] public CanvasGroup canvasGroup;

        [HideInInspector] public LetterSpawner spawner;

        [Header("Throw (tuning)")]
        [SerializeField] private float minThrowSpeed = 900f;
        [SerializeField] private float maxThrowSpeed = 3000f;
        [SerializeField] private float friction = 6.5f;
        [SerializeField] private float maxThrowDuration = 1.0f;
        [SerializeField] private float returnDuration = 0.15f;
        [SerializeField] private float throwScale = 1.5f;
        [SerializeField, Range(0f, 1f)] private float flickWeight = 0.75f;

        [Header("Throw (sampling)")]
        [SerializeField] private int maxSamples = 6;

        [Header("Feedback")]
        [SerializeField] private float incorrectShakeDuration = 0.14f;
        [SerializeField] private float incorrectShakeAmplitude = 14f;
        [SerializeField] private float incorrectShakeFrequency = 28f;
        
        [Header("PC Send animation(arc)")]
        [SerializeField] private float pcSendDuration = 0.16f;

        private bool _topArcFlip;

        private LetterState _state = LetterState.Idle;
        private RectTransform _rectTransform;
        private Canvas _parentCanvas;
        private RectTransform _parentRect;
        private Vector2 _spawnAnchoredPos;
        private Coroutine _activeRoutine;

        private Boxes.BoxesRegistry _boxesRegistry;

        private bool _pointerInputEnabled = true;
        private Vector2 _lastLocalPointer;
        private bool _isGrabbing;
        private bool _attemptResolved;

        // Services
        private UiThrowSampler _sampler;
        private LetterHitDetector _hitDetector;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _parentCanvas = GetComponentInParent<Canvas>();
            _parentRect = GetComponentInParent<RectTransform>();

            if (!envelopeImage) envelopeImage = GetComponent<Image>();
            if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();

            _sampler = new UiThrowSampler(maxSamples);
        }

        public void Setup(SymbolType type, LetterSpawner ownerSpawner, Boxes.BoxesRegistry boxesRegistry)
        {
            symbolType = type;
            spawner = ownerSpawner;
            _boxesRegistry = boxesRegistry;
            _hitDetector = new LetterHitDetector(_boxesRegistry);

            envelopeImage.sprite = type switch
            {
                SymbolType.Square => squareSprite,
                SymbolType.Triangle => triangleSprite,
                SymbolType.Circle => circleSprite,
                SymbolType.Diamond => diamondSprite,
                _ => neutralSprite
            };

            _spawnAnchoredPos = _rectTransform.anchoredPosition;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;

            _state = LetterState.Idle;
            _attemptResolved = false;

            _sampler.SetMaxSamples(maxSamples);
            _sampler.Reset();
            
        }

        public void SetPointerInputEnabled(bool isenabled)
        {
            _pointerInputEnabled = isenabled;
            if (canvasGroup)
                canvasGroup.blocksRaycasts = isenabled;
        }

        public void Grab(Vector2 screenPos)
        {
            if (_state is LetterState.Throwing or LetterState.Feedback) return;

            _spawnAnchoredPos = _rectTransform.anchoredPosition;
            _rectTransform.SetAsLastSibling();

            CancelActiveRoutine();

            _sampler.Reset();
            _sampler.BeginGrab(screenPos, Time.unscaledTime);

            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.8f;

            _isGrabbing = true;
            _lastLocalPointer = ScreenToLocalInParent(screenPos);

            _state = LetterState.Dragging;
        }

        public void Move(Vector2 screenPos)
        {
            if (_state != LetterState.Dragging || !_isGrabbing) return;

            _sampler.AddSample(screenPos, Time.unscaledTime);

            var local = ScreenToLocalInParent(screenPos);
            var delta = local - _lastLocalPointer;
            _lastLocalPointer = local;

            _rectTransform.anchoredPosition += delta;
        }

        public void Release(Vector2 screenPos)
        {
            if (_state != LetterState.Dragging || !_isGrabbing) return;

            _isGrabbing = false;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            _sampler.AddSample(screenPos, Time.unscaledTime);

            Vector2 velocity = _sampler.ComputeVelocity(_parentRect, _parentCanvas, flickWeight, throwScale);
            float speed = velocity.magnitude;

            if (speed < minThrowSpeed)
            {
                StartReturnToSpawn();
                return;
            }

            if (speed > maxThrowSpeed)
                velocity = velocity.normalized * maxThrowSpeed;

            _activeRoutine = StartCoroutine(ThrowAndDetect(velocity));
            _state = LetterState.Throwing;
        }

        private Vector2 ScreenToLocalInParent(Vector2 screenPos)
        {
            if (_parentRect == null) return Vector2.zero;
            Camera cam = null;
            if (_parentCanvas != null && _parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                cam = _parentCanvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, screenPos, cam, out var local);
            return local;
        }

        // EventSystem
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_pointerInputEnabled) return;
            _spawnAnchoredPos = _rectTransform.anchoredPosition;
            _rectTransform.SetAsLastSibling();
        }
        public void OnBeginDrag(PointerEventData eventData) { if (_pointerInputEnabled) Grab(eventData.position); }
        public void OnDrag(PointerEventData eventData) { if (_pointerInputEnabled) Move(eventData.position); }
        public void OnEndDrag(PointerEventData eventData) { if (_pointerInputEnabled) Release(eventData.position); }

        private void StartReturnToSpawn()
        {
            CancelActiveRoutine();
            _activeRoutine = StartCoroutine(ReturnToSpawnRoutine());
        }

        private IEnumerator ReturnToSpawnRoutine()
        {
            var start = _rectTransform.anchoredPosition;
            yield return LetterAnimations.ReturnTo(_rectTransform, start, _spawnAnchoredPos, returnDuration);

            _activeRoutine = null;
            if (_state != LetterState.Feedback)
                _state = LetterState.Idle;
        }

        private void CancelActiveRoutine()
        {
            if (_activeRoutine != null)
            {
                StopCoroutine(_activeRoutine);
                _activeRoutine = null;
            }
        }

        private IEnumerator ThrowAndDetect(Vector2 velocity)
        {
            _attemptResolved = false;
            _state = LetterState.Throwing;
            canvasGroup.blocksRaycasts = false;

            float t = 0f;
            while (t < maxThrowDuration)
            {
                float dt = Time.unscaledDeltaTime;
                t += dt;

                _rectTransform.anchoredPosition += velocity * dt;

                float damp = Mathf.Exp(-friction * dt);
                velocity *= damp;

                var hitBox = _hitDetector?.FindHit(_rectTransform);
                if (hitBox)
                {
                    canvasGroup.blocksRaycasts = true;
                    hitBox.ResolveHit(this);
                    yield break;
                }

                if (velocity.magnitude < 120f)
                    break;

                yield return null;
            }
            canvasGroup.blocksRaycasts = true;
            StartReturnToSpawn();
        }

        // PC send
        public void SendToBox(Boxes.ServiceBox box, float duration = 0.15f)
        {
            if (box == null) return;

            _state = LetterState.Throwing;
            _attemptResolved = false;

            CancelActiveRoutine();
            canvasGroup.blocksRaycasts = false;
            float d = duration > 0.0001f ? duration : pcSendDuration;
            _activeRoutine = StartCoroutine(SendToBoxRoutine(box, d));
        }

        private IEnumerator SendToBoxRoutine(Boxes.ServiceBox box, float duration)
        {
            RectTransform letterParent = _rectTransform.parent as RectTransform;

            Vector2 start = _rectTransform.anchoredPosition;
            Vector2 end = LetterAnimations.GetLocalPointIn(letterParent, box.RectTransform, _parentCanvas);

            yield return LetterAnimations.ReturnTo(_rectTransform, start, end, duration);

            canvasGroup.blocksRaycasts = true;
            box.ResolveHit(this);
        }


        
        
        // Called by ServiceBox
        public void ResolveDeliveryResult(Boxes.ServiceBox box, bool isCorrect)
        {
            if (_attemptResolved) return;
            _attemptResolved = true;

            if (isCorrect)
            {
                DestroyLetter();
            }
            else
            {
                CancelActiveRoutine();
                _activeRoutine = StartCoroutine(IncorrectFeedbackThenReturn());
            }
        }

        private IEnumerator IncorrectFeedbackThenReturn()
        {
            _state = LetterState.Feedback;
            canvasGroup.blocksRaycasts = false;

            yield return LetterAnimations.Shake(_rectTransform, incorrectShakeDuration, incorrectShakeAmplitude, incorrectShakeFrequency);
            canvasGroup.blocksRaycasts = true;
            
            yield return ReturnToSpawnRoutine();
            
            _attemptResolved = false;
            _activeRoutine = null;
            _state = LetterState.Idle;
        }

        public void DestroyLetter()
        {
            if (_activeRoutine != null)
            {
                StopCoroutine(_activeRoutine);
                _activeRoutine = null;
            }

            if (spawner)
                spawner.OnLetterDestroyed(this);

            Destroy(gameObject);
        }
    }
}
