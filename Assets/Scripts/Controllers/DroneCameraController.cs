using System;
using System.Collections.Generic;
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

        public Vector2 BoundariesOffset;
        public Vector2 BoundariesSize;

        [Header("Inputs")]
        [SerializeField] private InputActionReference pointerInput;
        [SerializeField] private InputActionReference heldInput;
        [SerializeField] private InputActionReference manualControlInput;
        [SerializeField] private InputActionReference fasterInput;
        [SerializeField] private InputActionReference zoomInput;
        [SerializeField] private InputActionReference followRobotInput;
        [SerializeField] private InputActionReference followPackageInput;

        private Vector3 heldPlanePosition;
        private Vector3 preinterpolatedCameraPosition;
        private bool pressed;
        private bool panning;
        private Transform followedObject;
        private bool followRobot = false;
        private bool followPackage = false;
        public bool FollowingRobot => followRobot;
        public bool FollowingPackage => followPackage;

        private void Start()
        {
            preinterpolatedCameraPosition = controlledCamera.transform.position;
            followRobotInput.action.performed += ctx => FollowRobot();
            followPackageInput.action.performed += ctx => FollowPackage();
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

            Clamping();

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

            if(manualControl.magnitude > 0 && followedObject != null)
            {
                FollowObject(null);
            }
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

        private void Clamping()
        {
            var lookPos = GetScreenPosOnXZPlane(new Vector2((Screen.width - simulationManager.Interface.InstructionEditorWidth) / 2, Screen.height / 2));
            var offset = lookPos - preinterpolatedCameraPosition;
            var offset2d = new Vector2(offset.x, offset.z);
            var minBounds = BoundariesOffset - BoundariesSize - offset2d;
            var maxBounds = BoundariesOffset + BoundariesSize - offset2d;

            preinterpolatedCameraPosition.x = Mathf.Clamp(preinterpolatedCameraPosition.x, minBounds.x, maxBounds.x);
            preinterpolatedCameraPosition.z = Mathf.Clamp(preinterpolatedCameraPosition.z, minBounds.y, maxBounds.y);
        }

        private Vector3 GetMousePosOnXZPlane()
        {
            return GetScreenPosOnXZPlane(pointerInput.action.ReadValue<Vector2>());
        }

        private Vector3 GetScreenPosOnXZPlane(Vector2 position)
        {
            var screenPosition = (Vector3)position + Vector3.forward;

            var cameraPosition = controlledCamera.transform.position;
            var worldDirection = controlledCamera.ScreenToWorldPoint(screenPosition) - cameraPosition;
            var worldPosition = preinterpolatedCameraPosition + worldDirection * (preinterpolatedCameraPosition.y / -worldDirection.y);
            return worldPosition;
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var points = new List<Vector3>();
            for(int x = -1; x <= 1; x += 2) for(int y = -1; y <= 1; y += 2)
            {
                points.Add(new(BoundariesOffset.x + BoundariesSize.x * x, 0, BoundariesOffset.y + BoundariesSize.y * y * x));
            }

            Gizmos.DrawLineStrip(points.ToArray(), true);
        }
    }
}