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

        public LeaderboardInterface()
        {
            Construct();
        }

        public void Initialize(LeaderboardProvider provider)
        {
            SetStatusText("Loading...");

            if(this.provider != provider)
            {
                this.provider = provider;
                this.provider.OnLoaded += () => RefreshDataDisplay();

                this.provider.OnFailed += (e) => SetStatusText("Connection failed");
                this.provider.OnSubmitFailed += (e) => SetStatusText("Connection failed");
            }

            RefreshDataDisplay();
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

            RefreshDataDisplay();
        }

        public void ShowData(ShownDataType type)
        {
            var oldData = shownData;
            shownData = type;
            if(oldData != shownData)
            {
                RefreshDataDisplay();
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

        private void RefreshDataDisplay()
        {
            Debug.Log("BBBB");
            if (provider == null || provider.Status != LeaderboardProvider.LoadingStatus.Loaded) return;

            SetStatusText(null);

            this.AddToClassList("leaderboards");

            var data = provider.CachedData;

            var recordsToShow = data.Records.ToDictionary(
                r => r.DisplayName,
                r => shownData switch
                {
                    ShownDataType.Time => r.CompletionTime,
                    ShownDataType.InstructionCount or _ => r.InstructionsCount
                }
            ).OrderBy(r=> r.Value.Place);

            var valueFormat = shownData switch
            {
                ShownDataType.Time => "0.00",
                ShownDataType.InstructionCount or _ => "0",
            };

            var stats = shownData switch
            {
                ShownDataType.Time => data.TimeStats,
                ShownDataType.InstructionCount or _ => data.InstructionsStats,
            };

            SetGraphLabels(new List<string> { 
                stats.BestRecord.ToString(valueFormat),
                //((stats.BestRecord + stats.WorstRecord) / 2).ToString(valueFormat),
                stats.WorstRecord.ToString(valueFormat),
            });

            SetGraphBars(stats.Averages);

            SetScores(recordsToShow.ToDictionary(
                r => $"{r.Value.Place}. {r.Key}",
                r => r.Value.Value.ToString(valueFormat)
            ));

            var userName = SaveManager.CurrentSave.Name;
            var ownRecordQuery = recordsToShow.Where(r => r.Key == userName);
            if(ownRecordQuery.Any())
            {
                var ownRecord = ownRecordQuery.First().Value;
                var position = ((ownRecord.Value - stats.BestRecord) / (stats.WorstRecord - stats.BestRecord));
                var percentage = (ownRecord.Place / (float)stats.RecordCount) * 100.0f;
                SetGraphIndicator(position, Mathf.Max((int)percentage, 1));
            }
        }

    }
}
