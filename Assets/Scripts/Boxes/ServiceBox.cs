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
        [SerializeField] public SymbolType acceptedSymbolType;
        
        [Header("Feedback")]
        [SerializeField] public Image backgroundImage;
        
        [SerializeField] private RectTransform rectTransform;
        private GameModes.GameSessionController _session;
        
        public RectTransform RectTransform => rectTransform;
        public SymbolType SymbolType => acceptedSymbolType;

        private void Awake()
        {
            _session = FindFirstObjectByType<GameModes.GameSessionController>();
        }


        private void Reset()
        {
            rectTransform = GetComponent<RectTransform>();
        }


        /// <summary>
        /// Called when a letter hits the box collider
        /// </summary>
        /// <param name="letter"></param>
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

            if (!Core.GameEventsProvider.Instance)
            {
                return;
            }
            
            var result = new DeliveryResult(expected, got, isCorrect);
            eventsProvider.Events.Publish(result);

        }
    }
}