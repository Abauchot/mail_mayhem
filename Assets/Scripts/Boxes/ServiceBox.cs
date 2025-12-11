using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Letters;

namespace Boxes
{
    public class ServiceBox : MonoBehaviour,IDropHandler
    {
        
        [Header("Accepted Symbol Type")]
        [SerializeField] public SymbolType acceptedSymbolType;
        
        [Header("Feedback")]
        [SerializeField] public Image backgroundImage;

        public void OnDrop(PointerEventData eventData)
        {
            GameObject go = eventData.pointerDrag;
            if (go == null)
            {
                return;
            }
            
            Letter letter = go.GetComponent<Letter>();
            if (letter == null)
            {
                return;
            }
            
            bool correct = letter.Symbol == acceptedSymbolType;
            if (correct)
            {
                Debug.Log("ServiceBox: Correct letter delivered!");
               //TODO: GameManager.Instance.OnLetterResolved(true);
                
            }
            else
            {
                Debug.Log("ServiceBox: Incorrect letter delivered.");
                //TODO: GameManager.Instance.OnLetterResolved(false);
            }
            letter.DestroyLetter();
        }
    }
}