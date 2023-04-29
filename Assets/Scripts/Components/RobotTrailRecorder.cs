using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare.Scripts.Components
{
    public class RobotTrailRecorder : MonoBehaviour
    {
        [SerializeField] private RobotController controller;
        [SerializeField] private float distanceThreshold;

        private bool recording;
        private Vector3 lastPosition = Vector3.zero;

        public bool Recording => recording;

        private List<Vector3> tracePoints = new List<Vector3>();
        private LineRenderer lineRenderer;

        private void Start()
        {
            // TODO: Zenject the shit out of the Robot Controller

            lineRenderer = GetComponent<LineRenderer>();

            StartRecording();
        }


        private void FixedUpdate()
        {
            if (recording)
            {
                var currentPoint = controller.transform.position;
                if ((currentPoint - lastPosition).magnitude > distanceThreshold)
                {
                    AddPoint(currentPoint);
                    lastPosition = currentPoint;
                }
            }
        }

        private void AddPoint(Vector3 point)
        {
            point.y = transform.position.y;
            tracePoints.Add(point);
            RefreshLine();
        }

        private void RefreshLine()
        {
            lineRenderer.positionCount = tracePoints.Count;
            lineRenderer.SetPositions(tracePoints.ToArray());
        }

        // TODO: Call this function when simulation has stopped.
        public void StopRecording()
        {
            recording = false;
        }

        // TODO: Call this function when simulation has started
        public void StartRecording()
        {
            tracePoints.Clear();
            AddPoint(controller.transform.position);
            recording = true;
        }
    }
}
