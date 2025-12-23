using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Letters
{
    public enum SymbolType
    {
        Square,
        Triangle,
        Circle,
        Diamond
    }

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
        [SerializeField] private float minThrowSpeed = 900f;     // UI units/sec
        [SerializeField] private float maxThrowSpeed = 3000f;    // clamp
        [SerializeField] private float friction = 6.5f;          // damping
        [SerializeField] private float maxThrowDuration = 1.0f;  // safety
        [SerializeField] private float returnDuration = 0.15f;   // return to spawn
        
        private RectTransform _rectTransform;
        private Canvas _parentCanvas;

        private Vector2 _spawnAnchoredPos;
        private Coroutine _throwRoutine;
        
        private Boxes.BoxesRegistry _boxesRegistry;

        private readonly List<Sample> _samples = new();
        
        private bool _pointerInputEnabled = true;

        private RectTransform _parentRect;
        private Vector2 _lastLocalPointer;
        private bool _isGrabbing;


        private struct Sample
        {
            public Vector2 screenPos;
            public float time;
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _parentCanvas = GetComponentInParent<Canvas>();
            _parentRect = GetComponentInParent<RectTransform>();

            if (envelopeImage == null)
                envelopeImage = GetComponent<Image>();

            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Setup(SymbolType type, LetterSpawner ownerSpawner, Boxes.BoxesRegistry boxesRegistry)
        {
            symbolType = type;
            spawner = ownerSpawner;
            _boxesRegistry = boxesRegistry;

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
        }
        
        public void SetPointerInputEnabled(bool enabled)
        {
            _pointerInputEnabled = enabled;

            // Sur PC, évite que la lettre capte le pointer
            if (canvasGroup != null)
                canvasGroup.blocksRaycasts = enabled;
        }

        public void Grab(Vector2 screenPos)
        {
            _spawnAnchoredPos = _rectTransform.anchoredPosition;
            _rectTransform.SetAsLastSibling();

            CancelThrowIfAny();

            _samples.Clear();
           

            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.8f;

            _isGrabbing = true;

            _lastLocalPointer = ScreenToLocalInParent(screenPos);
        }

        public void Move(Vector2 screenPos)
        {
            if (!_isGrabbing) return;

            var local = ScreenToLocalInParent(screenPos);
            var delta = local - _lastLocalPointer;
            _lastLocalPointer = local;

            _rectTransform.anchoredPosition += delta;
            
        }

        public void Release(Vector2 screenPos)
        {
            if (!_isGrabbing) return;
            _isGrabbing = false;
            
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            Vector2 velocity = ComputeThrowVelocityUI();
            float speed = velocity.magnitude;

            if (speed < minThrowSpeed)
            {
                StartReturnToSpawn();
                return;
            }

            if (speed > maxThrowSpeed)
                velocity = velocity.normalized * maxThrowSpeed;

            _throwRoutine = StartCoroutine(ThrowAndDetect(velocity));
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
        

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_pointerInputEnabled) return;
            _spawnAnchoredPos = _rectTransform.anchoredPosition;
            _rectTransform.SetAsLastSibling();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_pointerInputEnabled) return;
            Grab(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_pointerInputEnabled) return;
            Move(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_pointerInputEnabled) return;
            Release(eventData.position);
        }


        private void StartReturnToSpawn()
        {
            if (_throwRoutine != null) StopCoroutine(_throwRoutine);
            _throwRoutine = StartCoroutine(ReturnToSpawn());
        }

        private void CancelThrowIfAny()
        {
            if (_throwRoutine != null)
            {
                StopCoroutine(_throwRoutine);
                _throwRoutine = null;
            }
        }

        private Vector2 ComputeThrowVelocityUI()
        {
            if (_samples.Count < 2) return Vector2.zero;

            Sample first = _samples[0];
            Sample last = _samples[^1];

            float dt = last.time - first.time;
            if (dt <= 0.0001f) return Vector2.zero;

            RectTransform parent = _rectTransform.parent as RectTransform;
            if (parent == null) return Vector2.zero;
            
            Camera cam = null;
            if (_parentCanvas != null && _parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                cam = _parentCanvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, first.screenPos, cam, out var firstLocal);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, last.screenPos, cam, out var lastLocal);

            Vector2 delta = lastLocal - firstLocal; 
            return delta / dt; 
        }

        private IEnumerator ThrowAndDetect(Vector2 velocity)
        {
            canvasGroup.blocksRaycasts = false;

            float t = 0f;

            while (t < maxThrowDuration)
            {
                float dt = Time.unscaledDeltaTime;
                t += dt;

                _rectTransform.anchoredPosition += velocity * dt;
                
                float damp = Mathf.Exp(-friction * dt);
                velocity *= damp;
                
                var hitBox = FindHitBox();
                if (hitBox != null)
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
            _throwRoutine = StartCoroutine(ReturnToSpawn());
        }

        private Boxes.ServiceBox FindHitBox()
        {
            if (!_boxesRegistry) return null;

            Rect letterRect = GetWorldRect(_rectTransform);

            foreach (var box in _boxesRegistry.Boxes)
            {
                if (!box) continue;

                Rect boxRect = GetWorldRect(box.RectTransform);
                if (letterRect.Overlaps(boxRect, true))
                    return box;
            }

            return null;
        }

        private static Rect GetWorldRect(RectTransform rt)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            Vector2 min = corners[0];
            Vector2 max = corners[2];
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        private IEnumerator ReturnToSpawn()
        {
            Vector2 start = _rectTransform.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < returnDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float a = Mathf.Clamp01(elapsed / returnDuration);
                a = 1f - Mathf.Pow(1f - a, 3f); // ease-out

                _rectTransform.anchoredPosition = Vector2.LerpUnclamped(start, _spawnAnchoredPos, a);
                yield return null;
            }

            _rectTransform.anchoredPosition = _spawnAnchoredPos;
            _throwRoutine = null;
        }

        public void DestroyLetter()
        {
            if (_throwRoutine != null)
            {
                StopCoroutine(_throwRoutine);
                _throwRoutine = null;
            }

            if (spawner != null)
                spawner.OnLetterDestroyed(this);

            Destroy(gameObject);
        }
        
        public void SendToBox(Boxes.ServiceBox box, float duration = 0.15f)
        {
            if (box == null) return;

            CancelThrowIfAny();
            canvasGroup.blocksRaycasts = false;

            StartCoroutine(SendToBoxRoutine(box, duration));
        }

        private IEnumerator SendToBoxRoutine(Boxes.ServiceBox box, float duration)
        {
            Vector2 start = _rectTransform.anchoredPosition;
            Vector2 end = box.RectTransform.anchoredPosition;

            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float a = Mathf.Clamp01(t / Mathf.Max(0.0001f, duration));
                a = 1f - Mathf.Pow(1f - a, 3f);
                _rectTransform.anchoredPosition = Vector2.LerpUnclamped(start, end, a);
                yield return null;
            }

            _rectTransform.anchoredPosition = end;
            canvasGroup.blocksRaycasts = true;

            box.ResolveHit(this);
        }

    }
}
