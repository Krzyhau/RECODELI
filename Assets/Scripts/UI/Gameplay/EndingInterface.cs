using RecoDeli.Scripts.Controllers;
using RecoDeli.Scripts.Leaderboards;
using RecoDeli.Scripts.Level;
using RecoDeli.Scripts.SaveManagement;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;
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

        private LeaderboardInterface leaderboard;

        private enum Tab { Times, Instructions }
        private Tab displayedTab;
        private bool onlyFriends;

        private void Awake()
        {
            restartButton = endingDocument.rootVisualElement.Q<Button>("restart-button");
            continueButton = endingDocument.rootVisualElement.Q<Button>("continue-button");
            friendsStatsButton = endingDocument.rootVisualElement.Q<Button>("friends-stats-button");
            globalStatsButton = endingDocument.rootVisualElement.Q<Button>("global-stats-button");

            leaderboard = endingDocument.rootVisualElement.Q<LeaderboardInterface>("leaderboards");

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

            leaderboard.ShowData(displayedTab switch
            {
                Tab.Instructions => LeaderboardInterface.ShownDataType.InstructionCount,
                Tab.Times or _ => LeaderboardInterface.ShownDataType.Time
            });
        }

        private void SetFriendsOnly(bool state)
        {
            onlyFriends = state;
            friendsStatsButton.SetEnabled(!state);
            globalStatsButton.SetEnabled(state);
            leaderboard.SetFriendsOnly(state);
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

                leaderboard.Initialize(LevelLoader.CurrentlyLoadedLevel, new TestLeaderboardProvider());
                ShowTab(0);
            }
        }

        public bool IsInterfaceShown()
        {
            return endingDocument.rootVisualElement.enabledSelf;
        }
    }
}