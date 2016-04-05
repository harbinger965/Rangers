namespace Assets.Scripts.Util
{
    /// <summary>
    /// Holds all of the enums used in the game
    /// </summary>
    public static class Enums
    {
        public enum Arrows { Normal, Fireball, Ice, Thunder, Acid, Ricochet, Ghost, ZeroGravity, Tracking, Lifesteal, Virus, Splitting, HeavyKnockback, RapidFire, Grappling, NumTypes }
        public enum Tokens { Fireball, Ice, Thunder, Acid, Ricochet, Ghost, ZeroGravity, Tracking, Lifesteal, Virus, Splitting, HeavyKnockback, RapidFire, Grappling, Health, NumTypes }
		public enum GameType { Deathmatch, Stock, Target, NumTypes }
		public enum GameVariant { None, TokensForEveryone, NumTypes }
        public enum RepetitionTimerSettings { Limited, Unlimited }
        public enum Frequency { None, Sparce, Infrequent, Average, Frequent, Abundant, NumTypes }        
        public enum MenuDirections { Up, Down, Left, Right}
        public enum UIStates { Splash, Main, SinglePlayer, Multiplayer, Settings, Audio, Video, Signin, ArenaStandard, LevelSelect, ValueModifier, TargetLevelSelect, None}
		/// <summary> Different AI modes that can control AI rangers. </summary>
		public enum AIModes { ApproachShoot, RangerBot }

		public enum BattleStages { ProLeagueStandard, NumStages }
		public enum TargetPracticeStages { Intro, MagneticDistortion, NumStages }
    }
}