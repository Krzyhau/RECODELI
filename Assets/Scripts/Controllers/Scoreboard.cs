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
        [SerializeField] private float AuthorTime;
        [SerializeField] private int AuthorInstructions;
        [SerializeField] private Text PlayerTime;
        [SerializeField] private Text PlayerTimeList;
        [SerializeField] private Text PlayerTimeScore;
        [SerializeField] private Text PlayerInstructions;
        [SerializeField] private Text PlayerInstructionsList;
        [SerializeField] private Text PlayerInstructionsScore;

        public int Level => LevelID;

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
        public static bool ConnectionFailed { get; private set; }

        private string TimeBoardName => $"{LevelID}-time";
        private string CodeBoardName => $"{LevelID}-code";

        private bool lastSubmitted = false;
        private float lastSubmittedTime;
        private int lastSubmittedInstructions;

        public Scoreboard Instance { get; private set; }

        private void Awake() => Instance = this;

        private void Start()
        {
            if (PlayerName == null || PlayerName.Length == 0) PlayerName = PlayerPrefs.GetString("PlayerName", "Playtester");
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
                    PlayerName = playerName;
                }
                else
                {
                    Debug.Log("Could not start session");
                    ConnectionFailed = true;
                }
                Initiated = true;
            });
            yield return new WaitWhile(() => Initiated == false);
        }

        public void SubmitRecord(float time, int codeCount) => StartCoroutine(SubmitRecordRoutine(time, codeCount));
        private IEnumerator SubmitRecordRoutine(float time, int codeCount)
        {
            lastSubmitted = true;
            lastSubmittedTime = time;
            lastSubmittedInstructions = codeCount;

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
            PlayerTimeList.text = "";
            PlayerTimeScore.text = "";
            PlayerInstructionsList.text = "";
            PlayerInstructionsScore.text = "";


            yield return new WaitWhile(() => Initiated == false);

            bool[] loadTasks = new bool[4];

            bool failedAtLeastOnce = false;

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
                    failedAtLeastOnce = true;
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
                    failedAtLeastOnce = true;
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
                    failedAtLeastOnce = true;
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
                    failedAtLeastOnce = true;
                }
                loadTasks[3] = true;
            });

            yield return new WaitWhile(() => loadTasks.Any(b=>!b));

            if (lastSubmitted)
            {
                PlayerTime.text = $"TIME: {lastSubmittedTime.ToString("0.00")}".ToUpper().Replace(",", ".");
                PlayerInstructions.text = $"INSTRUCTIONS: {lastSubmittedInstructions}".ToUpper().Replace(",", ".");
            }
            else
            {
                PlayerInstructions.text = $"INSTRUCTIONS: {ownCodeRecord.Lines} (#{ownCodeRecord.Place})".ToUpper().Replace(",", ".");
                PlayerTime.text = $"TIME: {ownTimeRecord.Time} (#{ownTimeRecord.Place})".ToUpper().Replace(",", ".");
            }

            if (failedAtLeastOnce)
            {
                PlayerTimeList.text = "SCOREBOARD IS CURRENTLY NOT FUNCTIONING!\n\nOPERATORS ARE ENCOURAGED TO SHARE SCORES THROUGH OTHER COMMUNICATION CHANNELS!";
                PlayerInstructionsList.text += "AS AN ALTERNATIVE, WE ARE *DELIVERING* OUR OWN BEST SCORES:\n\n";
                PlayerInstructionsList.text += $"AUTHOR TIME: {AuthorTime.ToString("0.00").Replace(",", ".")}\n";
                PlayerInstructionsList.text += $"AUTHOR INSTRUCTIONS: {AuthorInstructions}";
            }
            else
            {
                for (int i = 0; i < topTimeRecords.Count; i++)
                {
                    PlayerTimeList.text = $"{PlayerTimeList.text}{topTimeRecords[i].Place}.) " +
                        $"{topTimeRecords[i].PlayerName}\n".Replace(",", ".");
                    PlayerTimeScore.text = $"{PlayerTimeScore.text}{topTimeRecords[i].Time}\n".Replace(",", ".");
                }


                for (int i = 0; i < topCodeRecords.Count; i++)
                {
                    PlayerInstructionsList.text = $"{PlayerInstructionsList.text}{topCodeRecords[i].Place}.) " +
                        $"{topCodeRecords[i].PlayerName}\n".Replace(",", ".");
                    PlayerInstructionsScore.text = $"{PlayerInstructionsScore.text}{topCodeRecords[i].Lines}\n".Replace(",", ".");
                }
            }
        }
        public void ShowScoreboard()
        {
            LoadScores();
        }
    }
}
