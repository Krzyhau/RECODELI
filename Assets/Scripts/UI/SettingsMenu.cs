using RecoDeli.Scripts.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private UIDocument settingsDocument;

        private VisualElement tabsContainer;
        private VisualElement viewsContainer;

        private Button exitButton;

        public bool Opened => settingsDocument.rootVisualElement.enabledSelf;

        private void Awake()
        {
            exitButton = settingsDocument.rootVisualElement.Q<Button>("exit-button");

            exitButton.clicked += () => SetEnabled(false);

            InitializeTabAndViews();

            InitializeGeneralSettings();
            InitializeGraphicsSettings();
            InitializeAudioSettings();
            InitializeControlsSettings();

            SetEnabled(false);
        }

        private void InitializeTabAndViews() 
        {
            tabsContainer = settingsDocument.rootVisualElement.Q<VisualElement>("settings-tabs");
            viewsContainer = settingsDocument.rootVisualElement.Q<VisualElement>("settings-views");

            for(int i = 0; i < tabsContainer.childCount; i++)
            {
                var childIndex = i;
                var child = tabsContainer.ElementAt(childIndex);
                if(!(child is Button button)) continue;
                button.clicked += () => SwitchToTab(childIndex);
            }

            SwitchToTab(0);
        }

        private void SwitchToTab(int index)
        {
            for (int i = 0; i < tabsContainer.childCount; i++)
            {
                tabsContainer.ElementAt(i).SetEnabled(i != index);
            }

            for (int i = 0; i < viewsContainer.childCount; i++)
            {
                viewsContainer.ElementAt(i).SetEnabled(i == index);
            }
        }
        private void InitializeGeneralSettings()
        {

        }

        private void InitializeGraphicsSettings()
        {

        }

        private void InitializeAudioSettings()
        {
            var masterVolumeSlider = settingsDocument.rootVisualElement.Q<Slider>("master-volume-slider");
            var musicVolumeSlider = settingsDocument.rootVisualElement.Q<Slider>("music-volume-slider");
            var environmentVolumeSlider = settingsDocument.rootVisualElement.Q<Slider>("environment-volume-slider");
            var interfaceVolumeSlider = settingsDocument.rootVisualElement.Q<Slider>("interface-volume-slider");

            masterVolumeSlider.value = SettingsProvider.MasterVolume;
            musicVolumeSlider.value = SettingsProvider.MusicVolume;
            environmentVolumeSlider.value = SettingsProvider.EnvironmentVolume;
            interfaceVolumeSlider.value = SettingsProvider.InterfaceVolume;

            masterVolumeSlider.RegisterValueChangedCallback(e => SettingsProvider.MasterVolume = e.newValue);
            musicVolumeSlider.RegisterValueChangedCallback(e => SettingsProvider.MusicVolume = e.newValue);
            environmentVolumeSlider.RegisterValueChangedCallback(e => SettingsProvider.EnvironmentVolume = e.newValue);
            interfaceVolumeSlider.RegisterValueChangedCallback(e => SettingsProvider.InterfaceVolume = e.newValue);
        }

        private void InitializeControlsSettings()
        {

        }

        public void SetEnabled(bool state)
        {
            settingsDocument.rootVisualElement.SetEnabled(state);
        }
    }
}
