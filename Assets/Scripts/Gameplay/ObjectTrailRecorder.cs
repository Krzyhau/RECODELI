using RecoDeli.Scripts.Gameplay.Robot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecoDeli.Scripts.Gameplay
{
    public class ObjectTrailRecorder : MonoBehaviour
    {
        [SerializeField] private float distanceThreshold;

        private bool recording;
        private Vector3 lastPosition = Vector3.zero;
        private Transform recordedObject;
        private List<Vector3> tracePoints = new List<Vector3>();
        private LineRenderer lineRenderer;

        public bool Recording => recording;
        public Transform RecordedObject => recordedObject;

        
        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }


        private void FixedUpdate()
        {
            if (!recording || recordedObject == null) return;
            

            var currentPoint = recordedObject.position;
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
            recordedObject = null;
        }

        public void StartRecording(Transform objectToRecord)
        {
            recordedObject = objectToRecord;
            tracePoints.Clear();
            AddPoint(recordedObject.position);
            recording = true;
        }
    }
}
