using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare.Scripts.Components.UI
{
    public class PlaybackButtons : MonoBehaviour
    {
        [SerializeField] private SimulationManager simulationManager;

        [Header("Buttons")]
        [SerializeField] private GameObject playButton;
        [SerializeField] private GameObject pauseButton;
        [SerializeField] private GameObject resumeButton;
        [SerializeField] private GameObject stopButton;

        private void Update() 
        {
            if (simulationManager == null) return;

            bool playState = !simulationManager.PlayingSimulation;
            bool pauseState = simulationManager.PlayingSimulation && !simulationManager.PausedSimulation;
            bool resumeState = simulationManager.PlayingSimulation && simulationManager.PausedSimulation;
            bool stopState = simulationManager.PlayingSimulation;

            if(playButton.activeSelf != playState) playButton.SetActive(playState);
            if(pauseButton.activeSelf != pauseState) pauseButton.SetActive(pauseState);
            if(resumeButton.activeSelf != resumeState) resumeButton.SetActive(resumeState);
            if(stopButton.activeSelf != stopState) stopButton.SetActive(stopState);
        }
    }
}
