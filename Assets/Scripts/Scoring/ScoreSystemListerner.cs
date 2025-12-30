using System;
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
        private bool _subscribed;
        
        public event Action<ScoreSnapshot> OnScoreChanged;

        public int Score => _state?.Score ?? 0;
        public int Combo => _state?.Combo ?? 0;
        public int Multiplier => _state?.Multiplier ?? 1;
        public int MaxCombo => _state?.MaxCombo ?? 0;

        

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
            Notify();
        }

        private void OnDelivery(DeliveryResult result)
        {
            if (!session.IsRunning) return;
            if (_state == null) return;

            if (result.isCorrect)
            {
                _state.RegisterCorrect(session.CurrentMode.basePointPerCorrect);
            }
            else
            {
                _state.RegisterWrong();
            }
            Notify();
        }
        
        private void Notify()
        {
            OnScoreChanged?.Invoke(new ScoreSnapshot(Score, Combo, Multiplier, MaxCombo));
        }
    }
}