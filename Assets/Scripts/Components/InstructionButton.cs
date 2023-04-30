using LudumDare.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace LudumDare.Scripts.Components
{
    public class InstructionButton : MonoBehaviour
    {
        [SerializeField] private Text textLabel;
        [SerializeField] private Text textInput;
        public Instructions instructions;
        public void ChangeLabel(string label)
        {
            textLabel.text = label;
        }
        public void MoveUp()
        {
            var i = transform.GetSiblingIndex();
            if (i > 0)
            {
                var childTransform = transform.parent.GetChild(i).transform;
                var childTransform2 = transform.parent.GetChild(i-1).transform;
                var newHeight = childTransform.position.y + Instructions.buttonOffset;
                var newHeight2 = childTransform2.position.y - Instructions.buttonOffset;
                childTransform.position = childTransform.position.ChangeY(newHeight);
                childTransform2.position = childTransform2.position.ChangeY(newHeight2);
                childTransform.SetSiblingIndex(i - 1);
            }
        }
        public void MoveDown()
        {
            var i = transform.GetSiblingIndex();
            if (i<transform.parent.childCount-1)
            {
                var childTransform = transform.parent.GetChild(i).transform;
                var childTransform2 = transform.parent.GetChild(i + 1).transform;
                var newHeight = childTransform.position.y - Instructions.buttonOffset;
                var newHeight2 = childTransform2.position.y + Instructions.buttonOffset;
                childTransform.position = childTransform.position.ChangeY(newHeight);
                childTransform2.position = childTransform2.position.ChangeY(newHeight2);
                childTransform.SetSiblingIndex(i + 1);
            }
        }
        public void RemoveInstruction()
        {
            for (int i = transform.GetSiblingIndex(); i < transform.parent.childCount; i++)
            {
                var childTransform = transform.parent.GetChild(i).transform;
                var newHeight = childTransform.position.y + Instructions.buttonOffset;
                childTransform.position =
                    childTransform.position.ChangeY(newHeight);
            }
            instructions.MoveAddButton(Instructions.buttonOffset);
            /*addNewButtonTransform.transform.position = addNewButtonTransform.transform.position
                    .ChangeY(addNewButtonTransform.transform.position.y - buttonOffset);*/
            Destroy(this.gameObject);
        }
    }
}
