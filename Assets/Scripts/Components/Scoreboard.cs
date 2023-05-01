using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace LudumDare.Scripts.Components
{
    public static class Scoreboard
    {
        public struct TimeRecord
        {
            public int Place;
            public string PlayerName;
            public float Time;
        }

        public static string PlayerName { get; private set; }
        public static bool Initiated { get; private set; }

        private static string TimeBoardName(int levelID) => $"{levelID}-time";
        private static string CodeBoardName(int levelID) => $"{levelID}-code";

        private static async Task<bool> EnsureConnected()
        {
            var result = new TaskCompletionSource<bool>();
            Debug.Log("HALO 1");
            LootLockerSDKManager.StartGuestSession(response =>
            {
                Debug.Log("HALO 2");
                result.SetResult(response.success);
                Debug.Log(response.text);
            });

            Debug.Log("HALO 3");
            return await result.Task;
        }

        public static async Task<List<TimeRecord>> GetTimeRecordsForLevel(int level)
        {
            bool state = await EnsureConnected();
            if(!state) return new List<TimeRecord>();

            var result = new TaskCompletionSource<List<TimeRecord>>();

            LootLockerSDKManager.GetScoreList(TimeBoardName(level), 10, 0, (response) =>
            {
                if (response.statusCode == 200)
                {
                    result.SetResult(response.items.Select(i=> new TimeRecord()
                    {
                        Place = i.rank,
                        PlayerName = i.member_id,
                        Time = i.score / 100.0f
                    }).ToList());
                }
                else
                {
                    result.SetResult(new List<TimeRecord>());
                }
            });

            return await result.Task;
        }



        public static async Task<bool> SubmitScore(int level, float time, int instructions)
        {
            bool state = await EnsureConnected();
            if (!state)
            {
                Debug.Log("Couldn't submit - cannot ensure guest connection");
                return false;
            }
            Debug.Log("HALO 4");

            var submitTime = new TaskCompletionSource<bool>();
            var submitInstructions = new TaskCompletionSource<bool>();

            LootLockerSDKManager.SubmitScore(PlayerName, Mathf.FloorToInt(time * 100.0f), TimeBoardName(level), (response) =>
            {
                submitTime.SetResult(response.statusCode == 200);
            });

            LootLockerSDKManager.SubmitScore(PlayerName, instructions, CodeBoardName(level), (response) =>
            {
                submitInstructions.SetResult(response.statusCode == 200);
            });

            bool[] results = await Task.WhenAll(submitTime.Task, submitInstructions.Task);
            return results[0] && results[1];
        }

    }
}
