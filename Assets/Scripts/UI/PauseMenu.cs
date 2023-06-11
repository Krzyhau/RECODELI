using RecoDeli.Scripts.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RecoDeli.Scripts.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private Scrollbar masterVolumeBar;
        [SerializeField] private Scrollbar musicVolumeBar;
        [SerializeField] private Button goBackButton;

        private void Start()
        {
            goBackButton.onClick.AddListener(() => RecoDeliGame.OpenSimpleLevelList());

            masterVolumeBar.value = SettingsProvider.MasterVolume;
            musicVolumeBar.value = SettingsProvider.MusicVolume;

            masterVolumeBar.onValueChanged.AddListener((f) => OnVolumeBarChanged());
            musicVolumeBar.onValueChanged.AddListener((f) => OnVolumeBarChanged());
        }

        private void OnVolumeBarChanged()
        {
            SettingsProvider.MasterVolume = Mathf.Clamp(masterVolumeBar.value, 0.0001f, 1.0f);
            SettingsProvider.MusicVolume = Mathf.Clamp(musicVolumeBar.value, 0.0001f, 1.0f);
        }
    }
}
