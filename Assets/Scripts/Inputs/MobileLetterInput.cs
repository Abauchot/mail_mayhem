using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inputs
{
    public class MobileLetterInput : MonoBehaviour
    {
        private MailMayhem_Inputs _inputs;
        private bool _bound;

        public event Action<Vector2> OnGrab;
        public event Action<Vector2> OnMove;
        public event Action<Vector2> OnRelease;

        private Vector2 _lastPos;

        private void OnEnable()
        {
            if (_inputs == null)
                _inputs = new MailMayhem_Inputs();

            _inputs.Gameplay_Mobile.Enable();

            if (_bound) return;
            _bound = true;

            _inputs.Gameplay_Mobile.Point.performed += OnPoint;
            _inputs.Gameplay_Mobile.Press.started += OnPressStarted;
            _inputs.Gameplay_Mobile.Press.canceled += OnPressCanceled;
        }

        private void OnDisable()
        {
            if (_inputs == null) return;

            if (_bound)
            {
                _inputs.Gameplay_Mobile.Point.performed -= OnPoint;
                _inputs.Gameplay_Mobile.Press.started -= OnPressStarted;
                _inputs.Gameplay_Mobile.Press.canceled -= OnPressCanceled;
                _bound = false;
            }

            _inputs.Gameplay_Mobile.Disable();
        }

        private void OnDestroy()
        {
            _inputs?.Dispose();
            _inputs = null;
        }

        private void OnPoint(InputAction.CallbackContext ctx)
        {
            _lastPos = ctx.ReadValue<Vector2>();
            
            if (_inputs != null && _inputs.Gameplay_Mobile.Press.IsPressed())
                OnMove?.Invoke(_lastPos);
        }

        private void OnPressStarted(InputAction.CallbackContext ctx)
        {
            if (_inputs == null) return;
            _lastPos = _inputs.Gameplay_Mobile.Point.ReadValue<Vector2>();
            OnGrab?.Invoke(_lastPos);
        }

        private void OnPressCanceled(InputAction.CallbackContext ctx)
        {
            if (_inputs == null) return;
            _lastPos = _inputs.Gameplay_Mobile.Point.ReadValue<Vector2>();
            OnRelease?.Invoke(_lastPos);
        }
    }
}
