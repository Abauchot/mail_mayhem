using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Inputs
{


    public class MailInputBehaviour : MonoBehaviour
    {
        private MailMayhem_Inputs _inputs;

        public event Action OnClick;
        public event Action OnDrag;
        public event Action OnRelease;

        private void Awake()
        {
            _inputs = new MailMayhem_Inputs();
        }

        private void OnEnable()
        {
            _inputs.UI.Click.performed += HandleClick;
            _inputs.UI.Click.performed += HandleDrag;
            _inputs.UI.Click.performed += HandleRelease;
            _inputs.UI.Enable();
        }
        
        private void OnDisable()
        {
            _inputs.UI.Click.performed -= HandleClick;
            _inputs.UI.Click.performed -= HandleDrag;
            _inputs.UI.Click.performed -= HandleRelease;
            _inputs.UI.Disable();
        }
        
        private void HandleClick(InputAction.CallbackContext context)
        {
            OnClick?.Invoke();
        }
        
        private void HandleDrag(InputAction.CallbackContext context)
        {
            OnDrag?.Invoke();
        }
        
        private void HandleRelease(InputAction.CallbackContext context)
        {
            OnRelease?.Invoke();
        }
    }
}
