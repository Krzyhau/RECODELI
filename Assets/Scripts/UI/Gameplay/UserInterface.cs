using RecoDeli.Scripts.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RecoDeli.Scripts.UI
{
    public class UserInterface : MonoBehaviour
    {
        [SerializeField] private SimulationManager simulationManager;

        [Header("Main Windows")]
        [SerializeField] private CanvasGroup gameplayInterface;
        [SerializeField] private CanvasGroup menuInterface;
        [SerializeField] private CanvasGroup endingInterface;

        [Header("Components")]
        [SerializeField] private InstructionEditor instructionEditor;
        [SerializeField] private TimescaleBar timescaleBar;
        [SerializeField] private Button focusOnRobotButton;
        [SerializeField] private Button focusOnPayloadButton;


        public CanvasGroup GameplayInterface => gameplayInterface;
        public CanvasGroup MenuInterface => menuInterface;
        public CanvasGroup EndingInterface => endingInterface;

        public SimulationManager SimulationManager => simulationManager;
        public InstructionEditor InstructionEditor => instructionEditor;
        public TimescaleBar TimescaleBar => timescaleBar;


        private void Awake()
        {
            focusOnRobotButton.onClick.AddListener(simulationManager.DroneCamera.FollowRobot);
            focusOnPayloadButton.onClick.AddListener(simulationManager.DroneCamera.FollowPackage);
        }
    }
}
