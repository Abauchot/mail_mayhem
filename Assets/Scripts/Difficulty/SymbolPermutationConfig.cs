using UnityEngine;
using Letters;

namespace Difficulty
{
    [CreateAssetMenu(fileName = "PermutationConfig", menuName = "MailMayhem/Difficulty/Symbol Permutation Config")]
    public class SymbolPermutationConfig : ScriptableObject
    {
        [System.Serializable]
        public struct Threshold
        {
            [Tooltip("Minimum score to activate this threshold")]
            public int scoreRequired;

            [Tooltip("Chance (0-1) to permute after each delivery at this threshold")]
            [Range(0f, 1f)] public float chancePerDelivery;
        }

        [Header("Permutation Thresholds")]
        [Tooltip("Define score thresholds in ascending order. Higher scores = higher permutation chance.")]
        [SerializeField] private Threshold[] thresholds = new[]
        {
            new Threshold { scoreRequired = 0,    chancePerDelivery = 0f },
            new Threshold { scoreRequired = 500,  chancePerDelivery = 0.1f },
            new Threshold { scoreRequired = 1500, chancePerDelivery = 0.2f },
            new Threshold { scoreRequired = 3000, chancePerDelivery = 0.35f },
            new Threshold { scoreRequired = 5000, chancePerDelivery = 0.5f },
        };

        [Header("Debug Overrides")]
        [SerializeField] private DebugOverrides debug;
        public DebugOverrides Debug => debug;

        [System.Serializable]
        public struct DebugOverrides
        {
            [Tooltip("Enable debug overrides")]
            public bool enabled;

            [Tooltip("Force permutation after every delivery")]
            public bool forcePermutationEveryDelivery;

            [Tooltip("Override chance regardless of score")]
            public bool overrideChance;
            [Range(0f, 1f)] public float forcedChance;

            [Tooltip("Force specific swap (ignores random selection)")]
            public bool forceSpecificSwap;
            public SymbolType swapSymbolA;
            public SymbolType swapSymbolB;

            [Tooltip("Disable all permutations")]
            public bool disablePermutations;

            [Tooltip("Log permutation events to console")]
            public bool logPermutations;
        }

        public float GetPermutationChance(int currentScore)
        {
            if (debug.enabled)
            {
                if (debug.disablePermutations) return 0f;
                if (debug.forcePermutationEveryDelivery) return 1f;
                if (debug.overrideChance) return debug.forcedChance;
            }

            float chance = 0f;
            foreach (var t in thresholds)
            {
                if (currentScore >= t.scoreRequired)
                    chance = t.chancePerDelivery;
                else
                    break;
            }
            return chance;
        }

        public bool TryGetForcedSwap(out SymbolType a, out SymbolType b)
        {
            if (debug.enabled && debug.forceSpecificSwap)
            {
                a = debug.swapSymbolA;
                b = debug.swapSymbolB;
                return true;
            }
            a = default;
            b = default;
            return false;
        }

        public bool ShouldLog => debug.enabled && debug.logPermutations;
    }
}
