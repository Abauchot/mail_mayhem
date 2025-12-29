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
            session.OnRunEnded += Handheld;
        }
        private void OnDisable()
        {
            session.OnRunEnded -= Handheld;
        }
        
        private void Handheld(RunResult result)
        {
            Debug.Log($"[RUN RESULT] " +
                      $"mode={result.modeName} " +
                      $"score={result.score}" +
                      $" maxCombo={result.maxCombo} " +
                      $"duration={result.durationSeconds:0.00}s " +
                      $"correct={result.correctCount} " +
                      $"wrong={result.wrongCount}"
                      );
        }
    }
}