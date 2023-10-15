using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RecoDeli.Scripts.Controllers
{
    public class DroneCameraController : MonoBehaviour
    {
        [SerializeField] private Camera controlledCamera;
        [SerializeField] private SimulationManager simulationManager;

        [Header("Settings")]
        [SerializeField] private float zoomScrollDistance;
        [SerializeField] private float zoomNearLimit;
        [SerializeField] private float zoomFarLimit;
        [SerializeField] private float manualPanningSpeed;
        [SerializeField] private float manualPanningFasterSpeed;
        [SerializeField] private float cameraInterpolationFactor;

        [Header("Inputs")]
        [SerializeField] private InputActionReference pointerInput;
        [SerializeField] private InputActionReference heldInput;
        [SerializeField] private InputActionReference manualControlInput;
        [SerializeField] private InputActionReference fasterInput;
        [SerializeField] private InputActionReference zoomInput;

        private Vector3 heldPlanePosition;
        private Vector3 preinterpolatedCameraPosition;
        private bool pressed;
        private bool panning;
        private Transform followedObject;
        private bool followRobot = false;
        private bool followPackage = false;
        private RectTransform instructionsEditorRect;

        private void Start()
        {
            preinterpolatedCameraPosition = controlledCamera.transform.position;
        }

        private void Update()
        {
            Panning();
            Zooming();

            if (followRobot) followedObject = simulationManager.RobotController.transform;
            if (followPackage) followedObject = simulationManager.GoalBox.transform;

            if (followedObject != null)
            {
                var currentY = preinterpolatedCameraPosition.y;
                preinterpolatedCameraPosition = followedObject.position;

                var displacementDelta = (currentY - preinterpolatedCameraPosition.y) * controlledCamera.transform.forward;

                preinterpolatedCameraPosition -= displacementDelta;
                preinterpolatedCameraPosition.y = currentY;
            }

            controlledCamera.transform.position = Vector3.Lerp(
                controlledCamera.transform.position,
                preinterpolatedCameraPosition,
                cameraInterpolationFactor * Time.unscaledDeltaTime
            );
        }

        private void Panning()
        {
            var pressedNow = heldInput.action.IsPressed();
            if(pressed != pressedNow)
            {
                pressed = pressedNow;
                if (pressed && !EventSystem.current.IsPointerOverGameObject())
                {
                    heldPlanePosition = GetMousePosOnXZPlane();
                    panning = true;
                    FollowObject(null);
                }
                if (!pressed)
                {
                    panning = false;
                }
            }

            if (panning)
            {
                var currentHeldPlanePosition = GetMousePosOnXZPlane();
                var difference = heldPlanePosition - currentHeldPlanePosition;
                preinterpolatedCameraPosition += difference;
            }

            var manualControl2D = manualControlInput.action.ReadValue<Vector2>();
            var manualControl = new Vector3(manualControl2D.x, 0, manualControl2D.y);

            manualControl *= fasterInput.action.IsPressed() ? manualPanningFasterSpeed : manualPanningSpeed;

            preinterpolatedCameraPosition += manualControl;
        }

        private void Zooming()
        {
            float zoomDistance = 0.0f;

            var scrolling = zoomInput.action.ReadValue<float>();

            if (scrolling > 0f)
            {
                zoomDistance += zoomScrollDistance;
            }

            else if (scrolling < 0f)
            {
                zoomDistance -= zoomScrollDistance;
            }

            if (zoomDistance == 0.0f || EventSystem.current.IsPointerOverGameObject()) return;

            var hoveredPlanePosition = GetMousePosOnXZPlane();

            var cameraForward = controlledCamera.transform.forward;
            var minimumZoomDistance = (zoomFarLimit - preinterpolatedCameraPosition.y) / cameraForward.y;
            var maximumZoomDistance = (zoomNearLimit - preinterpolatedCameraPosition.y) / cameraForward.y;
            preinterpolatedCameraPosition += cameraForward * Mathf.Clamp(zoomDistance, minimumZoomDistance, maximumZoomDistance);

            var newHoveredPlanePosition = GetMousePosOnXZPlane();
            var difference = hoveredPlanePosition - newHoveredPlanePosition;
            preinterpolatedCameraPosition += difference;
        }

        private Vector3 GetMousePosOnXZPlane()
        {
            var mouseScreenPosition = (Vector3)pointerInput.action.ReadValue<Vector2>() + Vector3.forward;

            var cameraPosition = controlledCamera.transform.position;
            var mouseWorldDirection = controlledCamera.ScreenToWorldPoint(mouseScreenPosition) - cameraPosition;
            var mouseWorldPosition = preinterpolatedCameraPosition + mouseWorldDirection * (preinterpolatedCameraPosition.y / -mouseWorldDirection.y);
            return mouseWorldPosition;
        }

        public void FollowRobot()
        {
            followRobot = true;
            followPackage = false;
        }

        public void FollowPackage()
        {
            followPackage = true;
            followRobot = false;
        }

        public void FollowObject(GameObject go)
        {
            followRobot = false;
            followPackage = false;
            followedObject = go != null ? go.transform : null;
        }
    }
}