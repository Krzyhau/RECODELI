using RecoDeli.Scripts.Controllers;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class EndingInterface : MonoBehaviour
    {
        [SerializeField] private UIDocument endingDocument;
        [SerializeField] private EndingController endingController;

        private VisualElement leaderboardTabs;
        private VisualElement leaderboardView;

        private Button restartButton;
        private Button continueButton;

        private void Awake()
        {
            restartButton = endingDocument.rootVisualElement.Q<Button>("restart-button");
            continueButton = endingDocument.rootVisualElement.Q<Button>("continue-button");

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
        }
    }
}