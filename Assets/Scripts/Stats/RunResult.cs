using UnityEngine;
using GameModes;

namespace Stats
{
    [SerializeField]
    public sealed class RunResult
    {
        // mode info
        public GameMode mode;
        public string modeName;
        
        //score info
        public int score;
        public int maxCombo;

        //correct/wrong counts
        public int correctCount;
        public int wrongCount;
        
        //time info
        public float durationSeconds;
        public string endedAtIso;
    }
}