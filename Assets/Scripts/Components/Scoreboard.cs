using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace LudumDare.Scripts.Components
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField] private int LevelID;
        [SerializeField] private Text PlayerTime;
        [SerializeField] private Text PlayerTimeList;
        [SerializeField] private Text PlayerTimeScore;
        [SerializeField] private Text PlayerInstructions;
        [SerializeField] private Text PlayerInstructionsList;
        [SerializeField] private Text PlayerInstructionsScore;


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

        public static string PlayerName { get; set; }
        public static bool Initiated { get; private set; }

        private string TimeBoardName => $"{LevelID}-time";
        private string CodeBoardName => $"{LevelID}-code";

        public Scoreboard Instance { get; private set; }

        private void Awake() => Instance = this;

        private void Start()
        {
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

            var ownTimeRecord = new TimeRecord();
            var ownCodeRecord = new CodeRecord();

            var topTimeRecords = new List<TimeRecord>();
            var topCodeRecords = new List<CodeRecord>();

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
                loadTasks[1] = true;
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
                loadTasks[2] = true;
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
                loadTasks[3] = true;
            });

            yield return new WaitWhile(() => loadTasks.Any(b=>!b));
            PlayerTimeList.text = "";
            PlayerTimeScore.text = "";
            PlayerInstructionsList.text = "";
            PlayerInstructionsScore.text = "";
            PlayerTime.text = $"TIME: {ownTimeRecord.Time} (#{ownTimeRecord.Place})".ToUpper().Replace(",", ".");
            for(int i = 0; i < topTimeRecords.Count; i++)
            {
                PlayerTimeList.text = $"{PlayerTimeList.text}{topTimeRecords[i].Place}.) " +
                    $"{topTimeRecords[i].PlayerName}\n".Replace(",",".");
                PlayerTimeScore.text = $"{PlayerTimeScore.text}{topTimeRecords[i].Time}\n".Replace(",", ".");
            }

            PlayerInstructions.text = $"INSTRUCTIONS: {ownCodeRecord.Lines} (#{ownCodeRecord.Place})".ToUpper().Replace(",", ".");
            for (int i = 0; i < topCodeRecords.Count; i++)
            {
                PlayerInstructionsList.text = $"{PlayerInstructionsList.text}{topCodeRecords[i].Place}.) " +
                    $"{topCodeRecords[i].PlayerName}\n".Replace(",", ".");
                PlayerInstructionsScore.text = $"{PlayerInstructionsScore.text}{topCodeRecords[i].Lines}\n".Replace(",", ".");
            }
        }
        public void ShowScoreboard()
        {
            LoadScores();
        }
    }
}
