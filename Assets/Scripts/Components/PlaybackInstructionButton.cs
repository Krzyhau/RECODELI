using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LudumDare.Scripts
{
    public class PlaybackInstructionButton : MonoBehaviour
    {
        [SerializeField] private Text textLabel;
        [SerializeField] private Text textInput;
        [SerializeField] private Sprite highlightSprite;

        private Image background;
        private Sprite defaultSprite;
        private void Awake()
        {
            background = GetComponent<Image>();
            defaultSprite = background.sprite;
        }
        public void ChangeLabel(string label)
        {
            textLabel.text = label;
        }
        public void ChangeValue(float value)
        {
            textInput.text = value.ToString().Replace(",", ".") + "s";
        }
        public void SetHighlighted(bool highlighted)
        { 
            background.sprite = (highlighted) ? highlightSprite : defaultSprite;
        }
    }
}
