using LudumDare.Scripts.Utils;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace LudumDare.Scripts.Components
{
    public class InstructionButton : MonoBehaviour
    {
        [SerializeField] private Text textLabel;
        [SerializeField] private Text textInput;
        public Instructions instructions;
        private float newHeight;

        public string Label => textLabel.text;

        public void ChangeLabel(string label)
        {
            textLabel.text = label;
        }

        public float GetParameterValue()
        {
            bool valid = float.TryParse(textInput.text.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out var parameter);
            return !valid ? 0.0f : parameter;
        }

        public void MoveUp() //TODO przepuœciæ przez kompilator Szymona.
        {
            var i = transform.GetSiblingIndex();
            if (i > 0)
            {
                var childTransform = transform.parent.GetChild(i).GetComponent<RectTransform>();
                var childTransform2 = transform.parent.GetChild(i-1).GetComponent<RectTransform>();

                var newHeight = childTransform.anchoredPosition.y + Instructions.buttonOffset;
                var newHeight2 = childTransform2.anchoredPosition.y - Instructions.buttonOffset;

                childTransform.anchoredPosition = childTransform.anchoredPosition.ChangeY(newHeight);
                childTransform2.anchoredPosition = childTransform2.anchoredPosition.ChangeY(newHeight2);
                childTransform.SetSiblingIndex(i - 1);
            }
        }
        public void MoveDown()
        {
            var i = transform.GetSiblingIndex();
            if (i<transform.parent.childCount-1)
            {
                var childTransform = transform.parent.GetChild(i).GetComponent<RectTransform>();
                var childTransform2 = transform.parent.GetChild(i + 1).GetComponent<RectTransform>();

                var newHeight = childTransform.anchoredPosition.y - Instructions.buttonOffset;
                var newHeight2 = childTransform2.anchoredPosition.y + Instructions.buttonOffset;

                childTransform.anchoredPosition = childTransform.anchoredPosition.ChangeY(newHeight);
                childTransform2.anchoredPosition = childTransform2.anchoredPosition.ChangeY(newHeight2);
                childTransform.SetSiblingIndex(i + 1);
            }
        }
        public void RemoveInstruction()
        {
            for (int i = transform.GetSiblingIndex(); i < transform.parent.childCount; i++)
            {
                var childTransform = transform.parent.GetChild(i).transform;
                newHeight = childTransform.position.y + Instructions.buttonOffset;
                childTransform.position =
                    childTransform.position.ChangeY(newHeight);
            }
            instructions.MoveAddButton(Instructions.buttonOffset);

            var lastChildTransform = transform.parent.GetChild(transform.parent.childCount-1).transform;
            if (lastChildTransform.GetComponent<RectTransform>().anchoredPosition.y < -290)
            {
                var parentRect = instructions.transform.GetComponent<RectTransform>();
                parentRect.sizeDelta = parentRect.sizeDelta.ChangeY(parentRect.sizeDelta.y - Instructions.buttonOffset);
            }

            Destroy(this.gameObject);
        }
    }
}
