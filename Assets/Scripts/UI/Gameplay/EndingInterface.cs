using RecoDeli.Scripts.Controllers;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class EndingInterface : MonoBehaviour
    {
        [SerializeField] private UIDocument endingDocument;
        [SerializeField] private EndingController endingController;

        private Button restartButton;
        private Button continueButton;

        private void Awake()
        {
            restartButton = endingDocument.rootVisualElement.Q<Button>("restart-button");
            continueButton = endingDocument.rootVisualElement.Q<Button>("continue-button");

            restartButton.clicked += endingController.SimulationManager.RestartSimulation;
            continueButton.clicked += endingController.FinalizeEnding;
        }

        public void ShowInterface(bool show)
        {
            endingDocument.rootVisualElement.SetEnabled(show);
        }
    }
}