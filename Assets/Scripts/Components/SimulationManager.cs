using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace LudumDare.Scripts.Components
{
    public class SimulationManager : MonoBehaviour
    {
        [SerializeField] private Transform simulationGroup;
        [SerializeField] private RobotTrailRecorder trailRecorder;
        [SerializeField] private Instructions instructionsHud;
        [SerializeField] private Scrollbar timescaleScrollbar;
        [SerializeField] private float maximumTimescale;
        [Header("Glitching")]
        [SerializeField] private Material glitchingMaterial;
        [SerializeField] private float glitchingFadeoutSpeed;
        [SerializeField] private float glitchingForce;

        private bool playingSimulation = false;
        private Transform simulationInstance;
        private string simulationGroupName;

        private float currentGlitchingForce = 0.0f;
        private int lastInstruction;

        public RobotController RobotController { get; private set; }

        private void Awake()
        {
            simulationGroupName = simulationGroup.name;
            simulationGroup.name += " (Original)";
            simulationGroup.gameObject.SetActive(false);

            timescaleScrollbar.value = 1.0f / Mathf.Max(1.0f,maximumTimescale);

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
                if(lastInstruction < RobotController.CurrentCommandIndex)
                {
                    instructionsHud.HighlightInstruction(RobotController.CurrentCommandIndex);
                    lastInstruction = RobotController.CurrentCommandIndex;
                }
            }
            ChangeTimeScale();
            UpdateGlitching();
        }

        private void UpdateGlitching()
        {
            if (currentGlitchingForce == 0.0f) return;
            currentGlitchingForce = Mathf.Max(0.0f, currentGlitchingForce - glitchingFadeoutSpeed * Time.unscaledDeltaTime);
            glitchingMaterial.SetFloat("_Intensity", currentGlitchingForce);
        }

        public void PlayInstructions()
        {
            RobotController.ExecuteCommands(instructionsHud.GetRobotActionList());
            trailRecorder.StartRecording(RobotController);
            playingSimulation = true;
            lastInstruction = -1;
        }

        public void RestartSimulation()
        {
            if (!playingSimulation && simulationInstance != null) return;

            if(simulationInstance != null)
            {
                Destroy(simulationInstance.gameObject);
            }

            simulationInstance = Instantiate(simulationGroup);
            simulationInstance.gameObject.SetActive(true);
            simulationInstance.name = simulationGroupName + " (Instance)";
            playingSimulation = false;
            instructionsHud.HighlightInstruction(-1);
            currentGlitchingForce = glitchingForce;

            RobotController = simulationInstance.GetComponentInChildren<RobotController>();
            Assert.IsNotNull(RobotController, "No Robot Controller in simulation group!!!");
        }

        public void ChangeTimeScale()
        {
            Time.timeScale = timescaleScrollbar.value * maximumTimescale;
        }
    }
}
