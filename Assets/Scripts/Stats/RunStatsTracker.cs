using UnityEngine;

namespace Stats
{
    public sealed class RunStatsTracker : MonoBehaviour
    {
        public int CorrectCount { get; private set; }
        public int WrongCount { get; private set; }

        public void ResetStats()
        {
            CorrectCount = 0;
            WrongCount = 0;
        }
        
        public void RegisterCorrect() => CorrectCount++;
        
        public void RegisterWrong() => WrongCount++;
    }
}