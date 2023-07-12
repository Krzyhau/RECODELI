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

        [Header("Settings")]
        [SerializeField] private string timerFormat = "0.000";

        private Slider timescaleSlider;
        private VisualElement instructionEditorContainer;

        private Button playButton;
        private Button pauseButton;
        private Button restartButton;
        private Label timeLabel;
        private Label instructionsLabel;

        private Button focusOnDroneButton;
        private Button focusOnGoalButton;

        private Button menuButton;
        public float InstructionEditorWidth => CalculateInstructionEditorWidth();

        public UIDocument Document => interfaceDocument;
        public InstructionEditor InstructionEditor => instructionEditor;

        public TimescaleBar TimescaleBar => timescaleBar;


        private void OnEnable()
        {
            timescaleSlider = Document.rootVisualElement.Q<Slider>("timescale-slider");
            instructionEditorContainer = Document.rootVisualElement.Q<VisualElement>("instruction-editor-window");

            playButton = Document.rootVisualElement.Q<Button>("play-button");
            pauseButton = Document.rootVisualElement.Q<Button>("pause-button");
            restartButton = Document.rootVisualElement.Q<Button>("restart-button");
            timeLabel = Document.rootVisualElement.Q<Label>("time-text");
            instructionsLabel = Document.rootVisualElement.Q<Label>("instructions-text");

            focusOnDroneButton = Document.rootVisualElement.Q<Button>("focus-on-drone-button");
            focusOnGoalButton = Document.rootVisualElement.Q<Button>("focus-on-goal-button");

            menuButton = Document.rootVisualElement.Q<Button>("menu-button");

            timescaleBar.Initialize(interfaceDocument);
            instructionEditor.Initialize(interfaceDocument);

            SetupPlaybackButtons();
            SetupMiscellaneousButtons();
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

            // menuButton.clicked += 
        }

        private void Update()
        {
            UpdateTimer();
            UpdateInstructions();
            UpdateInterfaceClasses();
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

        private void UpdateInterfaceClasses()
        {
            var rootElement = interfaceDocument.rootVisualElement;

            rootElement.EnableInClassList("playing", simulationManager.PlayingSimulation);
            rootElement.EnableInClassList("paused", simulationManager.PausedSimulation);
            rootElement.EnableInClassList("finished", simulationManager.FinishedSimulation);
        }

        private float CalculateInstructionEditorWidth()
        {
            if (instructionEditorContainer == null) return 0;
            float result = instructionEditorContainer.worldBound.width;
            if (float.IsNaN(result)) return 0.0f;
            var scale = RuntimePanelUtils.ScreenToPanel(instructionEditorContainer.panel, Vector2.up).y;
            return result / scale;
        }
    }
}
