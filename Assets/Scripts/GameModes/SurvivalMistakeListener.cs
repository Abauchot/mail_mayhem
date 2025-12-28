using UnityEngine;
using Core;

namespace GameModes
{
    public sealed class SurvivalMistakeListener : MonoBehaviour
    {
        [SerializeField] private GameEventsProvider eventsProvider;
        [SerializeField] private GameSessionController session;
        
        private bool _subscribed;
        
        private void Start()
        {
            if (eventsProvider == null)
            {
                Debug.LogError($"{name}: eventsProvider is NULL");
                return;
            }
            if (session == null)
            {
                Debug.LogError($"{name}: session is NULL");
                return;
            }

            eventsProvider.Events.OnLetterDelivered += OnDelivery;
            _subscribed = true;
        }

        private void OnEnable()
        {
            if (!_subscribed)
            {
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
            if (!session.IsRunning) return;
            if (result.isCorrect) return;
            session.RegisterMistake();
        }
    }
}