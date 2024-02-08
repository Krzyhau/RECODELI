using Cysharp.Threading.Tasks;
using RecoDeli.Scripts.Gameplay.Robot;
using RecoDeli.Scripts.SaveManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Random = UnityEngine.Random;

namespace RecoDeli.Scripts.Leaderboards
{
    public class TestLeaderboardProvider : LeaderboardProvider
    {

        private List<LeaderboardRecord> fakeTimeRecords;
        private List<LeaderboardRecord> fakeInstructionRecords;

        private static readonly string[] names = { "Bob", "Gamer", "Deliver", "Inpost", "Xddd", "Szmati", "Box", "Las", "Fok", "John", "ScriptKiddie" };
        public TestLeaderboardProvider(string levelName) : base(levelName) {

            fakeTimeRecords = new();
            fakeInstructionRecords = new();

            for (var i=0; i < 20; i++)
            {
                var fakeUserName = names[Random.Range(0, names.Length)];
                fakeUserName = Random.Range(0, 4) switch
                {
                    0 => fakeUserName.ToLower(),
                    1 => fakeUserName.ToUpper(),
                    _ => fakeUserName,
                };

                fakeUserName = Random.Range(0, 4) switch
                {
                    0 => fakeUserName + "XX",
                    1 => fakeUserName + Random.Range(0, 3000),
                    _ => fakeUserName,
                };

                while(fakeTimeRecords.Where(r => r.DisplayName == fakeUserName).Any())
                {
                    fakeUserName += "_";
                }

                fakeTimeRecords.Add(new(){Score = Random.Range(3.0f, 40.0f), DisplayName = fakeUserName});
                fakeInstructionRecords.Add(new(){Score = Random.Range(1, 20), DisplayName = fakeUserName});
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

        protected async override UniTask<LeaderboardData> FetchData()
        {
            await UniTask.Delay(3000);

            var data = new LeaderboardData();
            data.InstructionRecords = new();
            data.TimeRecords = new();

            // update place indicators for records
            var instructionPlaces = fakeInstructionRecords.Select(r => r.Score).OrderBy(i => i).ToList();
            var timePlaces = fakeTimeRecords.Select(r => r.Score).OrderBy(i => i).ToList();

            for(int i=0;i< timePlaces.Count;i++)
            {
                var record = fakeInstructionRecords[i];
                record.Place = instructionPlaces.IndexOf(record.Score) + 1;
                data.InstructionRecords.Add(record);
            }

            for (int i = 0; i < instructionPlaces.Count; i++)
            {
                var record = fakeTimeRecords[i];
                record.Place = timePlaces.IndexOf(record.Score) + 1;
                data.TimeRecords.Add(record);
            }

            data.InstructionRecords = data.InstructionRecords.OrderBy(r => r.Place).ToList();
            data.TimeRecords = data.TimeRecords.OrderBy(r => r.Place).ToList();

            data.InstructionsStats = GenerateStats(data.InstructionRecords.Select(r => r.Score).ToList(), 32);
            data.TimeStats = GenerateStats(data.TimeRecords.Select(r => r.Score).ToList(), 16);

            return data;
        }

        protected async override UniTask SendScore(float time, RobotInstruction[] instructions)
        {
            await UniTask.Delay(100);

            var name = SaveManager.CurrentSave.Name;

            fakeTimeRecords.RemoveAll(x => x.DisplayName == name && x.Score >= time);
            fakeInstructionRecords.RemoveAll(x => x.DisplayName == name && x.Score >= instructions.Length);

            fakeTimeRecords.Add(new(){ DisplayName = name, Score = time });
            fakeInstructionRecords.Add(new(){ DisplayName = name, Score = instructions.Length });
        }
    }
}
