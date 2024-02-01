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

        public virtual bool Simplified => false;
        public string LevelName { get; private set; }
        public LeaderboardData CachedData { get; protected set; }
        public LoadingStatus Status { get; private set; }


        public Action OnLoaded;
        public Action<Exception> OnFailed;
        public Action OnSubmit;
        public Action<Exception> OnSubmitFailed;

        public LeaderboardProvider(string levelName)
        {
            LevelName = levelName;
            Reset();
        }

        public void Reset()
        {
            CachedData = new();
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

                    CachedData = scores;
                    Status = LoadingStatus.Loaded;
                    await MainThreadExecutor.Run(() => OnLoaded?.Invoke());
                }
                catch (Exception ex)
                {
                    Status = LoadingStatus.Failed;
                    await MainThreadExecutor.Run(() => OnFailed?.Invoke(ex));
                    Debug.LogError($"Could not load leaderboards: {ex.Message}");
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
                    await MainThreadExecutor.Run(() => OnSubmit?.Invoke());
                }
                catch (Exception ex)
                {
                    await MainThreadExecutor.Run(() => OnSubmitFailed?.Invoke(ex));
                    Debug.LogError($"Could not submit new score to leaderboard: {ex.Message}");
                }
            });
        }

        protected abstract Task<LeaderboardData> FetchData();
        protected abstract Task SendScore(float time, RobotInstruction[] instructions);
    }
}
