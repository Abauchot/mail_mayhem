using UnityEngine;
using UnityEngine.InputSystem;
using GameModes;

namespace DebugTools
{
    public sealed class DebugStartRunKeyboard : MonoBehaviour
    {
        [SerializeField] private GameSessionController sessionController;
        [SerializeField] private GameModeDefinition modeToStart;

        [Header("Debug Key")]
        [SerializeField] private Key key = Key.F1;
        [SerializeField] private Key mistake = Key.F2;

        private void Update()
        {
            if (Keyboard.current == null) return;

            if (Keyboard.current[key].wasPressedThisFrame)
            {
                if (sessionController.IsRunning) return;
                sessionController.StartRun(modeToStart);
            }

            if (Keyboard.current[mistake].wasPressedThisFrame)
            {
                sessionController.RegisterMistake();
            }
        }
    }
}