using RecoDeli.Scripts.Controllers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RecoDeli.Scripts.UI
{
    public class PlaybackButtons : MonoBehaviour
    {
        [SerializeField] private SimulationManager simulationManager;

        [Header("Buttons")]
        [SerializeField] private GameObject playButton;
        [SerializeField] private GameObject pauseButton;
        [SerializeField] private GameObject resumeButton;
        [SerializeField] private GameObject stopButton;
        [Header("Timer")]
        [SerializeField] private GameObject timerContainer;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private string timerFormat = "0.000";

        private void Update() 
        {
            if (simulationManager == null) return;

            bool playState = !simulationManager.PlayingSimulation;
            bool pauseState = simulationManager.PlayingSimulation && !simulationManager.PausedSimulation;
            bool resumeState = simulationManager.PlayingSimulation && simulationManager.PausedSimulation;
            bool stopState = simulationManager.PlayingSimulation;
            bool timerTextState = simulationManager.PlayingSimulation;

            if(playButton.activeSelf != playState) playButton.SetActive(playState);
            if(pauseButton.activeSelf != pauseState) pauseButton.SetActive(pauseState);
            if(resumeButton.activeSelf != resumeState) resumeButton.SetActive(resumeState);
            if(stopButton.activeSelf != stopState) stopButton.SetActive(stopState);
            if(timerContainer.activeSelf != timerTextState) timerContainer.SetActive(timerTextState);

            UpdateTimer();
        }

        private void UpdateTimer()
        {
            var monospaceSize = 0.6f;
            var time = simulationManager.SimulationTime;
            var timeString = time.ToString(timerFormat);
            var timeSegments = timeString.Split('.');

            var timerString = $"<mspace={monospaceSize}em>{timeSegments[0]}</mspace>.<mspace={monospaceSize}em>{timeSegments[1]}</mspace>";
            timerText.text = timerString;
        }
    }
}
