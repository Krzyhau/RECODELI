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

        private bool skippingNow;

        public float Timescale => timescaleValues[Mathf.FloorToInt(timescaleValues.Count * Mathf.Clamp(skippingNow ? 1.0f : slider.value, 0.0f, 0.999f))];
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
            ClampSliderValueToStep();

            if (!skippingNow)
            {
                slider.label = $"{Mathf.RoundToInt(Timescale * 100.0f)}%";
            }

            if (overrideTimescale) Time.timeScale = Timescale;
        }

        private void SetSkipping(bool skipping)
        {
            if(skippingNow == skipping)
            {
                return;
            }

            skippingNow = skipping;

            OnValueChanged();

            if (skipping)
            {
                slider.label = "SKIPPING";
            }
            
        }
    }
}
