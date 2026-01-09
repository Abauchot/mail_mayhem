using System;
using Core;
using UnityEngine;
using UnityEngine.UI;
using Letters;

namespace Boxes
{
    public class ServiceBox : MonoBehaviour
    {
        [Header("Accepted Symbol Type")]
        [SerializeField] private SymbolType acceptedSymbolType;

        [Header("Symbol Sprites")]
        [SerializeField] private Sprite squareSprite;
        [SerializeField] private Sprite triangleSprite;
        [SerializeField] private Sprite circleSprite;
        [SerializeField] private Sprite diamondSprite;

        [Header("Visual Components")]
        [SerializeField] private Image symbolImage;
        [SerializeField] private Image backgroundImage;

        [SerializeField] private RectTransform rectTransform;
        private GameModes.GameSessionController _session;

        public RectTransform RectTransform => rectTransform;
        public SymbolType SymbolType => acceptedSymbolType;

        public event Action<SymbolType> OnSymbolChanged;

        private void Awake()
        {
            _session = FindFirstObjectByType<GameModes.GameSessionController>();
        }

        private void Start()
        {
            UpdateVisual();
        }

        private void Reset()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void SetAcceptedSymbol(SymbolType newSymbol)
        {
            if (acceptedSymbolType == newSymbol) return;

            acceptedSymbolType = newSymbol;
            UpdateVisual();
            OnSymbolChanged?.Invoke(newSymbol);
        }

        private void UpdateVisual()
        {
            var targetImage = symbolImage != null ? symbolImage : backgroundImage;
            if (targetImage == null) return;

            Sprite newSprite = acceptedSymbolType switch
            {
                SymbolType.Square => squareSprite,
                SymbolType.Triangle => triangleSprite,
                SymbolType.Circle => circleSprite,
                SymbolType.Diamond => diamondSprite,
                _ => null
            };

            if (newSprite != null)
                targetImage.sprite = newSprite;
        }

        public void ResolveHit(Letters.Letter letter)
        {
            if (_session && !_session.IsRunning)
            {
                return;
            }

            var got = letter.Symbol;
            var expected = acceptedSymbolType;

            bool isCorrect = got == expected;
            letter.ResolveDeliveryResult(this, isCorrect);

            var eventsProvider = GameEventsProvider.Instance;
            if (!eventsProvider)
            {
                Debug.LogWarning("ServiceBox: No GameEventsProvider found in scene, cannot publish DeliveryResult.");
                return;
            }

            if (!GameEventsProvider.Instance)
            {
                return;
            }

            var result = new DeliveryResult(expected, got, isCorrect);
            eventsProvider.Events.Publish(result);
        }
    }
}
