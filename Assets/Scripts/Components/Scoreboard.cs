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

        public static string PlayerName { get; private set; }
        public static bool Initiated { get; private set; }

        private string TimeBoardName => $"{LevelID}-time";
        private string CodeBoardName => $"{LevelID}-code";

        public Scoreboard Instance { get; private set; }

        private void Awake() => Instance = this;

        private void Start()
        {
            PlayerName = "Krzyhau"; // TODO: do ustawienia
            if (!Initiated && PlayerName.Length > 0) StartCoroutine(LoginRoutine(PlayerName));
        }

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


        public IEnumerator SubmitRecordRoutine(float time, int codeCount)
        {
            yield return new WaitWhile(() => Initiated == false);

            bool doneTime = false;
            bool doneCode = false;
            string playerID = PlayerName;

            LootLockerSDKManager.SubmitScore(playerID, Mathf.FloorToInt(time * 100), TimeBoardName, (response) =>
            {
                if (response.success)
                {
                    Debug.Log("Successfully uploaded time score");
                    doneTime = true;
                }
                else
                {
                    Debug.Log("Failed " + response.Error);
                    doneTime = true;
                }
            });

            LootLockerSDKManager.SubmitScore(playerID, codeCount, CodeBoardName, (response) =>
            {
                if (response.success)
                {
                    Debug.Log("Successfully uploaded code score");
                    doneCode = true;
                }
                else
                {
                    Debug.Log("Failed " + response.Error);
                    doneCode = true;
                }
            });

            yield return new WaitWhile(() => doneTime == false || doneCode == false);
        }

    }
}
