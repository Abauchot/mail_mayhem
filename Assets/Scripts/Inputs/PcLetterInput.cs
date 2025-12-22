using System;
using Letters;
using UnityEngine;

namespace Inputs
{
    public class PcLetterInput : MonoBehaviour
    {
        private MailMayhem_Inputs _inputs;

        public event Action<SymbolType> OnSend;

        private void Awake()
        {
            _inputs = new MailMayhem_Inputs();
        }

        private void OnEnable()
        {
            _inputs.Gameplay_PC.Enable();

            _inputs.Gameplay_PC.SendLeft.performed  += _ => OnSend?.Invoke(SymbolType.Square);
            _inputs.Gameplay_PC.SendUp.performed    += _ => OnSend?.Invoke(SymbolType.Triangle);
            _inputs.Gameplay_PC.SendRight.performed += _ => OnSend?.Invoke(SymbolType.Circle);
            _inputs.Gameplay_PC.SendDown.performed  += _ => OnSend?.Invoke(SymbolType.Diamond);
        }

        private void OnDisable()
        {
            _inputs.Gameplay_PC.Disable();
        }
    }
}