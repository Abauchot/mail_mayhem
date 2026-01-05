namespace Scoring
{
    public readonly struct ScoreEvent
    {
        public readonly bool isCorrect;
        public readonly int pointsGained;
        public readonly int scoreAfter;
        public readonly int comboAfter;
        public readonly int multiplierAfter;

        public ScoreEvent(
            bool isCorrect,
            int pointsGained,
            int scoreAfter,
            int comboAfter,
            int multiplierAfter)
        {
            this.isCorrect = isCorrect;
            this.pointsGained = pointsGained;
            this.scoreAfter = scoreAfter;
            this.comboAfter = comboAfter;
            this.multiplierAfter = multiplierAfter;
        }
    }
}