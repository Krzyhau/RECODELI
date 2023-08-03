using RecoDeli.Scripts.Gameplay.Robot;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class AddInstructionMenu : MonoBehaviour
    {
        [SerializeField] private List<string> validActionsList;

        private InstructionEditor instructionEditor;

        private VisualElement instructionEditorContainer;
        private VisualElement addInstructionMenu;
        private VisualElement addInstructionList;
        private Button closeButton;
        private Label titleLabel;

        private Dictionary<RobotAction, Button> actionButtonsCache = new();

        private bool opened = false;
        public bool Opened
        {
            get => opened;
            private set
            {
                opened = value;
                instructionEditorContainer.EnableInClassList("adding-instruction", opened);
            }
        }

        private bool replacing = false;
        private int replaceIndex;

        public void Initialize(InstructionEditor editor, UIDocument gameplayInterface)
        {
            instructionEditor = editor;

            instructionEditorContainer = gameplayInterface.rootVisualElement.Q<VisualElement>("instruction-editor");
            addInstructionMenu = gameplayInterface.rootVisualElement.Q<VisualElement>("add-instruction-menu");
            addInstructionList = gameplayInterface.rootVisualElement.Q<VisualElement>("add-instruction-list");

            closeButton = addInstructionMenu.Q<Button>("add-cancel-button");

            titleLabel = addInstructionMenu.Q<Label>("add-instruction-label");

            closeButton.clicked += () => { Opened = false; };

            CreateMenu();
        }

        private void CreateMenu()
        {
            foreach (var actionName in validActionsList)
            {
                var instruction = RobotAction.CreateInstruction(actionName);
                var button = new Button(() => OnInstructionClicked(instruction));
                button.text = actionName;
                button.AddToClassList("button");
                addInstructionList.Add(button);

                actionButtonsCache[instruction.Action] = button;
            }
        }

        private void OnInstructionClicked(RobotInstruction instruction)
        {
            if (replacing)
            {
                instructionEditor.ReplaceInstruction(replaceIndex, instruction);
            }
            else
            {
                instructionEditor.AddInstruction(instruction);
            }

            Finish();
        }

        public void StartAddingInstruction()
        {
            Opened = true;
            replacing = false;
            titleLabel.text = "ADD INSTRUCTION";

            foreach((var action, var button)  in actionButtonsCache)
            {
                button.SetEnabled(true);
            }
        }

        public void StartReplacingInstruction(int index, RobotAction actionToExclude)
        {
            Opened = true;
            replacing = true;
            replaceIndex = index;
            titleLabel.text = "REPLACE INSTRUCTION";

            foreach ((var action, var button) in actionButtonsCache)
            {
                button.SetEnabled(action != actionToExclude);
            }
        }

        public void Finish()
        {
            Opened = false;
        }
    }
}