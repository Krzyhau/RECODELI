using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace LudumDare.Scripts.Components.UI
{
    public class InstructionsScrolling : MonoBehaviour
    {
        [SerializeField] private Scrollbar verticalScrollBar;
        [SerializeField] private float mouseScrollScale;
        [SerializeField] private float mouseScrollTime;
        [SerializeField] private float edgeProximityDistance;
        [SerializeField] private float edgeProximityMaxSpeed;

        private RectTransform instructionsContainer;
        private RectTransform instructionsContainerParent;
        private Canvas canvasContainer;
        private InstructionEditor instructionEditor;

        private float mouseScrollFactor;
        private float mouseScrollState;

        private float parentVerticalMousePosition;

        private void Awake()
        {
            instructionsContainer = GetComponent<RectTransform>();
            instructionsContainerParent = instructionsContainer.parent.GetComponent<RectTransform>();
            canvasContainer = instructionsContainer.GetComponentInParent<Canvas>();
            instructionEditor = GetComponent<InstructionEditor>();
        }

        private void Update()
        {
            RecalculateButtonBasedMousePosition();

            HandleGrabbingEdgeProximityScrolling();
            HandleMouseScrolling();

            verticalScrollBar.value = Mathf.Clamp(verticalScrollBar.value, 0.0f, 1.0f);

            ApplyScrollingToContainer();
        }

        private void RecalculateButtonBasedMousePosition()
        {
            var mouseInRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                instructionsContainerParent, Input.mousePosition, canvasContainer.worldCamera, out var mousePosInRect
            );
            if (!mouseInRect) return;

            parentVerticalMousePosition = -mousePosInRect.y;
        }

        private void HandleMouseScrolling()
        {
            float containerHeight = instructionsContainer.sizeDelta.y;
            float maxHeight = instructionsContainerParent.rect.height;

            if (containerHeight <= maxHeight)
            {
                mouseScrollState = 0.0f;
                return;
            }

            var scrollingFactor = Input.mouseScrollDelta.y * mouseScrollScale;
            
            if(scrollingFactor != 0.0f)
            {
                mouseScrollFactor *= mouseScrollState / mouseScrollTime;
                mouseScrollFactor -= scrollingFactor;
                mouseScrollState = mouseScrollTime;
            }

            if(mouseScrollState > 0.0f)
            {
                mouseScrollState = Mathf.Max(0.0f, mouseScrollState - Time.unscaledDeltaTime);
                float sizeScalar = Mathf.Max(0.0f, containerHeight - maxHeight);

                float scrollValueStep = mouseScrollFactor / sizeScalar * Time.unscaledDeltaTime;
                verticalScrollBar.value += scrollValueStep;
                if(verticalScrollBar.value <= 0.0f || verticalScrollBar.value >= 1.0f)
                {
                    mouseScrollState = 0.0f;
                }
            }
        }

        private void HandleGrabbingEdgeProximityScrolling()
        {
            if (instructionEditor == null || !instructionEditor.Grabbing) return;

            float parentHeight = instructionsContainerParent.rect.height;
            var upperEdge = Mathf.Lerp(-edgeProximityMaxSpeed, 0.0f, parentVerticalMousePosition / edgeProximityDistance);
            var lowerEdge = Mathf.Lerp(edgeProximityMaxSpeed, 0.0f, (parentHeight - parentVerticalMousePosition) / edgeProximityDistance);

            verticalScrollBar.value += (upperEdge + lowerEdge) * Time.unscaledDeltaTime;
        }

        private void ApplyScrollingToContainer()
        {
            float containerHeight = instructionsContainer.sizeDelta.y;
            float maxHeight = instructionsContainerParent.rect.height;

            float scrollSize = maxHeight / Mathf.Max(maxHeight, containerHeight);
            verticalScrollBar.size = scrollSize;

            float containerNewPosition = Mathf.Max(0.0f, containerHeight - maxHeight) * verticalScrollBar.value;
            instructionsContainer.anchoredPosition = Vector2.up * containerNewPosition;
        }
    }
}
