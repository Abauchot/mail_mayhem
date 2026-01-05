namespace Scoring
{
    public sealed class ScoreState
    {
        public int Score { get; private set; }
        public int Combo { get; private set; }
        public int MaxCombo { get; private set; }
        public int Multiplier { get; private set; }
        
        private readonly ComboConfig _comboConfig;
        
        public ScoreState(ComboConfig comboConfig)
        {
            _comboConfig = comboConfig;
        }

        public void Reset()
        {
            Score = 0;
            Combo = 0;
            MaxCombo = 0;
            Multiplier = 1;
        }

        public int RegisterCorrect(int basePoints)
        {
            Combo++;
            MaxCombo = System.Math.Max(MaxCombo, Combo);
            
            Multiplier = _comboConfig.ComputeMultiplier(Combo);
            
            int gained = basePoints * Multiplier;
            Score += gained;

            return gained;
        }

        public void RegisterWrong()
        {
            Combo = 0;
            Multiplier = 1;
        }
    }
}