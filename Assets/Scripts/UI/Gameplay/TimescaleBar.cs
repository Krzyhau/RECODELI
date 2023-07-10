using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class TimescaleBar : MonoBehaviour
    {
        [SerializeField] private List<float> timescaleValues;
        [SerializeField] private bool overrideTimescale;

        private Slider slider;

        public float Timescale => timescaleValues[Mathf.FloorToInt(timescaleValues.Count * Mathf.Clamp(slider.value, 0.0f, 0.999f))];

        public void Initialize(UIDocument gameplayInterface)
        {
            slider = gameplayInterface.rootVisualElement.Q<Slider>("timescale-slider");
            slider.RegisterValueChangedCallback(f => OnValueChanged());
        }

        private void ClampSliderValueToStep()
        {
            var steps = (float)timescaleValues.Count - 1.0f;
            slider.value = Mathf.Round(slider.value * steps) / steps;
        }

        private void OnValueChanged()
        {
            ClampSliderValueToStep();

            slider.label = $"{Mathf.RoundToInt(Timescale * 100.0f)}%";

            if (overrideTimescale) Time.timeScale = Timescale;
        }
    }
}
