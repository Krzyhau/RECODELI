using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare.Scripts.Components
{
    public class RobotTrailRecorder : MonoBehaviour
    {
        [SerializeField] private float distanceThreshold;

        private bool recording;
        private Vector3 lastPosition = Vector3.zero;
        private RobotController controller;

        public bool Recording => recording;

        private List<Vector3> tracePoints = new List<Vector3>();
        private LineRenderer lineRenderer;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }


        private void FixedUpdate()
        {
            if (!recording || controller==null) return;
            

            var currentPoint = controller.transform.position;
            if ((currentPoint - lastPosition).magnitude > distanceThreshold)
            {
                AddPoint(currentPoint);
                lastPosition = currentPoint;
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

        public void StopRecording()
        {
            recording = false;
            controller = null;
        }

        public void StartRecording(RobotController controller)
        {
            this.controller = controller;
            tracePoints.Clear();
            AddPoint(controller.transform.position);
            recording = true;
        }
    }
}
