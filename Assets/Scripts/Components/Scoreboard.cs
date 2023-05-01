using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace LudumDare.Scripts.Components
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField] private int LevelID;

        public struct TimeRecord
        {
            public int Place;
            public string PlayerName;
            public float Time;
        }

        public struct CodeRecord
        {
            public int Place;
            public string PlayerName;
            public int Lines;
        }

        public static string PlayerName { get; private set; }
        public static bool Initiated { get; private set; }

        private string TimeBoardName => $"{LevelID}-time";
        private string CodeBoardName => $"{LevelID}-code";

        public Scoreboard Instance { get; private set; }

        private void Awake() => Instance = this;

        private void Start()
        {
            PlayerName = "Krzyhau"; // TODO: do ustawienia
            if (!Initiated && PlayerName.Length > 0) Login(PlayerName);
        }

        public void Login(string playerName) => StartCoroutine(LoginRoutine(PlayerName));
        public IEnumerator LoginRoutine(string playerName)
        {
            LootLockerSDKManager.StartGuestSession((response) =>
            {
                if (response.success)
                {
                    Debug.Log("Player was logged in");
                    Initiated = true;
                    PlayerName = playerName;
                }
                else
                {
                    Debug.Log("Could not start session");
                }
            });
            yield return new WaitWhile(() => Initiated == false);
        }

        public void SubmitRecord(float time, int codeCount) => StartCoroutine(SubmitRecordRoutine(time, codeCount));
        private IEnumerator SubmitRecordRoutine(float time, int codeCount)
        {
            yield return new WaitWhile(() => Initiated == false);

            bool doneTime = false;
            bool doneCode = false;

            LootLockerSDKManager.SubmitScore(PlayerName, Mathf.FloorToInt(time * 100), TimeBoardName, (response) =>
            {
                if (response.success)
                {
                    Debug.Log("Successfully uploaded time score");
                }
                else
                {
                    Debug.Log("Failed " + response.Error);
                }
                doneTime = true;
            });

            LootLockerSDKManager.SubmitScore(PlayerName, codeCount, CodeBoardName, (response) =>
            {
                if (response.success)
                {
                    Debug.Log("Successfully uploaded code score");
                }
                else
                {
                    Debug.Log("Failed " + response.Error);
                    
                }
                doneCode = true;
            });

            yield return new WaitWhile(() => doneTime == false || doneCode == false);
        }


        public void LoadScores() => StartCoroutine(LoadScoresRoutine());
        public IEnumerator LoadScoresRoutine()
        {
            yield return new WaitWhile(() => Initiated == false);

            bool[] loadTasks = new bool[4];

            const int topCount = 10;

            TimeRecord ownTimeRecord;
            CodeRecord ownCodeRecord;

            List<TimeRecord> topTimeRecords;
            List<CodeRecord> topCodeRecords;

            LootLockerSDKManager.GetMemberRank(TimeBoardName, PlayerName, (response) =>
            {
                if (response.statusCode == 200)
                {
                    Debug.Log("Successful");
                    ownTimeRecord = new TimeRecord()
                    {
                        PlayerName = response.member_id,
                        Place = response.rank,
                        Time = response.score / 100.0f
                    };
                }
                else
                {
                    Debug.Log("failed: " + response.Error);
                }
                loadTasks[0] = true;
            });

            LootLockerSDKManager.GetMemberRank(CodeBoardName, PlayerName, (response) =>
            {
                if (response.statusCode == 200)
                {
                    Debug.Log("Successful");
                    ownCodeRecord = new CodeRecord()
                    {
                        PlayerName = response.member_id,
                        Place = response.rank,
                        Lines = response.score
                    };
                }
                else
                {
                    Debug.Log("failed: " + response.Error);
                }
                loadTasks[0] = true;
            });


            LootLockerSDKManager.GetScoreList(TimeBoardName, topCount, 0, (response) =>
            {
                if (response.statusCode == 200)
                {
                    Debug.Log("Successful");
                    topTimeRecords = response.items.Select(item => new TimeRecord()
                    {
                        PlayerName = item.member_id,
                        Place = item.rank,
                        Time = item.score / 100.0f
                    }).ToList();
                }
                else
                {
                    Debug.Log("failed: " + response.Error);
                }
            });

            LootLockerSDKManager.GetScoreList(CodeBoardName, topCount, 0, (response) =>
            {
                if (response.statusCode == 200)
                {
                    Debug.Log("Successful");
                    topCodeRecords = response.items.Select(item => new CodeRecord()
                    {
                        PlayerName = item.member_id,
                        Place = item.rank,
                        Lines = item.score
                    }).ToList();
                }
                else
                {
                    Debug.Log("failed: " + response.Error);
                }
            });

            yield return new WaitWhile(() => loadTasks.Any(b=>!b));

            // TODO - wsadŸ to do gotowych divów jak ju¿ bêd¹ zrobione
        }
    }
}
