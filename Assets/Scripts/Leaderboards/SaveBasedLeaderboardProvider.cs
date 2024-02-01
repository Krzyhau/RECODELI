using RecoDeli.Scripts.Gameplay.Robot;
using RecoDeli.Scripts.Leaderboards;
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

            var saveCount = RecoDeliGame.Settings.UserSaveCount;
            for (int userSlot = 0; userSlot < saveCount; userSlot++)
            {
                SaveData slot = null;

                if(userSlot == SaveManager.CurrentSlot)
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

            return data;
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