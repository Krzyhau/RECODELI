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

        private bool opened = false;
        public bool Opened
        {
            get => opened;
            set
            {
                opened = value;
                instructionEditorContainer.EnableInClassList("adding-instruction", opened);
            }
        }

        public void Initialize(InstructionEditor editor, UIDocument gameplayInterface)
        {
            instructionEditor = editor;

            instructionEditorContainer = gameplayInterface.rootVisualElement.Q<VisualElement>("instruction-editor");
            addInstructionMenu = gameplayInterface.rootVisualElement.Q<VisualElement>("add-instruction-menu");
            addInstructionList = gameplayInterface.rootVisualElement.Q<VisualElement>("add-instruction-list");

            closeButton = addInstructionMenu.Q<Button>("add-cancel-button");

            closeButton.clicked += () => { Opened = false; };

            CreateMenu();
        }

        private void CreateMenu()
        {
            foreach (var actionName in validActionsList)
            {
                var instruction = RobotAction.CreateInstruction(actionName);
                var button = new Button(() => instructionEditor.AddInstruction(instruction));
                button.text = actionName;
                button.AddToClassList("button");
                addInstructionList.Add(button);
            }
        }
    }
}