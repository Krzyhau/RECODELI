using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LudumDare.Scripts
{
    public class TimescaleBar : MonoBehaviour
    {
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private TMP_Text text;
        [SerializeField] private List<float> timescaleValues;
        [SerializeField] private bool overrideTimescale;

        public float Timescale => timescaleValues[Mathf.FloorToInt(timescaleValues.Count * Mathf.Clamp(scrollbar.value, 0.0f, 0.999f))];

        private void OnEnable() => scrollbar.onValueChanged.AddListener(OnValueChanged);
        private void OnDisable() => scrollbar.onValueChanged.RemoveListener(OnValueChanged);

        private void Awake()
        {
            scrollbar.numberOfSteps = timescaleValues.Count;
        }

        private void OnValueChanged(float value)
        {
            scrollbar.numberOfSteps = timescaleValues.Count;

            if (overrideTimescale)
            {
                Time.timeScale = Timescale;
            }

            text.text = $"{Mathf.RoundToInt(Timescale * 100.0f)}%";
        }
    }
}
