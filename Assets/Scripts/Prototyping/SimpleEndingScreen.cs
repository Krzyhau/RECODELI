using RecoDeli.Scripts.Controllers;
using RecoDeli.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RecoDeli.Scripts.Prototyping
{
    public class SimpleEndingScreen : MonoBehaviour
    {
        [SerializeField] private UserInterface userInterface;
        [SerializeField] private TMP_Text valuesText;
        [SerializeField] private Button retryButton;


        private void Awake()
        {
            retryButton.onClick.AddListener(RetryAction);
        }

        private void OnEnable()
        {
            var simulationTime = userInterface.SimulationManager.LastCompletionTime;
            var instructionCount = userInterface.SimulationManager.RobotController.CurrentInstructions.Count;
            valuesText.text = $"{simulationTime:0.000}\n{instructionCount}";
        }

        private void RetryAction()
        {
            userInterface.SimulationManager.EndingController.RevertEnding();
            userInterface.SimulationManager.RestartSimulation();
            userInterface.EndingInterface.gameObject.SetActive(false);
        }
    }
}
