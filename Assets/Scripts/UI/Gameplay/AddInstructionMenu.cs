using RecoDeli.Scripts.Gameplay.Robot;
using RecoDeli.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
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

        private VisualElement lastFocusedElementInMenu;

        private bool opened = false;
        public bool Opened
        {
            get => opened;
            set
            {
                if (value == opened) return;
                opened = value;
                instructionEditorContainer.EnableInClassList("adding-instruction", opened);

                var cancelAction = EventSystem.current.GetCurrentUIInputModule().cancel.action;
                if (opened)
                {
                    cancelAction.performed += OnNavigationCancelButton;
                }
                else
                {
                    cancelAction.performed -= OnNavigationCancelButton;
                }
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

            closeButton.clicked += CancelAddingInstruction;

            gameplayInterface.rootVisualElement.RegisterCallback<FocusInEvent>(OnFocusIn);

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

        private void OnFocusIn(FocusInEvent evt)
        {
            if (!Opened) return;

            if(!addInstructionMenu.ContainsElement(evt.target as VisualElement))
            {
                if (lastFocusedElementInMenu == null || !lastFocusedElementInMenu.enabledSelf)
                {
                    lastFocusedElementInMenu = actionButtonsCache.Where(a => a.Value.enabledSelf).First().Value;
                }
                addInstructionMenu.schedule.Execute(() => lastFocusedElementInMenu.Focus());
            }
            else
            {
                lastFocusedElementInMenu = evt.target as VisualElement;
            }
        }

        private void OnNavigationCancelButton(InputAction.CallbackContext ctx)
        {
            CancelAddingInstruction();
            ctx.action.Reset();
        }

        private void CancelAddingInstruction()
        {
            Opened = false;
            instructionEditor.FocusOnSelected();
            //addInstructionMenu.schedule.Execute(() => instructionEditor.ListDocument.Focus());
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
            instructionEditor.FocusOnSelected();
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