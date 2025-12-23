using UnityEngine;
using Letters;
using Boxes;

namespace Inputs
{
    public class LetterInputRouter : MonoBehaviour
    {
        [SerializeField] private BoxesRegistry boxesRegistry;

        [Header("Inputs")]
        [SerializeField] private PcLetterInput pcInput;
        [SerializeField] private MobileLetterInput mobileInput;

        private Letter _currentLetter;
        
        
        private void Awake()
        {
            if (boxesRegistry == null)
                boxesRegistry = FindFirstObjectByType<BoxesRegistry>();

            if (pcInput == null)
                pcInput = GetComponent<PcLetterInput>() ?? FindFirstObjectByType<PcLetterInput>();

            if (mobileInput == null)
                mobileInput = GetComponent<MobileLetterInput>() ?? FindFirstObjectByType<MobileLetterInput>();
            
            if (pcInput == null) pcInput = FindFirstObjectByType<PcLetterInput>();
            if (mobileInput == null) mobileInput = FindFirstObjectByType<MobileLetterInput>();
            if (boxesRegistry == null) boxesRegistry = FindFirstObjectByType<BoxesRegistry>();
        }

        private void OnEnable()
        {
#if UNITY_ANDROID || UNITY_IOS
    if (mobileInput == null)
    {
        Debug.LogError("LetterInputRouter: MobileLetterInput not found in scene.");
        return;
    }

    mobileInput.OnGrab += HandleGrab;
    mobileInput.OnMove += HandleMove;
    mobileInput.OnRelease += HandleRelease;
#else
            if (pcInput == null)
            {
                Debug.LogError("LetterInputRouter: PcLetterInput not found in scene.");
                return;
            }

            pcInput.OnSend += HandleSendToBox;
#endif
        }


        private void OnDisable()
        {
#if UNITY_ANDROID || UNITY_IOS
    if (mobileInput == null) return;
    mobileInput.OnGrab -= HandleGrab;
    mobileInput.OnMove -= HandleMove;
    mobileInput.OnRelease -= HandleRelease;
#else
            if (pcInput == null) return;
            pcInput.OnSend -= HandleSendToBox;
#endif
        }


        public void SetCurrentLetter(Letter letter)
        {
            _currentLetter = letter;
        }

        private void HandleGrab(Vector2 screenPos)
        {
            if (_currentLetter == null) return;
            _currentLetter.Grab(screenPos);
        }

        private void HandleMove(Vector2 screenPos)
        {
            if (_currentLetter == null) return;
            _currentLetter.Move(screenPos);
        }

        private void HandleRelease(Vector2 screenPos)
        {
            if (_currentLetter == null) return;
            _currentLetter.Release(screenPos);
        }

        private void HandleSendToBox(SymbolType type)
        {
            if (_currentLetter == null) return;

            var box = boxesRegistry.GetBox(type);
            if (box == null) return;

            _currentLetter.SendToBox(box);
        }
    }
}