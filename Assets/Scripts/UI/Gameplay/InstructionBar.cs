using RecoDeli.Scripts.Gameplay.Robot;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.UI
{
    public class InstructionBar
    {
        private VisualElement barContainer;
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

            label = new Label();
            label.name = "action-name";
            barContainer.Add(label);
            label.RegisterCallback<MouseEnterEvent>(e => { hoveringOverHandle = true;});
            label.RegisterCallback<MouseLeaveEvent>(e => { hoveringOverHandle = false;});

            textFieldsContainer = new VisualElement();
            textFieldsContainer.name = "parameters";
            barContainer.Add(textFieldsContainer);
        }

        private void ConstructInputFields()
        {
            textFieldsContainer.Clear();
            var currentValues = Instruction.ParameterToStrings();

            for (int i = 0; i < Instruction.Action.ParameterStringCount; i++)
            {
                // TODO: allow other types of fields
                var field = new FloatField();
                field.value = float.Parse(currentValues[i]);
                field.RegisterValueChangedCallback(f => {
                    var values = Instruction.ParameterToStrings();
                    values[i] = field.value.ToString();
                    Instruction.SetParameterFromStrings(values);
                });
                textFieldsContainer.Add(field);
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
