using UnityEngine;
using TMPro;
using GameModes;
using Stats;

namespace UI
{
    public sealed class GameOverOverlay : MonoBehaviour
    {
        [SerializeField] private GameSessionController session;
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text hintText;

        private void Awake()
        {
            if (root == null)
                root = gameObject;

            root.SetActive(false);
        }

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
            if (titleText) titleText.text = "GAME OVER";

            if (hintText)
            {
                hintText.text = "Click Restart";
            }

            root.SetActive(true);
        }

        // === BUTTON CALLBACK ===
        public void RestartRun()
        {
            if (session == null)
            {
                Debug.LogError($"{name}: session is NULL");
                return;
            }

            root.SetActive(false);
            session.RestartRun();
        }

        public void Hide()
        {
            root.SetActive(false);
        }
    }
}