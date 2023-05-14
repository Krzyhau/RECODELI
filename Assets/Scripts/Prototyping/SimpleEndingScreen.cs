using RecoDeli.Scripts.Controllers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RecoDeli.Scripts.Prototyping
{
    public class SimpleEndingScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text valuesText;
        [SerializeField] private SimulationManager simulationManager;
        private void OnEnable()
        {
            var simulationTime = simulationManager.SimulationTime;
            var instructionCount = simulationManager.RobotController.CurrentInstructions.Count;
            var rotationMethod = "LUDUM DARE";
            if (RotationMethodSelector.ShouldUseFreeMethod) rotationMethod = "FREE";
            if (RotationMethodSelector.ShouldUseDragMethod) rotationMethod = "ANGULAR DRAG";
            valuesText.text = $"{simulationTime:0.000}\n{instructionCount}\n{rotationMethod}";
        }
    }
}
