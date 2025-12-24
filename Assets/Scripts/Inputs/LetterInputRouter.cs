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
        private bool _useMobile;

        private void Awake()
        {
            if (!boxesRegistry) boxesRegistry = FindFirstObjectByType<BoxesRegistry>();
            if (!pcInput) pcInput = GetComponent<PcLetterInput>() ?? FindFirstObjectByType<PcLetterInput>();
            if (!mobileInput) mobileInput = GetComponent<MobileLetterInput>() ?? FindFirstObjectByType<MobileLetterInput>();
        }

        private void OnEnable()
        {
            ApplyBindings();
        }

        private void OnDisable()
        {
            UnbindAll();
        }

        public void SetCurrentLetter(Letter letter)
        {
              _currentLetter = letter;
              if (_currentLetter)
              {
                  _currentLetter.SetPointerInputEnabled(!_useMobile);
              }
        }

        public void UseMobile(bool useMobile)
        {
            _useMobile = useMobile;
            if(_currentLetter != null)
            {
                _currentLetter.SetPointerInputEnabled(!_useMobile);
            }
            ApplyBindings();
        }

        private void ApplyBindings()
        {
            if (!isActiveAndEnabled) return;

            UnbindAll();

            if (_useMobile)
            {
                if (pcInput) pcInput.enabled = false;
                
                if (!mobileInput) return;
                mobileInput.enabled = true;

                mobileInput.OnGrab += HandleGrab;
                mobileInput.OnMove += HandleMove;
                mobileInput.OnRelease += HandleRelease;
            }
            else
            {
                if (mobileInput) mobileInput.enabled = false;
                
                if (!pcInput) return;
                pcInput.enabled = true;

                pcInput.OnSend += HandleSendToBox;
            }
        }

        private void UnbindAll()
        {
            if (mobileInput)
            {
                mobileInput.OnGrab -= HandleGrab;
                mobileInput.OnMove -= HandleMove;
                mobileInput.OnRelease -= HandleRelease;
            }

            if (pcInput)
            {
                pcInput.OnSend -= HandleSendToBox;
            }
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
            if (_currentLetter == null || !boxesRegistry) return;

            var box = boxesRegistry.GetBox(type);
            if (box == null) return;

            _currentLetter.SendToBox(box);
        }
    }
}
