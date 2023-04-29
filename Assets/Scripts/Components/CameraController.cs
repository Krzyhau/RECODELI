using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixelArt
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera controlledCamera;

        [SerializeField] private float minimumZoomDistance;
        [SerializeField] private float maximumZoomDistance;
        [SerializeField] private float scrollZoomScale;
        [SerializeField] private float zoomDistanceInterpolation;
        [SerializeField] private float orbitingScale;
        [SerializeField] private float transitionTime;


        private float zoomDistance;
        private bool orbiting;
        private bool panning;
        private bool transitioning;
        private Vector3 panningInitialMouse;
        private Vector3 orbitingInitialMouse;
        private Vector3 orbitingInitialEuler;

        private IEnumerator lastTransitionCoroutine;

        private void Start()
        {
            zoomDistance = controlledCamera.transform.localPosition.z * -1.0f;
        }

        private void Update()
        {
            HandleZooming();
            HandleOrbiting();
            HandlePanning();

            if (Input.GetKeyDown(KeyCode.Keypad0)) Recenter();

            if (Input.GetKeyDown(KeyCode.Keypad4)) Snap(new Vector3(0.0f, 45.0f));
            if (Input.GetKeyDown(KeyCode.Keypad6)) Snap(new Vector3(0.0f, -45.0f));
            if (Input.GetKeyDown(KeyCode.Keypad8)) Snap(new Vector3(45.0f, 0.0f));
            if (Input.GetKeyDown(KeyCode.Keypad2)) Snap(new Vector3(-45.0f, 0.0f));
        }

        private void HandleZooming()
        {
            var scroll = Input.mouseScrollDelta.y;
            while (scroll > 0)
            {
                scroll--;
                zoomDistance /= scrollZoomScale;
                if (zoomDistance < minimumZoomDistance)
                {
                    zoomDistance = minimumZoomDistance;
                    scroll = 0;
                }
            }
            while (scroll < 0)
            {
                scroll++;
                zoomDistance *= scrollZoomScale;
                if (zoomDistance > maximumZoomDistance)
                {
                    zoomDistance = maximumZoomDistance;
                    scroll = 0;
                }
            }

            var zPos = Mathf.Lerp(
                controlledCamera.transform.localPosition.z,
                zoomDistance * -1.0f,
                zoomDistanceInterpolation * Time.deltaTime
            );
            controlledCamera.transform.localPosition = new Vector3(0, 0, zPos);
        }


        private void HandleOrbiting()
        {
            if (Input.GetMouseButtonDown(2))
            {
                orbiting = true;
                orbitingInitialMouse = Input.mousePosition;
                orbitingInitialEuler = transform.localEulerAngles;
            }
            if (!Input.GetMouseButton(2) || transitioning)
            {
                orbiting = false;
            }

            if (!orbiting) return;

            var mouseDifference = (Input.mousePosition - orbitingInitialMouse) * orbitingScale;
            var newRotation = orbitingInitialEuler + new Vector3(-mouseDifference.y, mouseDifference.x, 0);

            var oldPitch = newRotation.x;
            newRotation.x = Mathf.Clamp((oldPitch + 180.0f) % 360.0f - 180.0f, -89.0f, 89.0f);
            orbitingInitialEuler.x += Mathf.DeltaAngle(oldPitch, newRotation.x);

            transform.localEulerAngles = newRotation;
        }

        private void HandlePanning()
        {
            if (Input.GetMouseButtonDown(0))
            {
                panning = true;
                var firstMousePosition = Input.mousePosition;
                firstMousePosition.z = zoomDistance;
                panningInitialMouse = controlledCamera.ScreenToWorldPoint(firstMousePosition);
            }
            if (!Input.GetMouseButton(0) || transitioning)
            {
                panning = false;
            }

            if (!panning) return;

            var checkedMousePosition = Input.mousePosition;
            checkedMousePosition.z = zoomDistance;
            var currentMouse = controlledCamera.ScreenToWorldPoint(checkedMousePosition);

            transform.position -= (currentMouse - panningInitialMouse);
        }

        private IEnumerator TransitionCoroutine(Vector3 targetPosition, Quaternion targetRotation)
        {
            if (!transitioning)
            {
                transitioning = true;

                var initialPosition = transform.position;
                var initialRotation = transform.rotation;

                for (float time = 0.0f; time <= 1.0f; time += Time.deltaTime / transitionTime)
                {
                    float t = (1.0f - Mathf.Cos(time * Mathf.PI)) * 0.5f;

                    transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
                    transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);

                    yield return 0;
                }

                transitioning = false;
            }
        }

        private void Transition(Vector3 position, Quaternion eulerAngles)
        {
            orbiting = false;
            panning = false;
            if (transitioning)
            {
                StopCoroutine(lastTransitionCoroutine);
                transitioning = false;
            }
            lastTransitionCoroutine = TransitionCoroutine(position, eulerAngles);
            StartCoroutine(lastTransitionCoroutine);
        }


        public void Recenter()
        {
            Transition(Vector3.zero, transform.rotation);
        }

        public void Snap(Vector3 eulerOffset)
        {
            // create new euler angles with pitch clamped
            Vector3 eulerAngles = transform.localEulerAngles + eulerOffset;
            eulerAngles.x = Mathf.Clamp((eulerAngles.x + 180.0f) % 360.0f - 180.0f, -89f, 89f);

            // round them up to the nearest increment
            var increment = 45.0f;
            eulerAngles.x = Mathf.Round(eulerAngles.x / increment) * increment;
            eulerAngles.y = Mathf.Round(eulerAngles.y / increment) * increment;

            // just in case, reset roll
            eulerAngles.z = 0.0f;

            Transition(transform.position, Quaternion.Euler(eulerAngles));
        }
    }

}