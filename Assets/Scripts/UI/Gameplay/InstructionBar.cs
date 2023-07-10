using RecoDeli.Scripts.Gameplay.Robot;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class InstructionBar
    {
        private VisualElement barContainer;
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

        public bool Selected
        {
            get { return selected; }
            set {
                selected = value;
                barContainer.EnableInClassList("selected", selected);
            }
        }
        public RobotInstruction Instruction
        {
            get { return instruction; }
            set
            {
                instruction = value;
                label.text = instruction.Action.Name;
                ConstructInputFields();
            }
        }
        public VisualElement Element => barContainer;

        public InstructionBar()
        {
            ConstructBase();
        }

        private void ConstructBase()
        {
            barContainer = new VisualElement();
            barContainer.name = "instruction-bar";
            barContainer.AddToClassList("instruction-bar");

            progressBar = new ProgressBar();
            progressBar.name = "progress";
            progressBar.lowValue = 0.0f;
            progressBar.highValue = 1.0f;
            barContainer.Add(progressBar);

            grabbingHandle = new VisualElement();
            grabbingHandle.name = "grabbing-handle";
            grabbingHandle.RegisterCallback<MouseEnterEvent>(e => { hoveringOverHandle = true;});
            grabbingHandle.RegisterCallback<MouseLeaveEvent>(e => { hoveringOverHandle = false;});
            barContainer.Add(grabbingHandle);

            var handleCharacter = new Label();
            handleCharacter.name = "handle-character";
            handleCharacter.text = "=";
            grabbingHandle.Add(handleCharacter);

            label = new Label();
            label.name = "action-name";
            grabbingHandle.Add(label);

            textFieldsContainer = new VisualElement();
            textFieldsContainer.name = "parameters";
            barContainer.Add(textFieldsContainer);
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
                Instruction.SetInputParameterFromString(parameterIndex, field.value.ToString());
            }
        }

        public void UpdateProgressBar()
        {
            if (progressBar == null) return;
            
            if(progressInterpStateTarget != instruction.Progress)
            {
                progressInterpState = progressInterpStateTarget;
                progressInterpStateTarget = instruction.Progress;

                progressInterpCalcSpeed = (progressInterpStateTarget - progressInterpState) / Time.fixedDeltaTime;
            }

            progressInterpState = Mathf.MoveTowards(progressInterpState, progressInterpStateTarget, progressInterpCalcSpeed * Time.deltaTime);
            progressBar.value = progressInterpState;
            
        }

        public bool IsPointerHoveringOnHandle()
        {
            return hoveringOverHandle;
        }
    }
}
