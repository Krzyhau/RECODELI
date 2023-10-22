using System;

namespace RecoDeli.Scripts.Leaderboards
{
    public struct LeaderboardScore
    {
        public int Place;
        public float Value;
    }
    public struct LeaderboardRecord
    {
        public string DisplayName;
        public LeaderboardScore CompletionTime;
        public LeaderboardScore InstructionsCount;
        public DateTime ScoreRecordedAt;
    }
} 
