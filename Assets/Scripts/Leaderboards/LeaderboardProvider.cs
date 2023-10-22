using RecoDeli.Scripts.Gameplay.Robot;
using RecoDeli.Scripts.Utils;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace RecoDeli.Scripts.Leaderboards
{
    public abstract class LeaderboardProvider
    {
        public enum LoadingStatus
        {
            NotLoaded,
            Loading,
            Loaded,
            Failed
        }

        private LeaderboardRecord? ownCachedRecord;

        public string LevelName { get; private set; }
        public LeaderboardData CachedData { get; protected set; }
        public LoadingStatus Status { get; private set; }


        public Action OnLoaded;
        public Action<Exception> OnFailed;
        public Action OnSubmit;
        public Action<Exception> OnSubmitFailed;

        public LeaderboardProvider(string levelName)
        {
            CachedData = new();
            LevelName = levelName;
            Status = LoadingStatus.NotLoaded;
        }

        public void RequestScores() {
            if (Status == LoadingStatus.Loading)
            {
                return;
            }
            Status = LoadingStatus.Loading;

            Task.Run(async () =>
            {
                try
                {
                    var scores = await FetchData();

                    if (ownCachedRecord.HasValue)
                    {
                        scores.Records.Add(ownCachedRecord.Value);
                    }

                    CachedData = scores;
                    Status = LoadingStatus.Loaded;
                    MainThreadExecutor.Run(() => OnLoaded?.Invoke());
                    Debug.Log("AAAA");
                }
                catch (Exception ex)
                {
                    Status = LoadingStatus.Failed;
                    Debug.LogError($"Could not load leaderboards: {ex.Message}");
                    MainThreadExecutor.Run(() => OnFailed?.Invoke(ex));
                }
            });
        }

        public void SubmitScore(float time, RobotInstruction[] instructions)
        {
            Task.Run(async () =>
            {
                try
                {
                    await SendScore(time, instructions);
                    MainThreadExecutor.Run(() => OnSubmit?.Invoke());
                }
                catch (Exception ex)
                {
                    MainThreadExecutor.Run(() => OnSubmitFailed?.Invoke(ex));
                    Debug.LogError($"Could not submit new score to leaderboard: {ex.Message}");
                }
            });
        }

        protected abstract Task<LeaderboardData> FetchData();
        protected abstract Task SendScore(float time, RobotInstruction[] instructions);
    }
}
