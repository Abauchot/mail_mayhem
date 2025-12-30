using UnityEngine;

namespace Scoring
{
    public readonly struct ScoreSnapshot
    {
        public readonly int score;
        public readonly int combo;
        public readonly int multiplier;
        public readonly int maxCombo;
        
        
        public ScoreSnapshot(int score, int combo, int multiplier, int maxCombo)
        {
            this.score = score;
            this.combo = combo;
            this.multiplier = multiplier;
            this.maxCombo = maxCombo;
        }
    }
}