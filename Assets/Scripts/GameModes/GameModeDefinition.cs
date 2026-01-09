using UnityEngine;
using Scoring;
using Difficulty;

namespace GameModes
{
    [CreateAssetMenu(fileName = "Mode_", menuName = "MailMayhem/Game Modes/Game Mode Definition")]
    public sealed class GameModeDefinition : ScriptableObject
    {
        [Header("Identity")]
        public GameMode mode;
        public string displayName = "Mode";

        [Header("Scoring")]
        [Min(1)] public int basePointPerCorrect = 100;
        public ComboConfig comboConfig;

        [Header("Time Attack")]
        public float timeAttackDurationSeconds = 60f;

        [Header("Survival")]
        public int maxMistakes = 3;

        [Header("Difficulty")]
        [Tooltip("Symbol permutation settings. If null, permutations are disabled.")]
        public SymbolPermutationConfig permutationConfig;

        [Header("Debug Overrides (Editor/Dev)")]
        public DebugOverrides debug;

        [System.Serializable]
        public struct DebugOverrides
        {
            public bool enabled;

            [Tooltip("Force every spawned letter to this symbol.")]
            public bool forceSymbol;
            public Letters.SymbolType forcedSymbol;

            [Tooltip("Freeze timer (TimeAttack)")]
            public bool freezeTime;

            [Tooltip("Ignore mistakes (Survival)")]
            public bool infiniteMistakes;

            [Tooltip("Optional deterministic RNG seed for spawns.")]
            public bool useSeed;
            public int seed;
        }
    }
}