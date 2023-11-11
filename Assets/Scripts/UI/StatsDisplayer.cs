using UnityEngine.UIElements;
using UnityEngine;
using RecoDeli.Scripts.Leaderboards;

namespace RecoDeli.Scripts.UI
{
    public class StatsDisplayer : MonoBehaviour
    {
        [SerializeField] private string timerFormat = "0.000";

        private VisualElement leaderboardTabs;

        private Button friendsStatsButton;
        private Button globalStatsButton;

        private Label timeLabel;
        private Label instructionsLabel;
        private Label attemptsLabel;

        private LeaderboardInterface leaderboard;

        private enum Tab { Times, Instructions }
        private Tab displayedTab;
        private bool onlyFriends;

        public void Initialize(UIDocument document)
        {
            friendsStatsButton = document.rootVisualElement.Q<Button>("friends-stats-button");
            globalStatsButton = document.rootVisualElement.Q<Button>("global-stats-button");

            leaderboard = document.rootVisualElement.Q<LeaderboardInterface>("leaderboards");

            timeLabel = document.rootVisualElement.Q<Label>("stat-time");
            instructionsLabel = document.rootVisualElement.Q<Label>("stat-instructions");
            attemptsLabel = document.rootVisualElement.Q<Label>("stat-attempts");

            leaderboardTabs = document.rootVisualElement.Q<VisualElement>("leaderboards-tabs");
            InitializeTabs();

            if (friendsStatsButton != null) friendsStatsButton.clicked += () => SetFriendsOnly(true);
            if (globalStatsButton != null) globalStatsButton.clicked += () => SetFriendsOnly(false);
            SetFriendsOnly(true);
        }

        private void InitializeTabs()
        {
            foreach (var tab in leaderboardTabs.Children())
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
            if (friendsStatsButton != null)
            {
                friendsStatsButton.SetEnabled(!state);
            }
            if (globalStatsButton != null)
            {
                globalStatsButton.SetEnabled(state);
            }
            leaderboard.SetFriendsOnly(state);
        }

        public void SetStats(bool completed, float time, int instructions, int attempts)
        {
            if (!completed)
            {
                timeLabel.text = "N/A";
                instructionsLabel.text = "N/A";
            }
            else
            {
                var timeString = time.ToString(timerFormat);
                var timeSegments = timeString.Split('.');
                var milliseconds = timeSegments.Length > 1 ? timeSegments[1] : "0";

                timeLabel.text = $"{timeSegments[0]}.{milliseconds}";
                instructionsLabel.text = $"{instructions}";
            }

            attemptsLabel.text = $"{attempts}";
        }

        public void SetProvider(LeaderboardProvider provider)
        {
            leaderboard.SetProvider(provider);
            ShowTab(0);
        }
    }
}
