using RecoDeli.Scripts.Controllers;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class EndingInterface : MonoBehaviour
    {
        [SerializeField] private UIDocument endingDocument;
        [SerializeField] private EndingController endingController;

        private VisualElement leaderboardTabView;

        private Button restartButton;
        private Button continueButton;

        private void Awake()
        {
            restartButton = endingDocument.rootVisualElement.Q<Button>("restart-button");
            continueButton = endingDocument.rootVisualElement.Q<Button>("continue-button");

            leaderboardTabView = endingDocument.rootVisualElement.Q<VisualElement>("leaderboards-tab-view");
            InitializeTabView(leaderboardTabView);

            restartButton.clicked += endingController.SimulationManager.RestartSimulation;
            continueButton.clicked += endingController.FinalizeEnding;
        }

        private void InitializeTabView(VisualElement tabView)
        {
            var tabsContainer = tabView.Q<VisualElement>(className: "tab-view-tabs");
            var viewsContainer = tabView.Q<VisualElement>(className: "tab-view-views");

            for(int i = 0; i < tabsContainer.childCount; i++)
            {
                var tab = tabsContainer.ElementAt(i) as Button;
                var view = viewsContainer.ElementAt(i);
                if (tab == null || view == null) continue;
                var index = i;
                tab.clicked += () => ShowTab(tabView, index); 
            }
            ShowTab(tabView, 0);
        }

        private void ShowTab(VisualElement tabView, int index)
        {
            var tabsContainer = tabView.Q<VisualElement>(className: "tab-view-tabs");
            var viewsContainer = tabView.Q<VisualElement>(className: "tab-view-views");

            for (int i = 0; i < tabsContainer.childCount; i++)
            {
                var tab = tabsContainer.ElementAt(i) as Button;
                var view = viewsContainer.ElementAt(i);
                if (tab == null || view == null) continue;
                tab.SetEnabled(i != index);
                view.SetEnabled(i == index);
            }
        }

        public void ShowInterface(bool show)
        {
            endingDocument.rootVisualElement.SetEnabled(show);
        }
    }
}