using RecoDeli.Scripts.Controllers;
using RecoDeli.Scripts.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts
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

        public float InstructionEditorWidth => CalculateInstructionEditorWidth();

        public UIDocument Document => interfaceDocument;
        public InstructionEditor InstructionEditor => instructionEditor;
        public TimescaleBar TimescaleBar => timescaleBar;
        public SimulationManager SimulationManager => simulationManager;


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

            saveButton.clicked += () => saveManagementWindow.SetOpened(true);
            settingsButton.clicked += () => settingsMenu.SetOpened(true);
            // menuButton.clicked += 
        }

        private void Update()
        {
            UpdateTimer();
            UpdateInstructions();
            UpdateGameInterfaceClasses();

            saveButton.SetEnabled(!simulationManager.PlayingSimulation);
            settingsButton.SetEnabled(!simulationManager.PlayingSimulation);

            Document.rootVisualElement.SetEnabled(!ModalWindow.AnyOpened);
        }

        private void UpdateTimer()
        {
            var monospaceSize = 0.6f;
            var time = simulationManager.SimulationTime;
            var timeString = time.ToString(timerFormat);
            var timeSegments = timeString.Split('.');

            var milliseconds = timeSegments.Length > 1 ? timeSegments[1] : "0";

            var timerString = $"<mspace={monospaceSize}em>{timeSegments[0]}</mspace>.<mspace={monospaceSize}em>{milliseconds}</mspace>";
            timeLabel.text = timerString;
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
            interfaceDocument.rootVisualElement.SetEnabled(!show);
        }
    }
}
