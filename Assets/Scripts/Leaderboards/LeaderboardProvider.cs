using Cysharp.Threading.Tasks;
using RecoDeli.Scripts.Gameplay.Robot;
using RecoDeli.Scripts.Utils;
using System;
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

            UniTask.Void(async () =>
            {
                try
                {
                    var scores = await FetchData();

                    CachedData = scores;
                    Status = LoadingStatus.Loaded;
                    OnLoaded?.Invoke();
                }
                catch (Exception ex)
                {
                    Status = LoadingStatus.Failed;
                    OnFailed?.Invoke(ex);
                    Debug.LogError($"Could not load leaderboards: {ex.Message}");
                }
            });
        }

        public void SubmitScore(float time, RobotInstruction[] instructions)
        {
            UniTask.Void(async () =>
            {
                try
                {
                    await SendScore(time, instructions);
                    OnSubmit?.Invoke();
                }
                catch (Exception ex)
                {
                    OnSubmitFailed?.Invoke(ex);
                    Debug.LogError($"Could not submit new score to leaderboard: {ex.Message}");
                }
            });
        }

        protected abstract UniTask<LeaderboardData> FetchData();
        protected abstract UniTask SendScore(float time, RobotInstruction[] instructions);
    }
}
