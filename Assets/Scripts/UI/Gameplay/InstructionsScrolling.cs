using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RecoDeli.Scripts.UI
{
    public class InstructionsScrolling : MonoBehaviour
    {
        [SerializeField] private Scrollbar verticalScrollBar;
        [SerializeField] private InstructionEditor instructionEditor;
        [SerializeField] private float mouseScrollScale;
        [SerializeField] private float mouseScrollTime;
        [SerializeField] private float edgeProximityDistance;
        [SerializeField] private float edgeProximityMaxSpeed;
        [SerializeField] private float interpolationEdgeDistance;
        [SerializeField] private AnimationCurve interpolationCurve;

        private RectTransform instructionsContainer;
        private RectTransform instructionsContainerParent;
        private Canvas canvasContainer;

        private float mouseScrollFactor;
        private float mouseScrollState;

        private float parentVerticalMousePosition;

        private bool interpolate = false;
        private float interpolationState = 0.0f;
        private float interpolationStartPosition;
        private float interpolationEndPosition;

        public float CurrentScrollPosition => instructionsContainer.anchoredPosition.y;

        private void Awake()
        {
            instructionsContainer = GetComponent<RectTransform>();
            instructionsContainerParent = instructionsContainer.parent.GetComponent<RectTransform>();
            canvasContainer = instructionsContainer.GetComponentInParent<Canvas>();
            instructionEditor = GetComponent<InstructionEditor>();
        }

        private void Update()
        {
            RecalculateVerticalMousePosition();

            HandleGrabbingEdgeProximityScrolling();
            HandleMouseScrolling();
            HandleInterpolation();

            verticalScrollBar.value = Mathf.Clamp(verticalScrollBar.value, 0.0f, 1.0f);

            ApplyScrollingToContainer();
        }

        private void RecalculateVerticalMousePosition()
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

                interpolate = false;
            }
        }

        private void HandleGrabbingEdgeProximityScrolling()
        {
            if (instructionEditor == null || !instructionEditor.Grabbing) return;

            float parentHeight = instructionsContainerParent.rect.height;
            var upperEdge = Mathf.Lerp(-edgeProximityMaxSpeed, 0.0f, parentVerticalMousePosition / edgeProximityDistance);
            var lowerEdge = Mathf.Lerp(edgeProximityMaxSpeed, 0.0f, (parentHeight - parentVerticalMousePosition) / edgeProximityDistance);

            float scrollValue = (upperEdge + lowerEdge) * Time.unscaledDeltaTime;

            verticalScrollBar.value += scrollValue;

            if (scrollValue != 0) interpolate = false;
        }

        private void HandleInterpolation()
        {
            if (!interpolate) return;

            float containerHeight = instructionsContainer.sizeDelta.y;
            float maxHeight = instructionsContainerParent.rect.height;

            if (containerHeight <= maxHeight)
            {
                interpolate = false;
                return;
            }

            interpolationState += Time.unscaledDeltaTime;
            var interpCurveState = interpolationCurve.Evaluate(interpolationState);

            if(interpCurveState >= 1.0f)
            {
                interpolate = false;
            }

            var targetPosition = Mathf.Lerp(interpolationStartPosition, interpolationEndPosition, interpCurveState);

            verticalScrollBar.value = targetPosition / (containerHeight - maxHeight);
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

        public void InterpolateToPosition(float position, bool literal=false)
        {
            if (instructionsContainer == null || instructionsContainerParent == null) return;

            float containerHeight = instructionsContainer.sizeDelta.y;
            float maxHeight = instructionsContainerParent.rect.height;

            if (containerHeight <= maxHeight)
            {
                interpolate = false;
                return;
            }

            interpolate = true;
            interpolationState = 0.0f;

            interpolationStartPosition = instructionsContainer.anchoredPosition.y;

            if (literal)
            {
                interpolationEndPosition = position;
                return;
            }

            var minimumPosition = interpolationStartPosition + interpolationEdgeDistance;
            var maximumPosition = interpolationStartPosition + maxHeight - interpolationEdgeDistance;

            interpolationEndPosition = interpolationStartPosition;
            if (position < minimumPosition)
            {
                interpolationEndPosition -= (minimumPosition - position);
            }
            if (position > maximumPosition)
            {
                interpolationEndPosition += (position - maximumPosition);
            }
        }
    }
}
