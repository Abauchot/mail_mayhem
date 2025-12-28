using UnityEngine;

namespace Scoring
{
    
        [CreateAssetMenu(fileName = "ComboConfig", menuName = "MailMayhem/Scoring/Combo Config")]
        public sealed class ComboConfig : ScriptableObject
        {
                [Min(1)] public int streakPerMultiplierStep = 5;
                [Min(1)] public int maxMultiplier = 10;
                
                public int ComputeMultiplier(int combo)
                {
                        int multiplier = 1 + (combo / streakPerMultiplierStep);
                        return Mathf.Clamp(multiplier,1, maxMultiplier);
                }
        }
}
