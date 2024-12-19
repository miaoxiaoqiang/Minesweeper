namespace Minesweeper.Model
{
    public struct Streak
    {
        public Streak(GameLevel level, int winningstreak, int losingstreak)
        {
            Level = level;
            WinningStreak = winningstreak;
            LosingStreak = losingstreak;
        }

        public GameLevel Level
        {
            get;
            private set;
        }

        public int WinningStreak
        {
            get;
            set;
        }

        public int LosingStreak
        {
            get;
            set;
        }
    }
}
