using RecoDeli.Scripts.Gameplay.Robot;
using RecoDeli.Scripts.Leaderboards;
using RecoDeli.Scripts.Level.Format;
using RecoDeli.Scripts.SaveManagement;
using RecoDeli.Scripts.Settings;
using RecoDeli.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecoDeli.Scripts.Assets.Scripts.Leaderboards
{
    internal class SaveBasedLeaderboardProvider : LeaderboardProvider
    {
        public override bool Simplified => true;

        public SaveBasedLeaderboardProvider(string levelName) : base(levelName)
        {

        }

        private LeaderboardData GetDataFromSaves()
        {
            var data = new LeaderboardData();
            data.InstructionRecords = new();
            data.TimeRecords = new();
            data.TimeStats.Averages = new();
            data.InstructionsStats.Averages = new();

            LoadSaveScores(ref data);
            InjectAuthorTime(ref data);
            SortScores(ref data);

            return data;
        }

        private void LoadSaveScores(ref LeaderboardData data)
        {
            var saveCount = RecoDeliGame.Settings.UserSaveCount;
            for (int userSlot = 0; userSlot < saveCount; userSlot++)
            {
                SaveData slot = null;

                if (userSlot == SaveManager.CurrentSlot)
                {
                    slot = SaveManager.CurrentSave;
                }
                else if (!SaveManager.TryLoadSlot(userSlot, out slot))
                {
                    continue;
                }

                if (!slot.IsLevelComplete(LevelName))
                {
                    continue;
                }

                var levelInfo = slot.GetLevelInfo(LevelName);
                data.InstructionRecords.Add(new()
                {
                    DisplayName = slot.Name,
                    Score = levelInfo.LowestInstructions,
                });
                data.TimeRecords.Add(new()
                {
                    DisplayName = slot.Name,
                    Score = levelInfo.FastestTime,
                });
            }
        }

        private void InjectAuthorTime(ref LeaderboardData data)
        {
            var authorRecordQuery = AuthorRecords.Instance.Records.Where(r => r.MapName == LevelName);
            if (authorRecordQuery.Any())
            {
                var record = authorRecordQuery.First();
                data.InstructionRecords.Add(new()
                {
                    DisplayName = AuthorRecords.Instance.AuthorName,
                    Score = record.Instructions,
                });
                data.TimeRecords.Add(new()
                {
                    DisplayName = AuthorRecords.Instance.AuthorName,
                    Score = record.Time,
                });
            }
        }

        private void SortScores(ref LeaderboardData data)
        {
            data.InstructionRecords = data.InstructionRecords
                .OrderBy(record => record.Score)
                .Select((record, index) => new LeaderboardRecord()
                {
                    DisplayName = record.DisplayName,
                    Score = record.Score,
                    Place = index + 1
                }).ToList();

            data.TimeRecords = data.TimeRecords
                .OrderBy(record => record.Score)
                .Select((record, index) => new LeaderboardRecord()
                {
                    DisplayName = record.DisplayName,
                    Score = record.Score,
                    Place = index + 1
                }).ToList();
        }

        protected async override Task<LeaderboardData> FetchData()
        {
            var dataSource = new TaskCompletionSource<LeaderboardData>();
            await MainThreadExecutor.Run(() => dataSource.SetResult(GetDataFromSaves()));

            return dataSource.Task.Result;
        }

        protected override async Task SendScore(float time, RobotInstruction[] instructions)
        {
            // nothing to do here, scores are saved into the save file already
            await Task.Delay(0);
        }
    }
}