using RecoDeli.Scripts.Controllers;
using RecoDeli.Scripts.SaveManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class EndingInterface : MonoBehaviour
    {
        [SerializeField] private UIDocument endingDocument;
        [SerializeField] private EndingController endingController;
        [SerializeField] private StatsDisplayer statsDisplayer;

        private Button restartButton;
        private Button continueButton;

        private bool shown;

        private void Awake()
        {
            restartButton = endingDocument.rootVisualElement.Q<Button>("restart-button");
            continueButton = endingDocument.rootVisualElement.Q<Button>("continue-button");

            restartButton.clicked += endingController.SimulationManager.RestartSimulation;
            continueButton.clicked += endingController.FinalizeEnding;

            statsDisplayer.Initialize(endingDocument);
        }
        
        public void ShowInterface(bool show)
        {
            endingDocument.rootVisualElement.SetEnabled(show);
            endingDocument.rootVisualElement.EnableInClassList("hidden", !show);

            if (show)
            {
                var time = endingController.SimulationManager.SimulationTime;
                var instructions = endingController.SimulationManager.RobotController.CurrentInstructions.Length;
                var levelInfo = SaveManager.CurrentSave.GetCurrentLevelInfo();

                statsDisplayer.SetStats(levelInfo.Completed, time, instructions, levelInfo.ExecutionCount);
                statsDisplayer.SetProvider(endingController.SimulationManager.LeaderboardProvider);
            }

            shown = show;
        }

        public void FinalizeEndingInterface()
        {
            endingDocument.rootVisualElement.SetEnabled(false);
            endingDocument.rootVisualElement.EnableInClassList("ending", true);
        }

        public bool IsInterfaceShown()
        {
            return shown;
        }
    }
}