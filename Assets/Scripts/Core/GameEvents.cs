using System;
using Core;

namespace Core
{
    public interface IGameEvent
    {
        event Action<DeliveryResult> OnLetterDelivered;
        void Publish(DeliveryResult result);
    }
    
    public sealed class GameEvents : IGameEvent
    {
        public event Action<DeliveryResult> OnLetterDelivered;
        public void Publish(DeliveryResult result) => OnLetterDelivered?.Invoke(result);
    }
}