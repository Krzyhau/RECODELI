using RecoDeli.Scripts.Controllers;
using RecoDeli.Scripts.SaveManagement;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class EndingInterface : MonoBehaviour
    {
        [SerializeField] private UIDocument endingDocument;
        [SerializeField] private EndingController endingController;

        [Header("Settings")]
        [SerializeField] private string timerFormat = "0.000";

        private VisualElement leaderboardTabs;
        private VisualElement leaderboardView;

        private Button restartButton;
        private Button continueButton;

        private Label timeLabel;
        private Label instructionsLabel;
        private Label attemptsLabel;

        private void Awake()
        {
            restartButton = endingDocument.rootVisualElement.Q<Button>("restart-button");
            continueButton = endingDocument.rootVisualElement.Q<Button>("continue-button");

            timeLabel = endingDocument.rootVisualElement.Q<Label>("stat-time");
            instructionsLabel = endingDocument.rootVisualElement.Q<Label>("stat-instructions");
            attemptsLabel = endingDocument.rootVisualElement.Q<Label>("stat-attempts");

            leaderboardTabs = endingDocument.rootVisualElement.Q<VisualElement>("leaderboards-tabs");
            leaderboardView = endingDocument.rootVisualElement.Q<VisualElement>("leaderboards-views");
            InitializeTabs();

            restartButton.clicked += endingController.SimulationManager.RestartSimulation;
            continueButton.clicked += endingController.FinalizeEnding;
        }

        private void InitializeTabs()
        {
            foreach(var tab in leaderboardTabs.Children())
            {
                if (!(tab is Button tabButton)) continue;
                tabButton.clicked += () => ShowTab(leaderboardTabs.IndexOf(tab)); 
            }
            ShowTab(0);
        }

        private void ShowTab(int index)
        {
            for (int i = 0; i < leaderboardTabs.childCount; i++)
            {
                if (!(leaderboardTabs.ElementAt(i) is Button tabButton)) continue;
                tabButton.SetEnabled(index != i);
            }
        }

        public void ShowInterface(bool show)
        {
            endingDocument.rootVisualElement.SetEnabled(show);

            if (show)
            {
                var time = endingController.SimulationManager.SimulationTime;
                var timeString = time.ToString(timerFormat);
                var timeSegments = timeString.Split('.');
                var milliseconds = timeSegments.Length > 1 ? timeSegments[1] : "0";

                var instructions = endingController.SimulationManager.RobotController.CurrentInstructions.Length;
                var attempts = SaveManager.CurrentSave.GetCurrentLevelInfo().ExecutionCount;

                timeLabel.text = $"{timeSegments[0]}.{milliseconds}";
                instructionsLabel.text = $"{instructions}";
                attemptsLabel.text = $"{attempts}";
            }
        }
    }
}