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

        [Header("Animation")]
        [SerializeField] private float returnDuration = 0.15f;

        [Header("Feedback")]
        [SerializeField] private float incorrectShakeDuration = 0.14f;
        [SerializeField] private float incorrectShakeAmplitude = 14f;
        [SerializeField] private float incorrectShakeFrequency = 28f;
        
        [Header("PC Send animation(arc)")]
        [SerializeField] private float pcSendDuration = 0.16f;

        private bool _topArcFlip;

        private LetterState _state = LetterState.Idle;
        public LetterState State => _state;
        private RectTransform _rectTransform;
        private Canvas _parentCanvas;
        private RectTransform _parentRect;
        private Vector2 _spawnAnchoredPos;
        private Coroutine _activeRoutine;

        private Boxes.BoxesRegistry _boxesRegistry;

        private bool _attemptResolved;

        // Services
        private LetterHitDetector _hitDetector;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _parentCanvas = GetComponentInParent<Canvas>();
            _parentRect = GetComponentInParent<RectTransform>();

            if (!envelopeImage) envelopeImage = GetComponent<Image>();
            if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
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
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _spawnAnchoredPos = _rectTransform.anchoredPosition;
            _rectTransform.SetAsLastSibling();
        }
        public void OnBeginDrag(PointerEventData eventData) { }
        public void OnDrag(PointerEventData eventData) { }
        public void OnEndDrag(PointerEventData eventData) { }

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
        
        public void SendToBox(Boxes.ServiceBox box, float duration = 0.15f)
        {
            if (box == null) return;
            if (_state != LetterState.Idle) return;

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
