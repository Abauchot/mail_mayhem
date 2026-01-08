using Letters;
using Stats;
using UnityEngine;

namespace GameModes.Run
{
    public sealed class RunCleanup : MonoBehaviour
    {
        [SerializeField] private GameSessionController session;
        [SerializeField] private LetterSpawner spawner;
        [SerializeField] private Behaviour[] disableOnEnd; 
        [SerializeField] private UI.GameOverOverlay overlay;

        private void OnEnable()
        {
            if (session == null) return;
            session.OnRunEnded += OnRunEnded;
        }

        private void OnDisable()
        {
            if (session == null) return;
            session.OnRunEnded -= OnRunEnded;
        }

        private void OnRunEnded(RunResult result)
        {
            if (spawner) spawner.SetEnabled(false);

            if (disableOnEnd != null)
                foreach (var b in disableOnEnd)
                    if (b) b.enabled = false;

            if (overlay)
            { }
        }
    }
}