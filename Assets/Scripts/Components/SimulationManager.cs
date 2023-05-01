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

        private bool playingSimulation = false;
        private Transform simulationInstance;
        private string simulationGroupName;


        private int lastInstruction;

        public RobotController RobotController { get; private set; }

        private void Awake()
        {
            simulationGroupName = simulationGroup.name;
            simulationGroup.name += " (Original)";
            simulationGroup.gameObject.SetActive(false);

            RestartSimulation();
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

            RobotController = simulationInstance.GetComponentInChildren<RobotController>();
            Assert.IsNotNull(RobotController, "No Robot Controller in simulation group!!!");
        }

        public void ChangeTimeScale()
        {
            Time.timeScale = timescaleScrollbar.value * 5.0f;
        }
    }
}
