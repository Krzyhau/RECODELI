using LudumDare.Scripts.Models;
using LudumDare.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare.Scripts.Components
{
    public class Instructions : MonoBehaviour //TODO scroll
    {
        public const float buttonOffset = 40f;

        [SerializeField] private InstructionButton instructionButtonPrefab;
        [SerializeField] private PlaybackInstructionButton playbackPrefab;


        [SerializeField] private RectTransform instructionsListParent;
        [SerializeField] private RectTransform addNewButtonTransform;
        [SerializeField] private RectTransform playbackInstructionsList;


        private float newHeight = -15;
        private int instructionsCount;

        public void AddInstruction(string text)
        {
            if (instructionsListParent.childCount > 0)
            {
                newHeight = instructionsListParent.GetChild(instructionsListParent.childCount - 1)
                    .GetComponent<RectTransform>().anchoredPosition.y - buttonOffset;
                //OLD na transform position
                //newHeight = instructionsListParent.GetChild(instructionsListParent.childCount - 1).transform.position.y - buttonOffset;
            }
            
            var newPrefab = Instantiate(instructionButtonPrefab, instructionsListParent);
            var newPrefabRect = newPrefab.GetComponent<RectTransform>();
            //Ustawia pozycje - dla 1dziecka
            if (instructionsListParent.childCount > 1)
            {
                newPrefabRect.anchoredPosition = newPrefabRect.anchoredPosition.ChangeY(newHeight);
                MoveAddButton(-buttonOffset);
                //OLD na transform position
                //newPrefab.transform.position = newPrefab.transform.position.ChangeY(newHeight);
            }
            else
            {
                newPrefabRect.anchoredPosition = new Vector3(0,newHeight,0);
                addNewButtonTransform.anchoredPosition = new Vector3(0, newHeight - 10 - buttonOffset, 0);
                Debug.Log(newHeight);
            }

            //zwieksza height instructions jak brakuje miejsca
            if (newPrefabRect.anchoredPosition.y<-300)
            {
                var instructionsRect = GetComponent<RectTransform>();
                instructionsRect.sizeDelta = instructionsRect.sizeDelta.ChangeY(instructionsRect.sizeDelta.y + buttonOffset);
            }

            newPrefab.ChangeLabel(text);
            newPrefab.gameObject.name = "Instruction: "+instructionsCount;
            newPrefab.instructions = this;

            instructionsCount++;
            newHeight = -15;
        }

        public List<RobotAction> GetRobotActionList()
        {
            var actionList = new List<RobotAction>();

            for (int i = 0; i < instructionsListParent.childCount; i++)
            {
                if (!instructionsListParent.GetChild(i).TryGetComponent(out InstructionButton button)) continue;
                var robotAction = RobotAction.CreateFromName(button.Label, button.GetParameterValue());
                if (robotAction != null)
                {
                    actionList.Add(robotAction);
                }
            }

            return actionList;
        }

        public void MoveAddButton(float offset)
        {
            newHeight = addNewButtonTransform.anchoredPosition.y + offset;
            addNewButtonTransform.anchoredPosition = addNewButtonTransform.anchoredPosition.ChangeY(newHeight);

            //OLD na transform position 
            /*addNewButtonTransform.transform.position = addNewButtonTransform.transform.position
                    .ChangeY(addNewButtonTransform.transform.position.y + offset);*/
        }

        public void HighlightInstruction(int id)
        {
            //OLD
            /*for (int i = 0; i < instructionsListParent.childCount; i++)
            {
                if (!instructionsListParent.GetChild(i).TryGetComponent(out InstructionButton button)) continue;

                button.SetHighlighted(i == id);
            }*/
            for (int i = 0; i < playbackInstructionsList.childCount; i++)
            {
                if (!playbackInstructionsList.GetChild(i).TryGetComponent(out PlaybackInstructionButton button)) continue;
                button.SetHighlighted(i == id);
            }
        }

        public void UpdatePlaybackInstruction()
        {
            for (var i = 0;i< instructionsListParent.childCount; i++)
            {
                var newPrefab = Instantiate(playbackPrefab, playbackInstructionsList);
                newPrefab.GetComponent<RectTransform>().anchoredPosition = 
                    instructionsListParent.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
                InstructionButton property = instructionsListParent.GetChild(i).GetComponent<InstructionButton>();
                newPrefab.ChangeLabel(property.Label);
                newPrefab.ChangeValue((property.GetParameterValue()));
            }
        }
        public void RemovePlaybackInstruction()
        {
            for (var i = 0; i < playbackInstructionsList.childCount; i++)
            {
                Destroy(playbackInstructionsList.GetChild(i).gameObject);
            }
        }
    }
}
