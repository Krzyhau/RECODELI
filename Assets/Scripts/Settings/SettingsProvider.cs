using System.Collections;
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
                SetAudioLevels("Master Volume", value);
            }
        }
        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat("SettingsMusicVolume", 1.0f);
            set
            {
                PlayerPrefs.SetFloat("SettingsMusicVolume", value);
                SetAudioLevels("Music Volume", value);
            }
        }

        public static float EnvironmentVolume
        {
            get => PlayerPrefs.GetFloat("SettingsEnvironmentVolume", 1.0f);
            set
            {
                PlayerPrefs.SetFloat("SettingsEnvironmentVolume", value);
                SetAudioLevels("Environment Volume", value);
            }
        }

        public static float InterfaceVolume
        {
            get => PlayerPrefs.GetFloat("SettingsInterfaceVolume", 1.0f);
            set
            {
                PlayerPrefs.SetFloat("SettingsInterfaceVolume", value);
                SetAudioLevels("Interface Volume", value);
            }
        }

        public static string Language
        {
            get => PlayerPrefs.GetString("Language", "en-US");
            set => PlayerPrefs.SetString("Language", value);
        }

        public static void ApplySettings()
        {
            Language = Language;
            ApplyAudioSettings();
            AudioSettings.OnAudioConfigurationChanged += (b) => ApplyAudioSettings();
        }

        private static void ApplyAudioSettings()
        {
            // BUG: audio mixer is reset on awake in editor. no idea how to prevent that lol
            MasterVolume = MasterVolume;
            MusicVolume = MusicVolume;
            EnvironmentVolume = EnvironmentVolume;
            InterfaceVolume = InterfaceVolume;
        }



        private static void SetAudioLevels(string name, float value)
        {
            var volumeValue = value == 0 ? -80.0f : Mathf.Log10(value) * 20.0f;
            RecoDeliGame.Settings.MusicAudioMixerGroup.audioMixer.SetFloat(name, volumeValue);
        }
    }
}
