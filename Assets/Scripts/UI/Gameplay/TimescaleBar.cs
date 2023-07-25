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

        private float cachedSliderValue;
        private bool skippingNow;

        public float Timescale => timescaleValues[Mathf.FloorToInt(timescaleValues.Count * Mathf.Clamp(slider.value, 0.0f, 0.999f))];
        public bool Skipping
        {
            get => skippingNow;
            set => SetSkipping(value);
        }

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
            if (skippingNow)
            {
                slider.value = 1.0f;
                slider.label = "SKIPPING";
                return;
            }

            ClampSliderValueToStep();

            slider.label = $"{Mathf.RoundToInt(Timescale * 100.0f)}%";

            if (overrideTimescale) Time.timeScale = Timescale;
        }

        private void SetSkipping(bool skipping)
        {
            if(skippingNow == skipping)
            {
                return;
            }

            skippingNow = skipping;

            if (skipping)
            {
                cachedSliderValue = slider.value;
                slider.value = 1.0f;
            }
            else
            {
                slider.value = cachedSliderValue;
            }
        }
    }
}
