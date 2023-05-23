using BEPUphysics.Unity;
using RecoDeli.Scripts.Gameplay;
using RecoDeli.Scripts.Gameplay.Robot;
using RecoDeli.Scripts.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RecoDeli.Scripts.Controllers
{
    public class SimulationManager : MonoBehaviour
    {
        [SerializeField] private DroneCameraController droneCamera;
        [SerializeField] private BepuSimulation simulationGroup;
        [SerializeField] private UserInterface userInterface;
        [SerializeField] private RobotTrailRecorder trailRecorder;
        [SerializeField] private EndingController endingController;
        [SerializeField] private string levelSelectScene;
        [Header("Glitching")]
        [SerializeField] private Material glitchingMaterial;
        [SerializeField] private float glitchingFadeoutSpeed;
        [SerializeField] private float glitchingForce;
        [Header("SFX")]
        [SerializeField] private AudioSource idleAmbient;
        [SerializeField] private AudioSource playAmbient;
        [SerializeField] private AudioSource successSound;
        [SerializeField] private AudioSource restartSound;

        private bool paused = false;
        private bool playingSimulation = false;
        private bool finishedSimulation = false;
        private BepuSimulation simulationInstance;
        private string simulationGroupName;

        private float currentGlitchingForce = 0.0f;
        private int lastInstruction;

        public RobotController RobotController { get; private set; }
        public GoalBox GoalBox { get; private set; }
        public float LastCompletionTime { get; private set; }
        public UserInterface UserInterface => userInterface;
        public DroneCameraController DroneCamera => droneCamera;
        public BepuSimulation PhysicsSimulationInstance => simulationInstance;
        public EndingController EndingController => endingController;
        public bool PlayingSimulation => playingSimulation;
        public bool PausedSimulation => paused;
        public float SimulationTime => (float)simulationInstance.SimulationTime;
        

        private void Awake()
        {
            simulationGroupName = simulationGroup.name;
            simulationGroup.name += " (Original)";
            simulationGroup.gameObject.SetActive(false);

            RestartSimulation();
        }
        private void OnEnable()
        {

        }

        private void OnDisable()
        {
            currentGlitchingForce = 0.0f;
            UpdateGlitching();
        }

        private void Update()
        {
            if (!paused && playingSimulation && !finishedSimulation)
            {
                Time.timeScale = userInterface.TimescaleBar.Timescale;
            }
            else
            {
                Time.timeScale = paused ? 0.0f : 1.0f;
            }

            UpdateGlitching();
        }

        private void BepuUpdate()
        {
            if (!playingSimulation) return;

            if (lastInstruction < RobotController.CurrentInstructionIndex)
            {
                userInterface.InstructionEditor.HighlightInstruction(RobotController.CurrentInstructionIndex);
                lastInstruction = RobotController.CurrentInstructionIndex;
            }
            if (!endingController.EndingInProgress && RobotController.ReachedGoalBox != null)
            {
                SimulationSuccessful();
            }
        }

        private void SimulationSuccessful()
        {
            LastCompletionTime = SimulationTime;
            endingController.StartEnding(RobotController, RobotController.ReachedGoalBox);

            paused = false;
            Time.timeScale = 1.0f;

            playAmbient.mute = true;
            idleAmbient.mute = false;
            successSound.Play();
            finishedSimulation = true;
        }

        private void UpdateGlitching()
        {
            if (currentGlitchingForce == 0.0f) return;
            var subtract = Mathf.Min(glitchingFadeoutSpeed * Time.unscaledDeltaTime, Time.timeSinceLevelLoad);
            currentGlitchingForce = Mathf.Max(0.0f, currentGlitchingForce - subtract);
            glitchingMaterial.SetFloat("_Intensity", currentGlitchingForce);
        }

        public void PlayInstructions()
        {
            simulationInstance.Active = true;
            userInterface.InstructionEditor.HighlightInstruction(0);
            userInterface.InstructionEditor.SetPlaybackState(true);
            var instructionSet = userInterface.InstructionEditor.GetRobotInstructionsList();
            RobotController.ExecuteCommands(instructionSet);
            trailRecorder.StartRecording(RobotController);
            playingSimulation = true;
            lastInstruction = -1;

            playAmbient.mute = false;
            idleAmbient.mute = true;
        }

        public void RestartSimulation()
        {
            if (!playingSimulation && simulationInstance != null) return;
            if (endingController.EndingInProgress)
            {
                endingController.RevertEnding();
            }

            playingSimulation = false;
            finishedSimulation = false;
            paused = false;

            userInterface.InstructionEditor.SetPlaybackState(false);
            userInterface.InstructionEditor.HighlightInstruction(-1);
            currentGlitchingForce = glitchingForce;

            restartSound.Play();

            playAmbient.mute = true;
            idleAmbient.mute = false;

            if (simulationInstance != null)
            {
                Destroy(simulationInstance.gameObject);
            }

            simulationInstance = Instantiate(simulationGroup);
            simulationInstance.transform.name = simulationGroupName + " (Instance)";
            simulationInstance.gameObject.SetActive(true);
            simulationInstance.Initialize();
            simulationInstance.OnPostPhysicsUpdate += BepuUpdate;

            RobotController = simulationInstance.GetComponentInChildren<RobotController>();
            Assert.IsNotNull(RobotController, "No Robot Controller in simulation group!!!");
            GoalBox = simulationInstance.GetComponentsInChildren<GoalBox>().Where(box => box.IsFinalGoalBox).FirstOrDefault();
        }

        public void PauseSimulation()
        {
            paused = true;
            Time.timeScale = 0.0f;
        }

        public void ResumeSimulation()
        {
            paused = false;
        }

        public void LeaveToLevelSelect()
        {
            SceneManager.LoadScene(levelSelectScene);
        }
    }
}
