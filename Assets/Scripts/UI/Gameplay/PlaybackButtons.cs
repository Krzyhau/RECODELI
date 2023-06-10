using RecoDeli.Scripts.Controllers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RecoDeli.Scripts.UI
{
    public class PlaybackButtons : MonoBehaviour
    {
        [SerializeField] private UserInterface userInterface;

        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button stopButton;
        [Header("Timer")]
        [SerializeField] private GameObject timerContainer;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private string timerFormat = "0.000";

        private SimulationManager simulationManager;

        private void Awake()
        {
            if (userInterface == null) return;

            simulationManager = userInterface.SimulationManager;
            if(simulationManager == null) return;

            playButton.onClick.AddListener(simulationManager.PlayInstructions);
            pauseButton.onClick.AddListener(simulationManager.PauseSimulation);
            resumeButton.onClick.AddListener(simulationManager.ResumeSimulation);
            stopButton.onClick.AddListener(simulationManager.RestartSimulation);
        }

        private void Update() 
        {
            if (simulationManager == null) return;

            SwitchState(playButton.gameObject, !simulationManager.PlayingSimulation);
            SwitchState(pauseButton.gameObject, simulationManager.PlayingSimulation && !simulationManager.PausedSimulation);
            SwitchState(resumeButton.gameObject, simulationManager.PlayingSimulation && simulationManager.PausedSimulation);
            SwitchState(stopButton.gameObject, simulationManager.PlayingSimulation);
            SwitchState(timerContainer, simulationManager.PlayingSimulation);

            UpdateTimer();
        }

        private void SwitchState(GameObject element, bool state)
        {
            if (element.activeSelf != state) element.SetActive(state);
        }

        private void UpdateTimer()
        {
            var monospaceSize = 0.6f;
            var time = simulationManager.SimulationTime;
            var timeString = time.ToString(timerFormat);
            var timeSegments = timeString.Split('.');

            var milliseconds = timeSegments.Length > 1 ? timeSegments[1] : "0";

            var timerString = $"<mspace={monospaceSize}em>{timeSegments[0]}</mspace>.<mspace={monospaceSize}em>{milliseconds}</mspace>";
            timerText.text = timerString;
        }
    }
}
