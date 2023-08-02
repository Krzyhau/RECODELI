using RecoDeli.Scripts.Controllers;
using RecoDeli.Scripts.SaveManagement;
using System.Collections.Generic;
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

        private Button restartButton;
        private Button continueButton;
        private Button friendsStatsButton;
        private Button globalStatsButton;

        private Label timeLabel;
        private Label instructionsLabel;
        private Label attemptsLabel;

        private VisualElement barsContainer;
        private VisualElement graphIndicator;
        private Label graphIndicatorLabel;
        private VisualElement graphLabels;
        private ScrollView scores;

        private enum Tab { Times, Instructions }
        private Tab displayedTab;
        private bool onlyFriends;

        private void Awake()
        {
            restartButton = endingDocument.rootVisualElement.Q<Button>("restart-button");
            continueButton = endingDocument.rootVisualElement.Q<Button>("continue-button");
            friendsStatsButton = endingDocument.rootVisualElement.Q<Button>("friends-stats-button");
            globalStatsButton = endingDocument.rootVisualElement.Q<Button>("global-stats-button");

            barsContainer = endingDocument.rootVisualElement.Q<VisualElement>("leaderboards-chart-bars");
            graphIndicator = endingDocument.rootVisualElement.Q<VisualElement>("leaderboards-chart-score");
            graphIndicatorLabel = endingDocument.rootVisualElement.Q<Label>("leaderboards-chart-score-label");
            graphLabels = endingDocument.rootVisualElement.Q<VisualElement>("leaderboards-chart-labels");
            scores = endingDocument.rootVisualElement.Q<ScrollView>("leaderboards-scores");

            timeLabel = endingDocument.rootVisualElement.Q<Label>("stat-time");
            instructionsLabel = endingDocument.rootVisualElement.Q<Label>("stat-instructions");
            attemptsLabel = endingDocument.rootVisualElement.Q<Label>("stat-attempts");

            leaderboardTabs = endingDocument.rootVisualElement.Q<VisualElement>("leaderboards-tabs");
            InitializeTabs();


            restartButton.clicked += endingController.SimulationManager.RestartSimulation;
            continueButton.clicked += endingController.FinalizeEnding;

            friendsStatsButton.clicked += () => SetFriendsOnly(true);
            globalStatsButton.clicked += () => SetFriendsOnly(false);
            SetFriendsOnly(true);
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
            displayedTab = (Tab)index;

            RefreshDataDisplay();
        }

        private void SetFriendsOnly(bool state)
        {
            onlyFriends = state;
            friendsStatsButton.SetEnabled(!state);
            globalStatsButton.SetEnabled(state);
            RefreshDataDisplay();
        }


        private void SetGraphIndicator(float position, int topPercentage)
        {
            if(position >= 1.0f)
            {
                graphIndicator.style.display = DisplayStyle.None;
                return;
            }
            
            graphIndicator.style.display = DisplayStyle.Flex;
            graphIndicator.style.left = Length.Percent(position * 100.0f);

            graphIndicator.EnableInClassList("top50", position < 0.5f);
            graphIndicatorLabel.text = $"YOU (TOP {topPercentage}%)";
        }

        private void SetGraphBars(List<float> values)
        {
            barsContainer.Clear();

            foreach(var value in values)
            {
                var newBar = new VisualElement();
                newBar.style.height = Length.Percent(value * 100.0f);
                barsContainer.Add(newBar);
            }
        }

        private void SetGraphLabels(List<string> labels)
        {
            graphLabels.Clear();

            foreach (var labelText in labels)
            {
                var label = new Label(labelText);
                graphLabels.Add(label);
            }
        }

        private void SetScores(Dictionary<string, string> scoresData)
        {
            scores.Clear();

            foreach(var score in scoresData)
            {
                var scoreTab = new VisualElement() { name = "score" };
                scoreTab.Add(new Label(score.Key));
                scoreTab.Add(new Label(score.Value));

                // TODO: make a proper user recognition for highlight
                if(score.Key == "YOU")
                {
                    scoreTab.AddToClassList("own");
                }

                scores.Add(scoreTab);
            }
        }

        private void RefreshDataDisplay()
        {
            if(displayedTab == Tab.Times)
            {
                SetGraphIndicator(0.8f, 69);
                SetGraphBars(new List<float> { 0.1f, 0.2f, 0.5f, 1.0f, 0.9f, 0.8f, 0.9f, 0.1f, 0.2f, 0.3f, 0.1f, 0.1f });
                SetGraphLabels(new List<string> { "0.00", "20.00" });

                SetScores(new Dictionary<string, string>
                {
                    {"THIS", "0.00"},
                    {"YOU", $"{SaveManager.CurrentSave.GetCurrentLevelInfo()?.FastestTime:F2}"},
                    {"LEADERBOARD", "0.00"},
                    {"IS", "0.00"},
                    {"FAKE", "0.00"}
                });
            }
            else if(displayedTab == Tab.Instructions)
            {
                SetGraphIndicator(0.2f, 5);
                SetGraphBars(new List<float> { 0.05f, 0.1f, 0.1f, 0.2f, 0.3f, 0.6f, 0.9f, 0.95f, 1.0f, 0.3f, 1.0f, 0.1f });
                SetGraphLabels(new List<string> { "0", "20" });

                SetScores(new Dictionary<string, string>
                {
                    {"THIS", "0"},
                    {"LEADERBOARD", "0"},
                    {"IS", "0"},
                    {"YOU", $"{SaveManager.CurrentSave.GetCurrentLevelInfo()?.LowestInstructions}"},
                    {"FAKE", "0"}
                });
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

                ShowTab(0);
            }
        }
    }
}