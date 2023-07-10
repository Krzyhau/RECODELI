using RecoDeli.Scripts.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class UserInterface : MonoBehaviour
    {
        [SerializeField] private SimulationManager simulationManager;

        [Header("Main Windows")]
        [SerializeField] private UIDocument gameplayInterface;
        [SerializeField] private UIDocument menuInterface;
        [SerializeField] private UIDocument endingInterface;

        [Header("Components")]
        [SerializeField] private InstructionEditor instructionEditor;
        [SerializeField] private TimescaleBar timescaleBar;


        public UIDocument GameplayInterface => gameplayInterface;
        public UIDocument MenuInterface => menuInterface;
        public UIDocument EndingInterface => endingInterface;

        public SimulationManager SimulationManager => simulationManager;
        public InstructionEditor InstructionEditor => instructionEditor;
        public TimescaleBar TimescaleBar => timescaleBar;
    }
}
