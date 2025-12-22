using UnityEngine;
using Letters;
using Boxes;

namespace Inputs
{
    public class LetterInputRouter : MonoBehaviour
    {
        [SerializeField] private Letter currentLetter;
        [SerializeField] private BoxesRegistry boxesRegistry;

        [Header("Inputs")]
        [SerializeField] private PcLetterInput pcInput;
        [SerializeField] private MobileLetterInput mobileInput;

        private void OnEnable()
        {
#if UNITY_ANDROID || UNITY_IOS
            mobileInput.OnGrab += currentLetter.Grab;
            mobileInput.OnMove += currentLetter.Move;
            mobileInput.OnRelease += currentLetter.Release;
#else
            pcInput.OnSend += HandleSendToBox;
#endif
        }

        private void OnDisable()
        {
#if UNITY_ANDROID || UNITY_IOS
            mobileInput.OnGrab -= currentLetter.Grab;
            mobileInput.OnMove -= currentLetter.Move;
            mobileInput.OnRelease -= currentLetter.Release;
#else
            pcInput.OnSend -= HandleSendToBox;
#endif
        }

        private void HandleSendToBox(SymbolType type)
        {
            var box = boxesRegistry.GetBox(type);
            if (box == null) return;

            // TODO: direction to box
            box.ResolveHit(currentLetter);

            // TODO: animate letter to box
            // currentLetter.AnimateTo(box);
        }
    }
}