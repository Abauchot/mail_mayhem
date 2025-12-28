using System;
using UnityEngine;

namespace Core
{
    public sealed class GameEventsProvider : MonoBehaviour
    {
        public static GameEventsProvider Instance { get; private set; }
        private IGameEvent _events;
        public IGameEvent Events => _events ??= new GameEvents();

        private void Awake()
        {
            Instance = this;
        }
    }
}