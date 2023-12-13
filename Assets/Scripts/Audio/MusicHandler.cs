using RecoDeli.Scripts.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecoDeli.Scripts.Audio
{
    public class MusicHandler : MonoBehaviour
    {
        [Header("Handler Settings")]
        [SerializeField] private SimulationManager simulationManager;
        [SerializeField] private AudioSource editorTrackPlayer;
        [SerializeField] private AudioSource simulationTrackPlayer;
        [SerializeField] private float endingFadeoutLength;

        private float fader;
        private float quittingFader;

        [Header("Level Settings")]
        [SerializeField] private MusicTrack music;
        public MusicTrack Music
        {
            get => music;
            set => SetTrack(value);
        }


        private void Awake()
        {
            fader = 0.0f;
            quittingFader = 0.0f;

            SetTrack(music);
        }

        private void Update()
        {
            if(music == null)
            {
                editorTrackPlayer.Stop();
                simulationTrackPlayer.Stop();
                return;
            }

            ApplyFade();
            HandleLooping();
        }

        private void ApplyFade()
        {
            float targetFaderValue = (simulationManager.PlayingSimulation && !simulationManager.FinishedSimulation) ? 1.0f : 0.0f;

            if (!music.dynamicFade && fader != targetFaderValue)
            {
                if (fader > targetFaderValue) editorTrackPlayer.timeSamples = 0;
                else simulationTrackPlayer.timeSamples = 0;
                fader = targetFaderValue;
            }else if (music.dynamicFade)
            {
                fader = Mathf.MoveTowards(fader, targetFaderValue, (1.0f / music.fadeTime) * Time.unscaledDeltaTime);
            }

            if (simulationManager.EndingController.Quitting) quittingFader += Time.unscaledDeltaTime;
            var musicVolumeScale = Mathf.Clamp(1.0f - quittingFader / endingFadeoutLength, 0.0f, 1.0f);
            editorTrackPlayer.volume = Mathf.Lerp(0.0f, music.editorTrackVolume, 1.0f - fader) * musicVolumeScale;
            simulationTrackPlayer.volume = Mathf.Lerp(0.0f, music.simulationTrackVolume, fader) * musicVolumeScale;
        }

        private void HandleLooping()
        {
            if (music.loopEnd == 0) return;

            if (editorTrackPlayer.timeSamples > music.loopEnd * music.editorTrack.frequency)
            {
                editorTrackPlayer.timeSamples = (int)(music.loopStart * music.editorTrack.frequency);
                simulationTrackPlayer.timeSamples = (int)(music.loopStart * music.simulationTrack.frequency);
            }
        }

        private void SetTrack(MusicTrack track)
        {
            music = track;
            editorTrackPlayer.clip = music.editorTrack;
            editorTrackPlayer.volume = music.editorTrackVolume;
            simulationTrackPlayer.clip = music.simulationTrack;
            simulationTrackPlayer.volume = music.simulationTrackVolume;

            editorTrackPlayer.Play();
            simulationTrackPlayer.Play();
        }
    }
}
