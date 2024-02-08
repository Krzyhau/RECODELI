using RecoDeli.Scripts.Assets.Scripts.UI;
using RecoDeli.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RecoDeli.Scripts.Settings
{
    public class UserSettingsProvider
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

        public static bool PostProcessing
        {
            get => PlayerPrefs.GetInt("PostProcessing", 1) > 0;
            set 
            {
                PlayerPrefs.SetInt("PostProcessing", value ? 1 : 0);
                EnablePostProcessing(value);
            }
        }

        public static InterfaceScaleSetter.Scale UIScale
        {
            get => (InterfaceScaleSetter.Scale)PlayerPrefs.GetInt("UIScale", (int)InterfaceScaleSetter.Scale.Medium);
            set
            {
                PlayerPrefs.SetInt("UIScale", (int)value);
                InterfaceScaleSetter.Set(value);
            }
        }

        public static void ApplySettings()
        {
            Language = Language;
            ApplyAudioSettings();
            ApplyVideoSettings();
            System.Threading.SynchronizationContext.Current.Post(_ => ApplyAudioSettings(), null);
        }


        private static void ApplyAudioSettings()
        {
            // BUG: audio mixer is reset on awake in editor. no idea how to prevent that lol
            MasterVolume = MasterVolume;
            MusicVolume = MusicVolume;
            EnvironmentVolume = EnvironmentVolume;
            InterfaceVolume = InterfaceVolume;
        }

        private static void ApplyVideoSettings()
        {
            PostProcessing = PostProcessing;
            UIScale = UIScale;
        }

        private static void SetAudioLevels(string name, float value)
        {
            var volumeValue = value == 0 ? -80.0f : Mathf.Log10(value) * 20.0f;
            RecoDeliGame.Settings.MusicAudioMixerGroup.audioMixer.SetFloat(name, volumeValue);
        }

        private static void EnablePostProcessing(bool status)
        {
            RecoDeliGame.Settings.CRTPassRenderFeature.SetActive(status);

            var volumeProfiles = new List<VolumeProfile>
            {
                RecoDeliGame.Settings.ScreenRenderingVolume,
                RecoDeliGame.Settings.GameVolume
            };

            foreach(var profile in volumeProfiles)
            {
                if(profile.TryGet(out Bloom bloom)) bloom.active = status;
            }
        }
    }
}
