using UnityEngine;
using TMPro;
using Scoring;
using System.Collections;

namespace UI
{
    public sealed class HudScoreFeedback : MonoBehaviour
    {
        [SerializeField] private ScoreSystemListener scoreSystem;

        [Header("References")]
        [SerializeField] private RectTransform hudRoot;
        [SerializeField] private TMP_Text popText;

        [Header("Tuning")]
        [SerializeField] private float shakeAmount = 10f;
        [SerializeField] private float shakeDuration = 0.12f;

        private Vector3 _baseScale;
        private Vector3 _basePos;
        private Coroutine _feedbackRoutine;

        private void Awake()
        {
            if (hudRoot == null)
                hudRoot = (RectTransform)transform;

            _baseScale = hudRoot.localScale;
            _basePos = hudRoot.anchoredPosition;

            if (popText)
                popText.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (scoreSystem == null) return;
            scoreSystem.OnScoreEvent += OnScoreEvent;
        }

        private void OnDisable()
        {
            if (scoreSystem == null) return;
            scoreSystem.OnScoreEvent -= OnScoreEvent;
        }

        private void OnScoreEvent(ScoreEvent evt)
        {
            if (_feedbackRoutine != null)
                StopCoroutine(_feedbackRoutine);

            _feedbackRoutine = StartCoroutine(
                evt.isCorrect
                    ? Correct(evt.pointsGained)
                    : WrongFeedback()
            );
        }

        private IEnumerator Correct(int points)
        {
            if (!popText) yield break;

            popText.text = $"+{points}";
            popText.color = Color.white;
            popText.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.3f);

            popText.gameObject.SetActive(false);
        }



        private IEnumerator WrongFeedback()
        {
            float t = 0f;
            
            while (t < shakeDuration)
            {
                Vector2 shake = Random.insideUnitCircle * shakeAmount;
                hudRoot.anchoredPosition = _basePos + new Vector3(shake.x, shake.y, 0f);
                t += Time.deltaTime;
                yield return null;
            }

            hudRoot.anchoredPosition = _basePos;
        }
    }
}
