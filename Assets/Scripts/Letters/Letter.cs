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
    
    public class Letter : MonoBehaviour, IPointerDownHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        [Header("Symbol")] 
        [SerializeField] private SymbolType symbolType;
        [SerializeField] public SymbolType Symbol => symbolType;
        
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
        
        private RectTransform _rectTransform;
        private Vector2 _startPosition;
        private Canvas _parentCanvas;
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _parentCanvas = GetComponentInParent<Canvas>();
            if (envelopeImage == null)
            {
                envelopeImage = GetComponent<Image>();
            }
        }

        public void Setup(SymbolType type, LetterSpawner ownerSpawner)
        {
            symbolType = type;
            spawner = ownerSpawner;

            envelopeImage.sprite = type switch
            {
                SymbolType.Square => squareSprite,
                SymbolType.Triangle => triangleSprite,
                SymbolType.Circle => circleSprite,
                SymbolType.Diamond => diamondSprite,
                _ => neutralSprite
            };
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _startPosition = _rectTransform.anchoredPosition;
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.8f;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
            _rectTransform.anchoredPosition = _startPosition;
        }

        
        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _parentCanvas.scaleFactor;
        }
        
        public void DestroyLetter()
        {
            if (spawner != null)
            {
                spawner.OnLetterDestroyed(this);
            }
            Destroy(gameObject);
        }
    }
}
