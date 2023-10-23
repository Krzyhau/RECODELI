using BEPUphysics.Unity;
using RecoDeli.Scripts.Assets.Scripts.Leaderboards;
using RecoDeli.Scripts.Gameplay;
using RecoDeli.Scripts.Gameplay.Robot;
using RecoDeli.Scripts.Leaderboards;
using RecoDeli.Scripts.Level;
using RecoDeli.Scripts.SaveManagement;
using RecoDeli.Scripts.UI;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace RecoDeli.Scripts.Controllers
{
    public class SimulationManager : MonoBehaviour
    {
        [SerializeField] private DroneCameraController droneCamera;
        [SerializeField] private BepuSimulation simulationGroup;
        [SerializeField] private SimulationInterface simulationInterface;
        [SerializeField] private RobotTrailRecorder trailRecorder;
        [SerializeField] private EndingController endingController;
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
        private bool wasPausedBeforeSkipping = false;
        private bool didAtLeastOneLogicUpdateThisFrame = false;

        private float currentGlitchingForce = 0.0f;
        private int lastInstruction;

        public RobotController RobotController { get; private set; }
        public GoalBox GoalBox { get; private set; }
        public float LastCompletionTime { get; private set; }
        public SimulationInterface Interface => simulationInterface;
        public DroneCameraController DroneCamera => droneCamera;
        public BepuSimulation PhysicsSimulationInstance => simulationInstance;
        public EndingController EndingController => endingController;
        public bool PlayingSimulation => playingSimulation;
        public bool FinishedSimulation => finishedSimulation;
        public bool PausedSimulation => paused;
        public float SimulationTime => simulationInstance != null ? (float)simulationInstance.SimulationTime : 0.00f;
        public LeaderboardProvider LeaderboardProvider { get;private set; }

        public static SimulationManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            LeaderboardProvider = new MyOwnShittyLeaderboardProvider(LevelLoader.CurrentlyLoadedLevel);
            LeaderboardProvider.OnSubmit += LeaderboardProvider.RequestScores;

            if (simulationGroup && simulationGroup.gameObject.activeSelf)
            {
                SetPhysicsSimulation(simulationGroup);
            }
        }
        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            currentGlitchingForce = 0.0f;
            UpdateGlitching();
        }

        private void Start()
        {
            RestartSimulation();
        }

        private void LateUpdate()
        {
            if (!didAtLeastOneLogicUpdateThisFrame)
            {
                LogicUpdate();
            }
            else
            {
                didAtLeastOneLogicUpdateThisFrame = false;
            }
        }

        private void LogicUpdate()
        {
            CheckSkipping();

            if (!paused && playingSimulation && !finishedSimulation)
            {
                Time.timeScale = simulationInterface.TimescaleBar.Timescale;
            }
            else
            {
                Time.timeScale = paused ? 0.0f : 1.0f;
            }

            UpdateGlitching();

            didAtLeastOneLogicUpdateThisFrame = true;
        }

        private void BepuUpdate()
        {
            if (!playingSimulation) return;

            LogicUpdate();

            if (lastInstruction < RobotController.CurrentInstructionIndex)
            {
                simulationInterface.InstructionEditor.HighlightInstruction(RobotController.CurrentInstructionIndex);
                lastInstruction = RobotController.CurrentInstructionIndex;
            }
            if (!endingController.EndingInProgress && RobotController.ReachedGoalBox != null)
            {
                SimulationSuccessful();
            }
        }

        private void SaveCompletionStats()
        {
            var levelInfo = SaveManager.CurrentSave.GetCurrentLevelInfo();
            if (!levelInfo.Completed)
            {
                levelInfo.FastestTime = LastCompletionTime;
                levelInfo.LowestInstructions = RobotController.CurrentInstructions.Length;
                levelInfo.Completed = true;
            }
            else
            {
                levelInfo.FastestTime = Mathf.Min(levelInfo.FastestTime, LastCompletionTime);
                levelInfo.LowestInstructions = Mathf.Min(levelInfo.LowestInstructions, RobotController.CurrentInstructions.Length);
            }
        }

        private void SimulationSuccessful()
        {
            LastCompletionTime = SimulationTime;

            SaveCompletionStats();

            LeaderboardProvider.Reset();
            LeaderboardProvider.SubmitScore(SimulationTime, RobotController.CurrentInstructions);

            endingController.StartEnding();

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

        private void CheckSkipping()
        {
            if (finishedSimulation)
            {
                wasPausedBeforeSkipping = false;
                return;
            }

            var currentIndex = RobotController.CurrentInstructionIndex;
            var skipToIndex = simulationInterface.InstructionEditor.SkipToInstruction;
            var skipping = currentIndex >= 0 && currentIndex < skipToIndex;

            simulationInterface.TimescaleBar.Skipping = skipping;
            if (skipping && paused)
            {
                ResumeSimulation();
                wasPausedBeforeSkipping = true;
            }
            if(!skipping && wasPausedBeforeSkipping)
            {
                PauseSimulation();
                simulationInstance.ResetStepTimer();
                wasPausedBeforeSkipping = false;
            }
        }

        public void PlayInstructions()
        {
            Assert.IsNotNull(RobotController, "Cannot play instructions without a robot within a simulation!");

            simulationInstance.Active = true;
            simulationInterface.InstructionEditor.SetPlaybackState(true);
            simulationInterface.InstructionEditor.HighlightInstruction(0);
            var instructionSet = simulationInterface.InstructionEditor.GetRobotInstructionsList();
            RobotController.ExecuteCommands(instructionSet.ToArray());
            trailRecorder.StartRecording(RobotController);
            playingSimulation = true;
            lastInstruction = -1;

            playAmbient.mute = false;
            idleAmbient.mute = true;

            SaveManager.CurrentSave.GetCurrentLevelInfo().ExecutionCount++;
        }

        public void RestartSimulation()
        {
            if(simulationGroup == null)
            {
                Debug.LogWarning("Simulation manager has no physics simulation linked");
            }
            if (!playingSimulation && simulationInstance != null)
            {
                // simulation has already been reset
                return;
            }
            if (endingController.EndingInProgress)
            {
                endingController.RevertEnding();
            }

            playingSimulation = false;
            finishedSimulation = false;
            paused = false;
            wasPausedBeforeSkipping = false;

            simulationInterface.InstructionEditor.SetPlaybackState(false);
            simulationInterface.InstructionEditor.HighlightInstruction(-1);

            playAmbient.mute = true;
            idleAmbient.mute = false;

            if (simulationInstance != null)
            {
                Destroy(simulationInstance.gameObject);
                currentGlitchingForce = glitchingForce;
                restartSound.Play();
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
            if (!playingSimulation)
            {
                PlayInstructions();
            }
        }

        public void SetPhysicsSimulation(BepuSimulation simulation)
        {
            simulationGroup = simulation;

            if (Application.isPlaying)
            {
                simulationGroupName = simulationGroup.name;
                simulationGroup.name += " (Original)";
                simulationGroup.gameObject.SetActive(false);

                RestartSimulation();
            }
        }
    }
}
