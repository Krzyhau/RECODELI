using RecoDeli.Scripts.Assets.Scripts.UI;
using RecoDeli.Scripts.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            tabsContainer.ElementAt(0).style.display = DisplayStyle.None;
            SwitchToTab(1);
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
            var postProcessingField = RootElement.Q<DropdownField>("post-processing-dropdown");
            postProcessingField.choices = new List<string>() { "ON", "OFF" };
            postProcessingField.value = postProcessingField.choices[UserSettingsProvider.PostProcessing ? 0 : 1];
            postProcessingField.RegisterValueChangedCallback(e => 
                UserSettingsProvider.PostProcessing = (e.newValue == postProcessingField.choices[0])
            );

            var uiScaleField = RootElement.Q<DropdownField>("ui-scaling-dropdown");
            uiScaleField.choices = Enum.GetNames(typeof(InterfaceScaleSetter.Scale))
                .Select(s => s.ToUpper()).ToList();
            uiScaleField.value = uiScaleField.choices[(int)UserSettingsProvider.UIScale];
            uiScaleField.RegisterValueChangedCallback(e =>
            {
                var value = uiScaleField.choices.IndexOf(e.newValue);
                UserSettingsProvider.UIScale = (InterfaceScaleSetter.Scale)value;
            });
        }

        private void InitializeAudioSettings()
        {
            var masterVolumeSlider = RootElement.Q<Slider>("master-volume-slider");
            var musicVolumeSlider = RootElement.Q<Slider>("music-volume-slider");
            var environmentVolumeSlider = RootElement.Q<Slider>("environment-volume-slider");
            var interfaceVolumeSlider = RootElement.Q<Slider>("interface-volume-slider");

            masterVolumeSlider.value = UserSettingsProvider.MasterVolume;
            musicVolumeSlider.value = UserSettingsProvider.MusicVolume;
            environmentVolumeSlider.value = UserSettingsProvider.EnvironmentVolume;
            interfaceVolumeSlider.value = UserSettingsProvider.InterfaceVolume;

            masterVolumeSlider.RegisterValueChangedCallback(e => UserSettingsProvider.MasterVolume = e.newValue);
            musicVolumeSlider.RegisterValueChangedCallback(e => UserSettingsProvider.MusicVolume = e.newValue);
            environmentVolumeSlider.RegisterValueChangedCallback(e => UserSettingsProvider.EnvironmentVolume = e.newValue);
            interfaceVolumeSlider.RegisterValueChangedCallback(e => UserSettingsProvider.InterfaceVolume = e.newValue);
        }

        private void InitializeControlsSettings()
        {
            // too lazy to actually implement rebinding for now
            // and it's for web at this point anyway, so I'll just dump formatted text

            var rebindingList = RootElement.Q<Label>("placeholder-controls-label");

            rebindingList.text = ""
                + "<b>Controlling simulation:</b>\n"
                + " - <b>Spacebar</b>: Toggle simulation\n"
                + " - <b>Backspace</b>: Reset simulation\n"
                + " - <b>F3</b>: Follow drone\n"
                + " - <b>F4</b>: Follow package\n"
                + "\n"
                + "<b>Editing instructions</b>\n"
                + " - <b>Insert</b>: Add instruction\n"
                + " - <b>R</b>: Replace selected instruction\n"
                + " - <b>Delete</b>: Deleting selected instructions\n"
                + " - <b>Ctrl</b>: Selecting multiple instructions\n"
                + " - <b>Shift</b>: Selecting range of instructions\n"
                + "\n"
                + "there are more controls but at this point im too lazy to list them all\n"
                + "the interface is mostly mouse-based anyway lmfao";

            rebindingList.text = rebindingList.text.ToUpper();
        }
    }
}
