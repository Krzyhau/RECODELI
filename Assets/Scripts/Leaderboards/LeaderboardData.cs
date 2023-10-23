using System.Collections.Generic;

namespace RecoDeli.Scripts.Leaderboards
{
    // this data should be prepared by whatever service provides it.
    // sending the entire leaderboard doesn't seem preferable lol
    public struct LeaderboardStats
    {
        public int RecordCount;
        public float BestRecord;
        public float WorstRecord;
        public List<float> Averages;
    }

    public struct LeaderboardRecord
    {
        public int Place;
        public string DisplayName;
        public float Score;
    }

    public struct LeaderboardData
    {
        public LeaderboardStats TimeStats;
        public LeaderboardStats InstructionsStats;
        public List<LeaderboardRecord> TimeRecords;
        public List<LeaderboardRecord> InstructionRecords;
    }
}
