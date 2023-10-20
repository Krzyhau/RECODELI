using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecoDeli.Scripts.Leaderboards
{
    public class TestLeaderboardProvider : LeaderboardProvider
    {
        private List<LeaderboardRecord> templateData = new()
        {
            new(){ CompletionTime = 1.1f, DisplayName = "This", InstructionsCount = 1, ScoreTime = new DateTime() },
            new(){ CompletionTime = 5.52f, DisplayName = "leaderboard", InstructionsCount = 2, ScoreTime = new DateTime() },
            new(){ CompletionTime = 10.3f, DisplayName = "data", InstructionsCount = 3, ScoreTime = new DateTime() },
            new(){ CompletionTime = 20.7f, DisplayName = "is", InstructionsCount = 4, ScoreTime = new DateTime() },
            new(){ CompletionTime = 25.54f, DisplayName = "fake", InstructionsCount = 5, ScoreTime = new DateTime() },
        };

        protected async override Task<List<LeaderboardRecord>> FetchScores()
        {
            await Task.Delay(3000);
            return templateData.Select(e => e).ToList();
        }

        protected override void SendScore(string levelName, float time, int instructions)
        {
            // doing absolutely nothing lol
        }
    }
}
