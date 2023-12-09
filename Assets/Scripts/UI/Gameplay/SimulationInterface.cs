using RecoDeli.Scripts.Controllers;
using RecoDeli.Scripts.Settings;
using RecoDeli.Scripts.UI;
using RecoDeli.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class SimulationInterface : MonoBehaviour
    {
        [SerializeField] private UIDocument interfaceDocument;
        [SerializeField] private SimulationManager simulationManager;
        [SerializeField] private InstructionEditor instructionEditor;
        [SerializeField] private TimescaleBar timescaleBar;
        [SerializeField] private EndingInterface endingInterface;
        [SerializeField] private SaveManagementWindow saveManagementWindow;
        [SerializeField] private SettingsMenu settingsMenu;

        [Header("Settings")]
        [SerializeField] private string timerFormat = "0.000";

        [Header("Buttons")]
        [SerializeField] private InputActionReference playInput;
        [SerializeField] private InputActionReference restartInput;

        private VisualElement instructionEditorContainer;

        private Button playButton;
        private Button pauseButton;
        private Button restartButton;
        private Label timeLabel;
        private Label instructionsLabel;

        private Button focusOnDroneButton;
        private Button focusOnGoalButton;

        private Button saveButton;
        private Button settingsButton;
        private Button exitButton;

        private float cachedInstructionEditorWidth;
        private bool quitting;

        public float InstructionEditorWidth => CalculateInstructionEditorWidth();

        public UIDocument Document => interfaceDocument;
        public InstructionEditor InstructionEditor => instructionEditor;
        public TimescaleBar TimescaleBar => timescaleBar;
        public SimulationManager SimulationManager => simulationManager;
        public EndingInterface EndingInterface => endingInterface;


        private void OnEnable()
        {
            // for clarity in editor, some UI elements could be disabled
            // so reenable them here
            instructionEditor.gameObject.SetActive(true);
            endingInterface.gameObject.SetActive(true);
            saveManagementWindow.gameObject.SetActive(true);
            settingsMenu.gameObject.SetActive(true);

            instructionEditorContainer = Document.rootVisualElement.Q<VisualElement>("instruction-editor-window");

            playButton = Document.rootVisualElement.Q<Button>("play-button");
            pauseButton = Document.rootVisualElement.Q<Button>("pause-button");
            restartButton = Document.rootVisualElement.Q<Button>("restart-button");
            timeLabel = Document.rootVisualElement.Q<Label>("time-text");
            instructionsLabel = Document.rootVisualElement.Q<Label>("instructions-text");

            focusOnDroneButton = Document.rootVisualElement.Q<Button>("focus-on-drone-button");
            focusOnGoalButton = Document.rootVisualElement.Q<Button>("focus-on-goal-button");

            saveButton = Document.rootVisualElement.Q<Button>("save-button");
            settingsButton = Document.rootVisualElement.Q<Button>("settings-button");
            exitButton = Document.rootVisualElement.Q<Button>("exit-button");

            timescaleBar.Initialize(interfaceDocument);
            instructionEditor.Initialize(interfaceDocument);

            SetupPlaybackButtons();
            SetupMiscellaneousButtons();

            ShowEndingInterface(false);

            InitializeInputs();
        }

        private void SetupPlaybackButtons()
        {
            playButton.clicked += simulationManager.ResumeSimulation;
            pauseButton.clicked += simulationManager.PauseSimulation;
            restartButton.clicked += simulationManager.RestartSimulation;
        }

        private void SetupMiscellaneousButtons()
        {
            focusOnDroneButton.clicked += simulationManager.DroneCamera.FollowRobot;
            focusOnGoalButton.clicked += simulationManager.DroneCamera.FollowPackage;

            saveButton.clicked += () => saveManagementWindow.Open();
            settingsButton.clicked += () => settingsMenu.Open();
            exitButton.clicked += () => StartQuittingGame();
        }

        private void InitializeInputs()
        {
            playInput.action.performed += ctx => PressPlayButton();
            restartInput.action.performed += ctx => PressRestartButton();
        }

        private void PressPlayButton()
        {
            if (simulationManager.PausedSimulation || !simulationManager.PlayingSimulation)
            {
                playButton.Click();
            }
            else
            {
                pauseButton.Click();
            }
        }

        private void PressRestartButton()
        {
            restartButton.Click();
        }

        private void Update()
        {
            UpdateTimer();
            UpdateInstructions();
            UpdateGameInterfaceClasses();

            saveButton.SetEnabled(!simulationManager.PlayingSimulation);
            settingsButton.SetEnabled(!simulationManager.PlayingSimulation);

            Document.rootVisualElement.SetEnabled(
                !ModalWindow.AnyOpened && !endingInterface.IsInterfaceShown() && !quitting
            );
        }

        private void UpdateTimer()
        {
            var monospaceSize = 0.6f;
            var time = GetDisplaySimulationTime();
            var timeString = time.ToString(timerFormat);
            var timeSegments = timeString.Split('.');

            var milliseconds = timeSegments.Length > 1 ? timeSegments[1] : "0";

            var timerString = $"<mspace={monospaceSize}em>{timeSegments[0]}</mspace>.<mspace={monospaceSize}em>{milliseconds}</mspace>";
            timeLabel.text = timerString;
        }

        private float GetDisplaySimulationTime()
        {
            // doing this achieves two things: time is interpolated between smaller
            // values when playback timescale is low, and it displays nice round
            // numbers during playback that actually matches with the instruction set
            var timeStep = (float)simulationManager.PhysicsSimulationInstance.TimeStep;
            var interpTime = simulationManager.PhysicsSimulationInstance.InterpolationTime;
            var displayTime = simulationManager.SimulationTime + (interpTime - 1.0f) * timeStep;

            return Mathf.Clamp(displayTime, 0.0f, simulationManager.FinishedSimulation ? simulationManager.LastCompletionTime : float.MaxValue);
        }

        private void UpdateInstructions()
        {
            // TODO: when instruction editor is refactored, change placeholder to actual instructon count
            instructionsLabel.text = "0";
        }

        private void UpdateGameInterfaceClasses()
        {
            var rootElement = interfaceDocument.rootVisualElement;

            rootElement.EnableInClassList("playing", simulationManager.PlayingSimulation);
            rootElement.EnableInClassList("paused", simulationManager.PausedSimulation);
            rootElement.EnableInClassList("finished", simulationManager.FinishedSimulation);

            playButton.SetEnabled((!simulationManager.PlayingSimulation || simulationManager.PausedSimulation) && !simulationManager.FinishedSimulation);
            pauseButton.SetEnabled(simulationManager.PlayingSimulation && !simulationManager.PausedSimulation && !simulationManager.FinishedSimulation);
            restartButton.SetEnabled(simulationManager.PlayingSimulation && !simulationManager.FinishedSimulation);
        }

        private float CalculateInstructionEditorWidth()
        {
            if (instructionEditorContainer == null || instructionEditorContainer.worldBound.width == 0.0f)
            {
                return cachedInstructionEditorWidth;
            }
            float result = instructionEditorContainer.worldBound.width;
            if (float.IsNaN(result))
            {
                return cachedInstructionEditorWidth;
            }
            var scale = RuntimePanelUtils.ScreenToPanel(instructionEditorContainer.panel, Vector2.up).y;
            cachedInstructionEditorWidth = result / scale;
            return cachedInstructionEditorWidth;
        }

        public void ShowEndingInterface(bool show)
        {
            endingInterface.ShowInterface(show);
        }

        public void StartQuittingGame()
        {
            quitting = true;

            bool runningSimulation = simulationManager.PlayingSimulation;
            interfaceDocument.rootVisualElement.EnableInClassList("quitting", !runningSimulation);
            interfaceDocument.rootVisualElement.EnableInClassList("quitting-long", runningSimulation);
            simulationManager.RestartSimulation();
            simulationManager.EndingController.FinalizeEnding(!runningSimulation);
        }
    }
}
