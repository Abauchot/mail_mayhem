using UnityEngine;
using Letters;
using Boxes;

namespace Inputs
{
    public class LetterInputRouter : MonoBehaviour
    {
        [SerializeField] private BoxesRegistry boxesRegistry;
        public BoxesRegistry BoxesRegistry => boxesRegistry;

        [Header("Inputs")]
        [SerializeField] private PcLetterInput pcInput;

        private Letter _currentLetter;

        private void Awake()
        {
            if (!boxesRegistry) boxesRegistry = FindFirstObjectByType<BoxesRegistry>();
            if (!pcInput) pcInput = GetComponent<PcLetterInput>() ?? FindFirstObjectByType<PcLetterInput>();
        }

        private void OnEnable()
        {
            if (!boxesRegistry) boxesRegistry = FindFirstObjectByType<BoxesRegistry>();
            if (!pcInput) pcInput = GetComponent<PcLetterInput>() ?? FindFirstObjectByType<PcLetterInput>();

            if (!isActiveAndEnabled || !pcInput) return;
            pcInput.enabled = true;
            pcInput.OnSend += HandleSendToBox;
        }

        private void OnDisable()
        {
            if (pcInput) pcInput.OnSend -= HandleSendToBox;
        }

        public void SetCurrentLetter(Letter letter)
        {
            _currentLetter = letter;
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
