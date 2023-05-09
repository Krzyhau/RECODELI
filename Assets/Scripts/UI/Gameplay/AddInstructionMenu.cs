using RecoDeli.Scripts.Gameplay.Robot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecoDeli.Scripts.UI
{
    public class ActionAddMenu : MonoBehaviour
    {
        [SerializeField] private InstructionBar actionButtonPrefab;
        [SerializeField] private Transform buttonsContainer;
        [SerializeField] private InstructionEditor instructionEditor;
        [SerializeField] private List<string> availableActionsList;

        private void Awake()
        {
            CreateActionButtons();
        }

        private void CreateActionButtons()
        {
            // destroy previous buttons
            foreach(Transform previousChild in buttonsContainer)
            {
                Destroy(previousChild.gameObject);
            }

            // create new buttons
            foreach(var action in availableActionsList)
            {
                var button = Instantiate(actionButtonPrefab, buttonsContainer);
                button.Instruction = RobotAction.CreateInstruction(action);
                button.OnClick.AddListener(delegate { SetInstruction(button.Instruction); });
            }
        }

        private void SetInstruction(RobotInstruction instruction)
        {
            instructionEditor.AddInstruction(instruction);
        }
    }
}
