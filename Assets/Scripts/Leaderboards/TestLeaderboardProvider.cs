using RecoDeli.Scripts.Gameplay.Robot;
using RecoDeli.Scripts.SaveManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using Random = UnityEngine.Random;

namespace RecoDeli.Scripts.Leaderboards
{
    public class TestLeaderboardProvider : LeaderboardProvider
    {

        private List<LeaderboardRecord> fakeRecords;

        private static readonly string[] names = { "Bob", "Gamer", "Deliver", "Inpost", "Xddd", "Szmati", "Box", "Las", "Fok", "John", "ScriptKiddie" };
        public TestLeaderboardProvider(string levelName) : base(levelName) {

            fakeRecords = new();

            for (var i=0; i < 20; i++)
            {
                var record = new LeaderboardRecord();


                record.DisplayName = names[Random.Range(0, names.Length)];
                record.DisplayName = Random.Range(0, 4) switch
                {
                    0 => record.DisplayName.ToLower(),
                    1 => record.DisplayName.ToUpper(),
                    _ => record.DisplayName,
                };

                record.DisplayName = Random.Range(0, 4) switch
                {
                    0 => record.DisplayName + "XX",
                    1 => record.DisplayName + Random.Range(0, 3000),
                    _ => record.DisplayName,
                };

                while(fakeRecords.Where(r => r.DisplayName == record.DisplayName).Any())
                {
                    record.DisplayName += "_";
                }

                record.InstructionsCount = new() { Place = 0, Value = Random.Range(1, 20) };
                record.CompletionTime = new() { Place = 0, Value = Random.Range(3.0f, 40.0f) };

                record.ScoreRecordedAt = DateTime.Now;

                fakeRecords.Add(record);
            }
        }

        private LeaderboardStats GenerateStats(List<float> values, float maxBars)
        {
            var stats = new LeaderboardStats();
            stats.RecordCount = values.Count;
            stats.WorstRecord = values.Max();
            stats.BestRecord = values.Min();
            stats.Averages = new List<float>();

            float barsCount = Mathf.Ceil(Mathf.Min(maxBars, stats.WorstRecord - stats.BestRecord));
            for(int i=0; i < barsCount; i++)
            {
                float rangeMin = Mathf.Lerp(stats.BestRecord, stats.WorstRecord, i / barsCount);
                float rangeMax = Mathf.Lerp(stats.BestRecord, stats.WorstRecord, (i+1) / barsCount);

                stats.Averages.Add(values.Where(x => x >= rangeMin && x <= rangeMax).Sum());
            }
            var maxAverage = stats.Averages.Max();
            if(maxAverage > 0)
            {
                stats.Averages = stats.Averages.Select(a => a / maxAverage).ToList();
            }

            return stats;
        }

        protected async override Task<LeaderboardData> FetchData()
        {
            await Task.Delay(3000);

            var data = new LeaderboardData();
            data.Records = new();

            // update place indicators for records
            var instructionPlaces = fakeRecords.Select(r => r.InstructionsCount.Value).OrderBy(i => i).ToList();
            var timePlaces = fakeRecords.Select(r => r.CompletionTime.Value).OrderBy(i => i).ToList();

            for(int i=0;i<fakeRecords.Count;i++)
            {
                var record = fakeRecords[i];
                record.InstructionsCount.Place = instructionPlaces.IndexOf(record.InstructionsCount.Value) + 1;
                record.CompletionTime.Place = timePlaces.IndexOf(record.CompletionTime.Value) + 1;

                data.Records.Add(record);
            }

            data.InstructionsStats = GenerateStats(data.Records.Select(r => r.InstructionsCount.Value).ToList(), 32);
            data.TimeStats = GenerateStats(data.Records.Select(r => r.CompletionTime.Value).ToList(), 16);

            return data;
        }

        protected async override Task SendScore(float time, RobotInstruction[] instructions)
        {
            await Task.Delay(100);

            var name = SaveManager.CurrentSave.Name;

            var record = new LeaderboardRecord()
            {
                DisplayName = SaveManager.CurrentSave.Name,
                CompletionTime = {Value = time},
                InstructionsCount = {Value = instructions.Length},
            };

            var ownScore = fakeRecords.Where(r => r.DisplayName == name);
            if (ownScore.Any())
            {
                var ownRecord = ownScore.First();

                record.CompletionTime.Value = Mathf.Min(record.CompletionTime.Value, ownRecord.CompletionTime.Value);
                record.InstructionsCount.Value = Mathf.Min(record.InstructionsCount.Value, ownRecord.InstructionsCount.Value);

                fakeRecords.Remove(ownScore.First());
            }

            fakeRecords.Add(record);
        }
    }
}
