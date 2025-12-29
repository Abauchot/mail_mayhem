using System;
using Letters;
using UnityEngine;

namespace Inputs
{
    public class PcLetterInput : MonoBehaviour
    {
        private MailMayhem_Inputs _inputs;
        private bool _bound;

        public event Action<SymbolType> OnSend;
        
        private void OnLeft(UnityEngine.InputSystem.InputAction.CallbackContext _)  => OnSend?.Invoke(SymbolType.Square);
        private void OnUp(UnityEngine.InputSystem.InputAction.CallbackContext _)    => OnSend?.Invoke(SymbolType.Triangle);
        private void OnRight(UnityEngine.InputSystem.InputAction.CallbackContext _) => OnSend?.Invoke(SymbolType.Circle);
        private void OnDown(UnityEngine.InputSystem.InputAction.CallbackContext _)  => OnSend?.Invoke(SymbolType.Diamond);


        private void EnsureInputs()
        {
            _inputs ??= new MailMayhem_Inputs();
        }
        
        private void OnEnable()
        {
            EnsureInputs();
            _inputs.Gameplay_PC.Enable();

            if (_bound) return;
            _bound = true;

            _inputs.Gameplay_PC.SendLeft.performed  += OnLeft;
            _inputs.Gameplay_PC.SendUp.performed    += OnUp;
            _inputs.Gameplay_PC.SendRight.performed += OnRight;
            _inputs.Gameplay_PC.SendDown.performed  += OnDown;
        }

        private void OnDisable()
        {
            if (_inputs == null) return;

            if (_bound)
            {
                _inputs.Gameplay_PC.SendLeft.performed  -= OnLeft;
                _inputs.Gameplay_PC.SendUp.performed    -= OnUp;
                _inputs.Gameplay_PC.SendRight.performed -= OnRight;
                _inputs.Gameplay_PC.SendDown.performed  -= OnDown;
                _bound = false;
            }

            _inputs.Gameplay_PC.Disable();
            
        }
        
        private void OnDestroy()
        {
            _inputs?.Dispose();
            _inputs = null;
        }
    }
}