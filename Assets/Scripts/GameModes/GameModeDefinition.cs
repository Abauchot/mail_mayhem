using Scoring;
using UnityEngine;

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
        [Tooltip("Only used in Time Attack mode")]
        public float timeAttackDurationSeconds = 60f;
        
        [Header("Survival")]
        [Tooltip("Only used in Survival mode")]
        public int maxMistakes = 3;

        public void Validate()
        {
            if (comboConfig == null)
            {
                Debug.LogWarning($"Game Mode Definition '{name}' has no Combo Config assigned. Assigning default.");
                //TODO : Assign a default ComboConfig from a known location or create one programmatically
            }
        }
    }
}
