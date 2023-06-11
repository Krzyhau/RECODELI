using UnityEngine;

namespace RecoDeli.Scripts.Settings
{
    public class SettingsProvider
    {
        public static float MasterVolume
        {
            get => PlayerPrefs.GetFloat("SettingsMasterVolume", 1.0f);
            set {
                PlayerPrefs.SetFloat("SettingsMasterVolume", value);
                RecoDeliGame.Settings.MainAudioMixerGroup.audioMixer.SetFloat("Master Volume", Mathf.Log10(value) * 20.0f);
            }
        }
        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat("SettingsMusicVolume", 1.0f);
            set
            {
                PlayerPrefs.SetFloat("SettingsMusicVolume", value);
                RecoDeliGame.Settings.MusicAudioMixerGroup.audioMixer.SetFloat("Music Volume", Mathf.Log10(value) * 20.0f);
            }
        }

        public static void ApplySettings()
        {
            MasterVolume = MasterVolume;
            MusicVolume = MusicVolume;
        }
    }
}
