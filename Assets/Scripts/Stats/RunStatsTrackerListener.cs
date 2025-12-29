using UnityEngine;
using Core;
using GameModes;

namespace Stats
{
    public sealed class RunStatsTrackerListener : MonoBehaviour
    {
        [SerializeField] private GameEventsProvider eventsProvider;
        [SerializeField] private RunStatsTracker stats;
        [SerializeField] private GameSessionController session;

        private bool _subscribed;

        private void Start()
        {
            if (eventsProvider == null || stats == null  || session == null )
            {
                Debug.LogError($"{name}: eventsProvider is NULL");
                return;
            }
            if (stats == null)
            {
                Debug.LogError($"{name}: stats is NULL");
                return;
            }

            eventsProvider.Events.OnLetterDelivered += OnDelivery;
            _subscribed = true;
        }

        private void OnDisable()
        {
            if (!_subscribed)
            {
                return;
            }
            eventsProvider.Events.OnLetterDelivered -= OnDelivery;
            _subscribed = false;
        }

        private void OnDelivery(DeliveryResult result)
        {
            if (result.isCorrect) stats.RegisterCorrect();
            else stats.RegisterWrong();
        }
    }
}