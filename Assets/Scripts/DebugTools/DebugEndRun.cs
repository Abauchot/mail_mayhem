using UnityEngine;
using GameModes;
using Stats;

namespace DebugTools
{
    
    public class DebugRunEnd : MonoBehaviour
    {
        [SerializeField] private GameSessionController session;

        private void OnEnable()
        {
            session.onRunEnded += Handheld;
        }
        private void OnDisable()
        {
            session.onRunEnded -= Handheld;
        }
        
        private void Handheld(RunResult result)
        {
            Debug.Log($"[RUN RESULT] mode={result.modeName} duration={result.durationSeconds:0.00}s " +
                      $"correct={result.correctCount} wrong={result.wrongCount}");
        }
    }
}