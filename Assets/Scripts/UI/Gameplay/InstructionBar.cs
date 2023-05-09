using LudumDare.Scripts.Models;
using LudumDare.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LudumDare.Scripts.Components.UI
{
    public class InstructionBar : MonoBehaviour
    {
        [SerializeField] private Sprite selectedSprite;
        [SerializeField] private Button barHandle;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_InputField primaryField; 
        [SerializeField] private TMP_InputField secondaryField;
        [SerializeField] private RectTransform progressBar;

        private Sprite originalSprite;
        private Image barHandleImage;
        private bool selected;

        private float progressInterpState;
        private float progressInterpStateTarget;
        private float progressInterpCalcSpeed;

        private RobotInstruction instruction;

        public Button.ButtonClickedEvent OnClick => barHandle.onClick;

        public bool Selected
        {
            get { return selected; }
            set {
                selected = value;
                if (barHandleImage == null)
                {
                    barHandleImage = barHandle.GetComponent<Image>();
                    originalSprite = barHandleImage.sprite;
                }
                barHandleImage.sprite = selected ? selectedSprite : originalSprite;
            }
        }
        public RobotInstruction Instruction
        {
            get { return instruction; }
            set
            {
                instruction = value;
                titleText.text = instruction.Action.Name;
                if(primaryField != null && secondaryField != null)
                {
                    secondaryField.gameObject.SetActive(instruction.Action.ParameterStringCount > 1);
                    var paramStrings = instruction.ParameterToStrings();
                    primaryField.text = paramStrings[0];
                    if (paramStrings.Length > 1) secondaryField.text = paramStrings[1];
                }
            }
        }

        private void OnEnable()
        {
            if (primaryField != null) primaryField.onValueChanged.AddListener(OnParameterChanged);
            if (secondaryField != null) secondaryField.onValueChanged.AddListener(OnParameterChanged);
        }
        private void OnDisable()
        {
            if (primaryField != null) primaryField.onValueChanged.RemoveListener(OnParameterChanged);
            if (secondaryField != null) secondaryField.onValueChanged.RemoveListener(OnParameterChanged);
        }

        private void FixedUpdate()
        {
            if (progressBar != null)
            {
                progressInterpState = progressInterpStateTarget;
                progressInterpStateTarget = instruction.Progress;

                progressInterpCalcSpeed = (progressInterpStateTarget - progressInterpState) / Time.fixedDeltaTime;
            }
        }

        private void Update()
        {
            if(progressBar != null)
            {
                progressInterpState = Mathf.MoveTowards(progressInterpState, progressInterpStateTarget, progressInterpCalcSpeed * Time.deltaTime);
                progressBar.localScale = new Vector3(progressInterpState, 1.0f, 1.0f);
            }
        }

        private void OnParameterChanged(string _)
        {
            if (Instruction == null || primaryField == null || secondaryField == null) return;

            Instruction.SetParameterFromStrings(new string[] { primaryField.text, secondaryField.text });
        }

        public bool IsPointerHoveringOnHandle()
        {
            return EventSystem.current.IsPointerOverGameObject(barHandle.gameObject);
        }
    }
}
