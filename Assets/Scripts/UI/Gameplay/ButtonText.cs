using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RecoDeli.Scripts.UI
{
    [ExecuteInEditMode]
    public class ButtonText : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text ownText;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color disabledColor;

        private void Update()
        {
            if (button == null || ownText == null) return;

            if (!button.interactable)
            {
                ownText.color = disabledColor;
            }
            else
            {
                ownText.color = normalColor;
            }
        }
    }
}
