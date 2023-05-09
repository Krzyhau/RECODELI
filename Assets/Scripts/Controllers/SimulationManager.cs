using RecoDeli.Scripts.Gameplay;
using RecoDeli.Scripts.Gameplay.Robot;
using RecoDeli.Scripts.UI;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RecoDeli.Scripts.Controllers
{
    public class SimulationManager : MonoBehaviour
    {
        [SerializeField] private Transform simulationGroup;
        [SerializeField] private RobotTrailRecorder trailRecorder;
        [SerializeField] private EndingController endingController;
        [SerializeField] private InstructionEditor instructionEditor;
        [SerializeField] private TimescaleBar timescaleBar;
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
        private Transform simulationInstance;
        private string simulationGroupName;

        private float currentGlitchingForce = 0.0f;
        private int lastInstruction;

        private float simulationTime;

        public RobotController RobotController { get; private set; }
        public bool PlayingSimulation => playingSimulation;
        public bool PausedSimulation => paused;
        public float SimulationTime => simulationTime;

        private void Awake()
        {
            simulationGroupName = simulationGroup.name;
            simulationGroup.name += " (Original)";
            simulationGroup.gameObject.SetActive(false);

            RestartSimulation();
        }

        private void OnDisable()
        {
            currentGlitchingForce = 0.0f;
            UpdateGlitching();
        }

        private void Update()
        {
            if (playingSimulation)
            {
                if(lastInstruction < RobotController.CurrentInstructionIndex)
                {
                    Debug.Log(RobotController.CurrentInstructionIndex);
                    instructionEditor.HighlightInstruction(RobotController.CurrentInstructionIndex);
                    lastInstruction = RobotController.CurrentInstructionIndex;
                }
            }

            if (!paused)
            {
                Time.timeScale = timescaleBar.Timescale;
            }

            UpdateGlitching();
        }

        private void FixedUpdate()
        {
            if (playingSimulation)
            {
                simulationTime += Time.fixedDeltaTime;
            }

            if (playingSimulation && !endingController.EndingInProgress && RobotController.ReachedGoalBox != null)
            {
                SimulationSuccessful();
            }
        }

        private void SimulationSuccessful()
        {
            endingController.StartEnding(RobotController, RobotController.ReachedGoalBox);

            Time.timeScale = 1.0f;

            playAmbient.gameObject.SetActive(false);
            idleAmbient.gameObject.SetActive(true);
            successSound.Play();
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
            simulationTime = 0.0f;
            instructionEditor.HighlightInstruction(0);
            instructionEditor.SetPlaybackState(true);
            RobotController.ExecuteCommands(instructionEditor.GetRobotInstructionsList());
            trailRecorder.StartRecording(RobotController);
            playingSimulation = true;
            lastInstruction = -1;

            playAmbient.gameObject.SetActive(true);
            idleAmbient.gameObject.SetActive(false);
        }

        public void RestartSimulation()
        {
            if (!playingSimulation && simulationInstance != null) return;
            if (endingController.EndingInProgress)
            {
                endingController.RevertEnding();
            }

            if(simulationInstance != null)
            {
                Destroy(simulationInstance.gameObject);
            }

            simulationInstance = Instantiate(simulationGroup);
            simulationInstance.gameObject.SetActive(true);
            simulationInstance.name = simulationGroupName + " (Instance)";
            playingSimulation = false;
            instructionEditor.SetPlaybackState(false);
            instructionEditor.HighlightInstruction(-1);
            currentGlitchingForce = glitchingForce;

            restartSound.Play();

            playAmbient.gameObject.SetActive(false);
            idleAmbient.gameObject.SetActive(true);

            RobotController = simulationInstance.GetComponentInChildren<RobotController>();
            Assert.IsNotNull(RobotController, "No Robot Controller in simulation group!!!");
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
