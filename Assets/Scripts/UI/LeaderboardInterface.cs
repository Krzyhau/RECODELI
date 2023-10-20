using RecoDeli.Scripts.Leaderboards;
using RecoDeli.Scripts.Level;
using RecoDeli.Scripts.SaveManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class LeaderboardInterface : VisualElement
    {
        public enum ShownDataType
        {
            Time,
            InstructionCount,
        }

        public new class UxmlFactory : UxmlFactory<LeaderboardInterface, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private VisualElement barsContainer;
        private VisualElement graphIndicator;
        private Label graphIndicatorLabel;
        private VisualElement graphLabels;
        private ScrollView scores;
        private Label statusLabel;

        private bool friendsOnly;
        private ShownDataType shownData;

        private LeaderboardProvider provider;
        private string levelName;
        private bool triedLoading;
        private bool hasLoaded;
        private List<LeaderboardRecord> loadedRecords;

        public LeaderboardInterface()
        {
            triedLoading = false;
            hasLoaded = false;

            Construct();
        }

        public void Initialize(string levelName, LeaderboardProvider provider)
        {
            this.levelName = levelName;
            this.provider = provider;

            RequestAndRefreshDataDisplay();
        }

        public void Construct()
        {
            this.name = "leaderboards";
            this.AddToClassList("leaderboards");

            var chartContainer = new VisualElement();
            chartContainer.name = "leaderboards-chart";
            this.Add(chartContainer);

            var chartContainerMain = new VisualElement();
            chartContainerMain.name = "leaderboards-chart-main";
            chartContainer.Add(chartContainerMain);

            graphIndicator = new VisualElement();
            graphIndicator.name = "leaderboards-chart-score";
            graphIndicator.AddToClassList("top50");
            chartContainerMain.Add(graphIndicator);

            graphIndicatorLabel = new Label("UNDEFINED");
            graphIndicatorLabel.name = "leaderboards-chart-score-label";
            graphIndicator.Add(graphIndicatorLabel);

            var chartContainerScoreBar = new VisualElement();
            chartContainerScoreBar.name = "leaderboards-chart-line";
            graphIndicator.Add(chartContainerScoreBar);

            barsContainer = new VisualElement();
            barsContainer.name = "leaderboards-chart-bars";
            chartContainerMain.Add(barsContainer);

            graphLabels = new VisualElement();
            graphLabels.name = "leaderboards-chart-labels";
            chartContainer.Add(graphLabels);

            scores = new ScrollView();
            scores.name = "leaderboards-scores";
            this.Add(scores);

            statusLabel = new Label("LOADING...");
            statusLabel.name = "leaderboards-status";
            this.Add(statusLabel);
        }

        public void SetFriendsOnly(bool state)
        {
            friendsOnly = state;

            RequestAndRefreshDataDisplay();
        }

        public void ShowData(ShownDataType type)
        {
            var oldData = shownData;
            shownData = type;
            if(oldData != shownData)
            {
                RequestAndRefreshDataDisplay();
            }
        }

        private void SetGraphIndicator(float position, int topPercentage)
        {
            if (position >= 1.0f)
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

            foreach (var value in values)
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

            foreach (var score in scoresData)
            {
                var scoreTab = new VisualElement() { name = "score" };
                scoreTab.Add(new Label(score.Key));
                scoreTab.Add(new Label(score.Value));

                // TODO: make a proper user recognition for highlight
                if (score.Key == "YOU")
                {
                    scoreTab.AddToClassList("own");
                }

                scores.Add(scoreTab);
            }
        }

        private void SetStatusText(string status)
        {
            this.EnableInClassList("loading", status != null && status.Length > 0);
            statusLabel.text = status;
        }

        private void RequestAndRefreshDataDisplay()
        {
            SetStatusText("Loading...");

            if (provider == null)
            {
                return;
            }
            if (hasLoaded && provider.HasScoresCached(levelName))
            {
                RefreshDataDisplay(provider.CachedRecords[levelName]);
            }
            else if(!triedLoading && !hasLoaded)
            {

                this.schedule.Execute(() =>
                {
                    if (loadedRecords == null) return;
                    RefreshDataDisplay(loadedRecords);
                }).Until(() => loadedRecords != null || hasLoaded);

                triedLoading = true;
                provider.RequestScores(
                    levelName,
                    list =>
                    {
                        hasLoaded = true;
                        loadedRecords = list;
                    },
                    ex =>
                    {
                        hasLoaded = true;
                        Debug.LogError(ex.Message);
                        SetStatusText("Cannot fetch scores");
                    }
                );
            }
            else
            {
                return;
            }
        }

        private void RefreshDataDisplay(List<LeaderboardRecord> records)
        {
            SetStatusText(null);

            this.AddToClassList("leaderboards");

            //var minimumValue = 0.0f;
            //var maximumValue = 10.0f;
            //var bar = new List<float>();
            //var graphLabel = new List<string>();
            //var scores = new Dictionary<string, string>();

            if (shownData == ShownDataType.Time)
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
            else if (shownData == ShownDataType.InstructionCount)
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

    }
}
