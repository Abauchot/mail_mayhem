using System;
using UnityEngine;
using Boxes;
using Core;
using Letters;
using Scoring;
using Random = UnityEngine.Random;

namespace Difficulty
{
    public class SymbolPermutationController : MonoBehaviour
    {
        [SerializeField] private SymbolPermutationConfig config;
        [SerializeField] private BoxesRegistry boxesRegistry;
        [SerializeField] private ScoreSystemListener scoreListener;

        private bool _subscribed;

        public event Action<SymbolType, SymbolType> OnSymbolsSwapped;

        private void Start()
        {
            if (boxesRegistry == null)
                boxesRegistry = FindFirstObjectByType<BoxesRegistry>();

            if (scoreListener == null)
                scoreListener = FindFirstObjectByType<ScoreSystemListener>();

            Subscribe();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (_subscribed) return;

            var events = GameEventsProvider.Instance?.Events;
            if (events != null)
            {
                events.OnLetterDelivered += HandleDelivery;
                _subscribed = true;
            }
        }

        private void Unsubscribe()
        {
            if (!_subscribed) return;

            var events = GameEventsProvider.Instance?.Events;
            if (events != null)
            {
                events.OnLetterDelivered -= HandleDelivery;
                _subscribed = false;
            }
        }

        private void HandleDelivery(DeliveryResult result)
        {
            if (config == null || boxesRegistry == null) return;

            TryPermutation();
        }

        private void TryPermutation()
        {
            int currentScore = scoreListener != null ? scoreListener.Score : 0;
            float chance = config.GetPermutationChance(currentScore);

            if (chance <= 0f) return;

            if (Random.value <= chance)
            {
                ExecuteSwap();
            }
        }

        private void ExecuteSwap()
        {
            SymbolType a, b;

            if (config.TryGetForcedSwap(out var forcedA, out var forcedB))
            {
                a = forcedA;
                b = forcedB;
            }
            else
            {
                PickRandomPair(out a, out b);
            }

            if (a == b) return;

            boxesRegistry.SwapBoxSymbols(a, b);

            if (config.ShouldLog)
            {
                Debug.Log($"[Permutation] Swapped {a} <-> {b} at score {scoreListener?.Score ?? 0}");
            }

            OnSymbolsSwapped?.Invoke(a, b);
        }

        private void PickRandomPair(out SymbolType a, out SymbolType b)
        {
            var symbols = (SymbolType[])Enum.GetValues(typeof(SymbolType));
            int indexA = Random.Range(0, symbols.Length);
            int indexB;
            do
            {
                indexB = Random.Range(0, symbols.Length);
            } while (indexB == indexA);

            a = symbols[indexA];
            b = symbols[indexB];
        }

        public void SetConfig(SymbolPermutationConfig newConfig)
        {
            config = newConfig;
        }

        public void ResetMapping()
        {
            boxesRegistry?.ResetMapping();
        }
    }
}
