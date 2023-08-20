using RecoDeli.Scripts.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class SettingsMenu : ModalWindow
    {
        private VisualElement tabsContainer;
        private VisualElement viewsContainer;

        protected override void Awake()
        {
            base.Awake();

            InitializeTabAndViews();

            InitializeGeneralSettings();
            InitializeGraphicsSettings();
            InitializeAudioSettings();
            InitializeControlsSettings();
        }

        private void InitializeTabAndViews() 
        {
            tabsContainer = RootElement.Q<VisualElement>("settings-tabs");
            viewsContainer = RootElement.Q<VisualElement>("settings-views");

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
            var masterVolumeSlider = RootElement.Q<Slider>("master-volume-slider");
            var musicVolumeSlider = RootElement.Q<Slider>("music-volume-slider");
            var environmentVolumeSlider = RootElement.Q<Slider>("environment-volume-slider");
            var interfaceVolumeSlider = RootElement.Q<Slider>("interface-volume-slider");

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
    }
}
