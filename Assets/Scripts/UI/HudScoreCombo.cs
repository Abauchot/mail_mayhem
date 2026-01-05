using UnityEngine;
using TMPro;
using Scoring;

namespace UI
{
    public sealed class HudScoreCombo : MonoBehaviour
    {
        [SerializeField] private ScoreSystemListener scoreSystemListener;

        [Header("Texts")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text comboText;
        [SerializeField] private TMP_Text multiplierText;

        private void OnEnable()
        {
            if (scoreSystemListener == null)
            {
                return;
            }

            if (scoreText == null)
            {
                return;
            }

            scoreSystemListener.OnScoreChanged += OnScoreChanged;

            OnScoreChanged(new ScoreSnapshot(
                scoreSystemListener.Score,
                scoreSystemListener.Combo,
                scoreSystemListener.Multiplier,
                scoreSystemListener.MaxCombo
            ));
        }

        private void OnDisable()
        {
            if (scoreSystemListener == null) return;

            scoreSystemListener.OnScoreChanged -= OnScoreChanged;
        }

        private void OnScoreChanged(ScoreSnapshot snapshot)
        {
            if (scoreText) scoreText.text = snapshot.score.ToString();
            if (comboText) comboText.text = snapshot.combo > 0 ? snapshot.combo.ToString() : "";
            if (multiplierText) multiplierText.text = snapshot.multiplier > 1 ? $"x{snapshot.multiplier}" : "";
        }
    }
}
