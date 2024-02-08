using RecoDeli.Scripts.Gameplay.Robot;
using RecoDeli.Scripts.Leaderboards;
using RecoDeli.Scripts.SaveManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace RecoDeli.Scripts.Assets.Scripts.Leaderboards
{
    public class MyOwnShittyLeaderboardProvider : LeaderboardProvider
    {
        [Serializable]
        private struct UserStats
        {
            public string username;
            public string levelname;
            public float completion_time;
            public float instructions_count;
            public int time_rank;
            public int instructions_rank;
        }

        [Serializable]
        private struct LevelStats
        {
            public string levelname;
            public int records_count;
            public float best_record;
            public float worst_record;
            public float[] bars;
        }

        [Serializable]
        private struct UserRank
        {
            public int rank;
            public string username;
            public float score;
        }

        [Serializable]
        private struct UserRankArray
        {
            public UserRank[] Items;
        }



        private const string API_ADDRESS = "http://localhost:3000";
        public MyOwnShittyLeaderboardProvider(string levelName) : base(levelName)
        {

        }

        private string GetUserIdentifier()
        {
            return SaveManager.CurrentSave.Name;
        }

        private async UniTask<UserStats> GetUserStats(string username)
        {
            var query = $"?levelname={this.LevelName}&username={GetUserIdentifier()}";
            var userInfoRequest = await RequestResource("/userscore" + query);

            if (userInfoRequest.status != HttpStatusCode.OK)
            {
                throw new Exception($"Cannot fetch user data from leaderboards ({userInfoRequest.status})");
            }

            return JsonUtility.FromJson<UserStats>(userInfoRequest.text);
        }

        private async UniTask<LeaderboardStats> GetLevelStats(string type, int maxBars)
        {
            var stats = new LeaderboardStats();

            var query = $"?levelname={this.LevelName}&maxbars={maxBars}";
            var levelStatsRequest = await RequestResource($"/{type}/stats" + query);

            if (levelStatsRequest.status != HttpStatusCode.OK)
            {
                throw new Exception($"Cannot fetch stats data from leaderboards ({levelStatsRequest.status})");
            }

            var data = JsonUtility.FromJson<LevelStats>(levelStatsRequest.text);
            stats.RecordCount = data.records_count;
            stats.BestRecord = data.best_record;
            stats.WorstRecord = data.worst_record;
            stats.Averages = data.bars.ToList();

            return stats;
        }

        private async UniTask<List<LeaderboardRecord>> GetLevelRecords(string type, int limit, int offset)
        {
            var stats = new List<LeaderboardRecord>();

            var query = $"?levelname={this.LevelName}&limit={limit}&offset={offset}";
            var levelRecordsRequest = await RequestResource($"/{type}/get" + query);

            if (levelRecordsRequest.status != HttpStatusCode.OK)
            {
                throw new Exception($"Cannot fetch record data from leaderboards ({levelRecordsRequest.status})");
            }

            var data = JsonUtility.FromJson<UserRankArray>("{\"Items\":" + levelRecordsRequest.text + "}");
            foreach(var record in data.Items)
            {
                stats.Add(new()
                {
                    Place = record.rank,
                    DisplayName = record.username,
                    Score = record.score
                });
            }

            return stats;
        }

        protected async override UniTask<LeaderboardData> FetchData()
        {
            // we first want to figure out which scores we want to see based on own score
            var userStats = await GetUserStats(GetUserIdentifier());
            var timeRank = userStats.time_rank;
            var instructionsRank = userStats.instructions_rank;

            // now based on that, we can fetch specific scores
            var timeStatsTask = GetLevelStats("times", 16);
            var instructionsStatsTask = GetLevelStats("instructions", 32);

            const int range = 25;
            bool usingTwoTimesRanges = timeRank > range + range;
            bool usingTwoInstructionsRanges = instructionsRank > range + range;

            var timeScoresTask = GetLevelRecords("times", usingTwoTimesRanges ? (timeRank + range / 2) : range, 0);
            var instructionScoresTask = GetLevelRecords("instructions", usingTwoInstructionsRanges ? (instructionsRank + range / 2) : range, 0);

            var boardTasks = new List<UniTask>()
            {
                timeStatsTask,
                instructionsStatsTask,
                timeScoresTask,
                instructionScoresTask
            };

            UniTask<List<LeaderboardRecord>> secondTimeScoresTask;
            if (usingTwoTimesRanges)
            {
                secondTimeScoresTask = GetLevelRecords("times", range, timeRank - range / 2);
                boardTasks.Add(secondTimeScoresTask);
            }

            UniTask<List<LeaderboardRecord>> secondInstructionScoresTask;
            if (usingTwoInstructionsRanges)
            {
                secondInstructionScoresTask = GetLevelRecords("instructions", range, instructionsRank - range / 2);
                boardTasks.Add(secondInstructionScoresTask);
            }

            await UniTask.WhenAll(boardTasks);

            // task completed! return the data
            return new()
            {
                TimeStats = timeStatsTask.GetAwaiter().GetResult(),
                InstructionsStats = instructionsStatsTask.GetAwaiter().GetResult(),
                TimeRecords = timeScoresTask.GetAwaiter().GetResult(),
                InstructionRecords = instructionScoresTask.GetAwaiter().GetResult()
            };
        }

        protected async override UniTask SendScore(float time, RobotInstruction[] instructions)
        {
            var instructionsString = RobotInstruction.ListToString(instructions.ToList()).Replace("\n", "\\n");

            var postData = "{"
                + $"\"levelname\": \"{this.LevelName}\","
                + $"\"username\": \"{GetUserIdentifier()}\","
                + $"\"completion_time\": {time},"
                + $"\"instructions_count\": {instructions.Length},"
                + $"\"instructions\": \"{instructionsString}\""
                + "}";

            var submitRequest = await RequestResource("/submit", postData);

            if(submitRequest.status != HttpStatusCode.OK)
            {
                throw new Exception($"Failed to submit the score ({submitRequest.status})");
            }
        }

        public async UniTask<(string text, HttpStatusCode status)> RequestResource(string endpoint, string postData = null)
        {
            using var httpClient = new HttpClient();
            
            httpClient.BaseAddress = new Uri(API_ADDRESS);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            httpClient.Timeout = TimeSpan.FromSeconds(10);

            HttpResponseMessage response;
            if (postData != null)
            {
                var content = new StringContent(postData, Encoding.UTF8, "application/json");
                response = await httpClient.PostAsync(endpoint, content);
            }
            else
            {
                response = await httpClient.GetAsync(endpoint);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return ("", response.StatusCode);
            }

            var responseString = await response.Content.ReadAsStringAsync();

            return (responseString, response.StatusCode);
            
        }
    }
}
