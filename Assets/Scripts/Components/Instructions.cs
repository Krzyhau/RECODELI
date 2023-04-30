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
        [SerializeField] private RectTransform instructions; //todo to jest .this???


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

            //Ustawia pozycje - dla 1dziecka anchored
            if (instructionsListParent.childCount > 1)
            {
                newPrefab.transform.position = newPrefab.transform.position.ChangeY(newHeight);
                MoveAddButton(-buttonOffset);
            }
            else
            {
                newPrefab.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,newHeight,0);
                addNewButtonTransform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, newHeight - 10 - buttonOffset, 0);
                Debug.Log(newHeight);
            }

            //zwieksza height instructions jak brakuje miejsca
            if (newPrefab.GetComponent<RectTransform>().anchoredPosition.y<-300)
            {
                var parentRect = instructions.transform.GetComponent<RectTransform>();
                parentRect.sizeDelta = parentRect.sizeDelta.ChangeY(parentRect.sizeDelta.y + buttonOffset);
            }

            newPrefab.ChangeLabel(text);
            newPrefab.gameObject.name = "Instruction: "+instructionsCount;
            newPrefab.instructions = this;

            RefreshAddNewButton();
            instructionsCount++;
            newHeight = -15;
        }

        private void RefreshAddNewButton()
        {
            addNewButtonTransform.transform.position.ChangeY
                (addNewButtonTransform.transform.position.y - buttonOffset);
        }
        public void MoveAddButton(float offset)
        {
            addNewButtonTransform.transform.position = addNewButtonTransform.transform.position
                    .ChangeY(addNewButtonTransform.transform.position.y + offset);
        }
    }
}
