using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LudumDare.Scripts.Components
{
    public class DroneCameraController : MonoBehaviour
    {
        [SerializeField] private Camera controlledCamera;
        [SerializeField] private SimulationManager simulationManager;

        [SerializeField] private float zoomScrollDistance;
        [SerializeField] private float zoomNearLimit;
        [SerializeField] private float zoomFarLimit;
        [SerializeField] private float manualPanningSpeed;
        [SerializeField] private float manualPanningFasterSpeed;
        [SerializeField] private float cameraInterpolationFactor;

        private Vector3 heldPlanePosition;
        private Vector3 preinterpolatedCameraPosition;
        private bool panning;
        private bool followingRobot;

        private void Start()
        {
            preinterpolatedCameraPosition = controlledCamera.transform.position;
        }

        private void Update()
        {
            Panning();
            Zooming();

            if (followingRobot && simulationManager.RobotController != null)
            {
                var currentY = preinterpolatedCameraPosition.y;
                preinterpolatedCameraPosition = simulationManager.RobotController.transform.position;

                var displacementDelta = (currentY - preinterpolatedCameraPosition.y) * controlledCamera.transform.forward;

                preinterpolatedCameraPosition -= displacementDelta;
                preinterpolatedCameraPosition.y = currentY;
                preinterpolatedCameraPosition += controlledCamera.transform.right * currentY * 0.25f;
            }

            controlledCamera.transform.position = Vector3.Lerp(
                controlledCamera.transform.position,
                preinterpolatedCameraPosition,
                cameraInterpolationFactor * Time.unscaledDeltaTime
            );
        }

        private void Panning()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                heldPlanePosition = GetMousePosOnXZPlane();
                panning = true;
                followingRobot = false;
            }
            if (!Input.GetMouseButton(0))
            {
                panning = false;
            }

            if (!panning) return;


            var currentHeldPlanePosition = GetMousePosOnXZPlane();
            var difference = heldPlanePosition - currentHeldPlanePosition;

            var manualControl = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) difference.z += 1;
            if (Input.GetKey(KeyCode.S)) difference.z -= 1;
            if (Input.GetKey(KeyCode.A)) difference.x -= 1;
            if (Input.GetKey(KeyCode.D)) difference.x += 1;

            manualControl *= (Input.GetKey(KeyCode.LeftShift)) ? manualPanningFasterSpeed : manualPanningSpeed;

            preinterpolatedCameraPosition += difference + manualControl;
        }

        private void Zooming()
        {
            float zoomDistance = 0.0f;

            if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus))
            {
                zoomDistance += zoomScrollDistance;
            }

            else if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(KeyCode.Minus))
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
            var mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = 1;

            var cameraPosition = controlledCamera.transform.position;
            var mouseWorldDirection = controlledCamera.ScreenToWorldPoint(mouseScreenPosition) - cameraPosition;
            var mouseWorldPosition = preinterpolatedCameraPosition + mouseWorldDirection * (preinterpolatedCameraPosition.y / -mouseWorldDirection.y);
            return mouseWorldPosition;
        }

        public void FollowRobot()
        {
            followingRobot = true;
        }
    }
}