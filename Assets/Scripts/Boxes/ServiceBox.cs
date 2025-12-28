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
        
        public RectTransform RectTransform => rectTransform;
        public SymbolType SymbolType => acceptedSymbolType;


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
            var got = letter.Symbol;
            var expected = acceptedSymbolType;
            
            bool isCorrect = got == expected;
            Debug.Log($"HIT ON BOX_{expected} expected:{expected} got:{got} correct:{isCorrect}");
            
            letter.ResolveDeliveryResult(this, isCorrect);
            
            var eventsProvider = GameEventsProvider.Instance;
            if (!eventsProvider)
            {
                Debug.LogWarning("ServiceBox: No GameEventsProvider found in scene, cannot publish DeliveryResult.");
                return;
            }

            var result = new DeliveryResult(expected, got, isCorrect);
            eventsProvider.Events.Publish(result);

        }
    }
}