using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inputs
{
    public class MobileLetterInput : MonoBehaviour
    {
        private MailMayhem_Inputs _inputs;

        public event Action<Vector2> OnGrab;
        public event Action<Vector2> OnMove;
        public event Action<Vector2> OnRelease;

        private Vector2 _lastPos;

        private void Awake()
        {
            _inputs = new MailMayhem_Inputs();
        }

        private void OnEnable()
        {
            _inputs.Gameplay_Mobile.Enable();

            _inputs.Gameplay_Mobile.Point.performed += OnPoint;
            _inputs.Gameplay_Mobile.Press.started += OnPressStarted;
            _inputs.Gameplay_Mobile.Press.canceled += OnPressCanceled;
        }

        private void OnDisable()
        {
            _inputs.Gameplay_Mobile.Point.performed -= OnPoint;
            _inputs.Gameplay_Mobile.Press.started -= OnPressStarted;
            _inputs.Gameplay_Mobile.Press.canceled -= OnPressCanceled;

            _inputs.Gameplay_Mobile.Disable();
        }

        private void OnPoint(InputAction.CallbackContext ctx)
        {
            _lastPos = ctx.ReadValue<Vector2>();

            if (_inputs.Gameplay_Mobile.Press.IsPressed())
                OnMove?.Invoke(_lastPos);
        }

        private void OnPressStarted(InputAction.CallbackContext ctx)
        {
            OnGrab?.Invoke(_lastPos);
        }

        private void OnPressCanceled(InputAction.CallbackContext ctx)
        {
            OnRelease?.Invoke(_lastPos);
        }
    }
}