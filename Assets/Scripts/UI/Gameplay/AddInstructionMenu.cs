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
            set
            {
                opened = value;
                instructionEditorContainer.EnableInClassList("adding-instruction", opened);
                if (opened) addInstructionMenu.Focus();
            }
        }

        private int replaceIndex;
        private RobotInstruction instructionToReplace;

        public bool Replacing => instructionToReplace != null;

        public void Initialize(InstructionEditor editor, UIDocument gameplayInterface)
        {
            instructionEditor = editor;

            instructionEditorContainer = gameplayInterface.rootVisualElement.Q<VisualElement>("instruction-editor");
            addInstructionMenu = gameplayInterface.rootVisualElement.Q<VisualElement>("add-instruction-menu");
            addInstructionList = gameplayInterface.rootVisualElement.Q<VisualElement>("add-instruction-list");

            closeButton = addInstructionMenu.Q<Button>("add-cancel-button");

            titleLabel = addInstructionMenu.Q<Label>("add-instruction-label");

            closeButton.clicked += () => { Opened = false; };

            addInstructionMenu.RegisterCallback<NavigationCancelEvent>(OnNavigationCancel);

            CreateMenu();
        }

        private void CreateMenu()
        {
            foreach (var actionName in validActionsList)
            {
                var action = RobotAction.GetByName(actionName);
                var button = new Button(() => OnActionClicked(action));
                button.text = actionName;
                button.AddToClassList("button");
                addInstructionList.Add(button);

                actionButtonsCache[action] = button;
            }
        }

        private void OnNavigationCancel(NavigationCancelEvent evt)
        {
            Opened = false;
        }

        private void OnActionClicked(RobotAction action)
        {
            var instruction = action.CreateInstruction();

            if (Replacing)
            {
                instruction.TryTransferParametersFrom(instructionToReplace);
                instructionEditor.ReplaceInstruction(replaceIndex, instruction);
            }
            else
            {
                instructionEditor.AddInstruction(instruction);
            }

            Opened = false;
        }

        public void StartAddingInstruction()
        {
            Opened = true;
            instructionToReplace = null;
            titleLabel.text = "ADD INSTRUCTION";

            foreach((var action, var button)  in actionButtonsCache)
            {
                button.SetEnabled(true);
            }
        }

        public void StartReplacingInstruction(int index, RobotInstruction replacedInstruction)
        {
            Opened = true;
            instructionToReplace = replacedInstruction;
            replaceIndex = index;
            titleLabel.text = "REPLACE INSTRUCTION";

            foreach ((var action, var button) in actionButtonsCache)
            {
                button.SetEnabled(action != replacedInstruction.Action);
            }
        }
    }
}