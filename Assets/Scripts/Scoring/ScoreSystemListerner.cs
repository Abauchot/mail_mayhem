using UnityEngine;
using Core;
using GameModes;

namespace Scoring
{
    public sealed class ScoreSystemListener : MonoBehaviour
    {
        [SerializeField] private GameEventsProvider eventsProvider;
        [SerializeField] private GameSessionController session;

        private ScoreState _state;

        public int Score => _state?.Score ?? 0;
        public int MaxCombo => _state?.MaxCombo ?? 0;

        private bool _subscribed;

        private void Start()
        {
            if (eventsProvider == null || session == null)
            {
                Debug.LogError($"{name}: missing eventsProvider or session");
                return;
            }

            eventsProvider.Events.OnLetterDelivered += OnDelivery;
            _subscribed = true;
        }

        private void OnDisable()
        {
            if (!_subscribed) return;
            eventsProvider.Events.OnLetterDelivered -= OnDelivery;
            _subscribed = false;
        }

        public void ResetForRun(GameModeDefinition mode)
        {
            _state = new ScoreState(mode.comboConfig);
            _state.Reset();
        }

        private void OnDelivery(DeliveryResult result)
        {
            if (!session.IsRunning) return;
            if (_state == null) return;

            if (result.isCorrect)
                _state.RegisterCorrect(session.CurrentMode.basePointPerCorrect);
            else
                _state.RegisterWrong();
        }
    }
}