using LudumDare.Scripts.Utils;
using UnityEngine;
using Zenject;

namespace LudumDare.Scripts.Components
{
    public class Instructions : MonoBehaviour //TODO - 10px na start pozniej 5px przerwy, scrollbar dla instructions
    {
        public const float buttonOffset = 35f;

        [Inject] private readonly InstructionButton instructionButtonPrefab;

        [SerializeField] private RectTransform instructionsListParent;
        [SerializeField] private RectTransform addNewButtonTransform;

        private float newHeight = -15;
        private int instructionsCount;

        public void AddInstruction(string text)
        {
            if (instructionsListParent.childCount > 0)
            {
                newHeight = instructionsListParent.GetChild(instructionsListParent.childCount - 1)
                    .transform.position.y - buttonOffset;
            }
            
            var newPrefab = Instantiate(instructionButtonPrefab, instructionsListParent);
            if (instructionsListParent.childCount > 1)
            {
                newPrefab.transform.position = newPrefab.transform.position.ChangeY(newHeight);
            }
            else
            {
                newPrefab.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,newHeight,0);
            }
            addNewButtonTransform.transform.position = addNewButtonTransform.transform.position.ChangeY(newHeight);
            newPrefab.ChangeLabel(text);
            newPrefab.gameObject.name = "Instruction: "+instructionsCount;


            RefreshAddNewButton();
            instructionsCount++;
        }

        private void RefreshAddNewButton()
        {
            addNewButtonTransform.transform.position.ChangeY
                (addNewButtonTransform.transform.position.y - buttonOffset);
        }
    }
}
