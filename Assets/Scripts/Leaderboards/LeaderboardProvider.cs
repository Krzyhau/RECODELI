using RecoDeli.Scripts.SaveManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecoDeli.Scripts.Leaderboards
{
    public abstract class LeaderboardProvider
    {
        public Dictionary<string, List<LeaderboardRecord>> CachedRecords = new();
        private LeaderboardRecord ownCachedRecord;
        private string ownCachedRecordLevelName;

        public bool HasScoresCached(string levelName)
        {
            return CachedRecords.ContainsKey(levelName);
        }

        public void RequestScores(string levelName, Action<List<LeaderboardRecord>> onLoaded, Action<Exception> onFailed) {
            if (HasScoresCached(levelName))
            {
                onLoaded(CachedRecords[levelName]);
                return;
            }

            Task.Run(async () =>
            {
                try
                {
                    List<LeaderboardRecord> scores = await FetchScores();
                    if(ownCachedRecordLevelName == levelName)
                    {
                        scores.Add(ownCachedRecord);
                    }
                    CachedRecords[levelName] = scores;
                    onLoaded(scores);
                }
                catch (Exception ex)
                {
                    onFailed(ex);
                }
            });
        }

        public void SubmitScore(string levelName, float time, int instructions)
        {
            SendScore(levelName, time, instructions);
            ownCachedRecord = new()
            {
                DisplayName = SaveManager.CurrentSave.Name,
                CompletionTime = time,
                InstructionsCount = instructions,
                ScoreTime = DateTime.Now
            };
            ownCachedRecordLevelName = levelName;
        }

        protected abstract Task<List<LeaderboardRecord>> FetchScores();
        protected abstract void SendScore(string levelName, float time, int instructions);
    }
}
