using System;
using UnityEngine;
using UnityEngine.EventSystems;
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
            Debug.Log($"HIT ON BOX_{expected} expected:{expected} got:{got}");

            if (got == expected)
            {
                Debug.Log("ServiceBox: Correct letter delivered!");
            } else
            {
                Debug.Log("ServiceBox: Incorrect letter delivered.");
            }
            letter.DestroyLetter();

        }

        // public void OnDrop(PointerEventData eventData)
        // {
        //     GameObject go = eventData.pointerDrag;
        //     if (go == null)
        //     {
        //         return;
        //     }
        //     
        //     Letter letter = go.GetComponent<Letter>();
        //     if (letter == null)
        //     {
        //         return;
        //     }
        //     
        //
        //     
        //     bool correct = letter.Symbol == acceptedSymbolType;
        //     if (correct)
        //     {
        //         Debug.Log("ServiceBox: Correct letter delivered!");
        //        //TODO: GameManager.Instance.OnLetterResolved(true);
        //         
        //     }
        //     else
        //     {
        //         Debug.Log("ServiceBox: Incorrect letter delivered.");
        //         //TODO: GameManager.Instance.OnLetterResolved(false);
        //     }
        //     letter.DestroyLetter();
        // }
    }
}