using RecoDeli.Scripts.Gameplay.Robot;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class InstructionBar : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InstructionBar, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private VisualElement grabbingHandle;
        private Label label;
        private ProgressBar progressBar;
        private VisualElement textFieldsContainer;

        private bool selected;
        private bool hoveringOverHandle;

        private float progressInterpState;
        private float progressInterpStateTarget;
        private float progressInterpCalcSpeed;

        private RobotInstruction instruction;

        public Action changing;
        public Action changed;

        public bool Selected
        {
            get => selected;
            set {
                selected = value;
                EnableInClassList("selected", selected);
            }
        }
        public RobotInstruction Instruction
        {
            get => instruction;
            set
            {
                instruction = value;
                label.text = instruction.Action.Name;
                ConstructInputFields();
            }
        }

        public InstructionBar()
        {
            ConstructBase();
        }

        private void ConstructBase()
        {
            this.name = "instruction-bar";
            this.AddToClassList("instruction-bar");

            progressBar = new ProgressBar();
            progressBar.name = "progress";
            progressBar.lowValue = 0.0f;
            progressBar.highValue = 1.0f;
            this.Add(progressBar);

            grabbingHandle = new VisualElement();
            grabbingHandle.name = "grabbing-handle";
            grabbingHandle.RegisterCallback<MouseEnterEvent>(e => { hoveringOverHandle = true;});
            grabbingHandle.RegisterCallback<MouseLeaveEvent>(e => { hoveringOverHandle = false;});
            this.Add(grabbingHandle);

            var handleCharacter = new Label();
            handleCharacter.name = "handle-character";
            handleCharacter.text = "=";
            grabbingHandle.Add(handleCharacter);

            label = new Label();
            label.name = "action-name";
            grabbingHandle.Add(label);

            textFieldsContainer = new VisualElement();
            textFieldsContainer.name = "parameters";
            this.Add(textFieldsContainer);
        }

        private void ConstructInputFields()
        {
            textFieldsContainer.Clear();

            for (int i = 0; i < Instruction.Action.InputParametersCount; i++)
            {
                var paramIndex = i;
                var field = new TextField();
                field.value = Instruction.GetInputParameterAsString(i);
                field.RegisterValueChangedCallback(evt => OnInputFieldChanged(field, paramIndex, evt));
                textFieldsContainer.Add(field);
            }
        }

        private void OnInputFieldChanged(TextField field, int parameterIndex, ChangeEvent<string> evt) 
        {
            var type = Instruction.Action.GetParameterInputType(parameterIndex);

            bool preventChange = false;

            if(type == typeof(float))
            {
                string validNumberPattern = @"^[+-]?\d*\.?\d*$";
                if(!Regex.IsMatch(evt.newValue, validNumberPattern))
                {
                    preventChange = true;
                }
            }

            if (preventChange)
            {
                field.SetValueWithoutNotify(evt.previousValue);
                // prevents the cursor from shifting right when typing illegal characters 
                field.cursorIndex -= Mathf.Max(0, evt.newValue.Length - evt.previousValue.Length);
                field.selectIndex = field.cursorIndex;
            }
            else
            {
                changing?.Invoke();
                Instruction.SetInputParameterFromString(parameterIndex, field.value.ToString());
                changed?.Invoke();
            }
        }

        public void Update()
        {
            UpdateProgressBar();
        }

        private void UpdateProgressBar()
        {
            if (progressBar == null) return;
            
            if(progressInterpStateTarget < instruction.Progress)
            {
                progressInterpState = progressInterpStateTarget;
                progressInterpStateTarget = instruction.Progress;

                progressInterpCalcSpeed = (progressInterpStateTarget - progressInterpState) / Time.fixedDeltaTime;
            }
            else if(progressInterpStateTarget > instruction.Progress)
            {
                progressInterpState = instruction.Progress;
                progressInterpStateTarget = instruction.Progress;
                progressInterpCalcSpeed = 0.0f;
            }

            progressBar.value = progressInterpState = Mathf.MoveTowards(
                progressInterpState, 
                progressInterpStateTarget, 
                progressInterpCalcSpeed * Time.deltaTime
            );

            EnableInClassList("in-progress", progressBar.value > 0);
        }

        public bool IsPointerHoveringOnHandle()
        {
            return hoveringOverHandle;
        }
    }
}
