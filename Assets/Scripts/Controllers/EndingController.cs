using BEPUphysics.Unity;
using RecoDeli.Scripts.Gameplay;
using RecoDeli.Scripts.Gameplay.Robot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RecoDeli.Scripts.Controllers
{
    public class EndingController : MonoBehaviour
    {
        [SerializeField] private float animationLength;
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private AnimationCurve hudBlinkingCurve;
        [SerializeField] private DroneCameraController controlledCamera;
        [SerializeField] private CanvasGroup mainUiGroupToHide;

        [SerializeField] private UnityEvent OnEndingAnimationDone;

        private RobotController robot;
        private GoalBox goalBox;

        private Vector3 robotStartPosition;
        private Quaternion robotStartRotation;
        private Quaternion goalBoxStartRotation;

        private Vector3 startCameraPosition;
        private Quaternion startCameraRotation;

        private Vector3 robotTargetPosition;
        private Quaternion robotTargetRotation;
        private Quaternion goalBoxTargetRotation;

        private Vector3 desiredCameraPosition;
        private Quaternion desiredCameraRotation;

        private bool started = false;
        private float animationState = 0.0f;

        public bool IsAnimating => started && animationState < 1.0f;
        public bool EndingInProgress => started;

        private void Update()
        {
            if (!EndingInProgress) return;

            if (animationState >= 1.0f) return;

            animationState += Time.unscaledDeltaTime / animationLength;

            if (animationState >= 1.0f) {
                OnEndingAnimationDone.Invoke();
                animationState = 1.0f;
            }

            var t = animationCurve.Evaluate(animationState);

            robot.transform.position = Vector3.Lerp(robotStartPosition, robotTargetPosition, t);
            robot.transform.rotation = Quaternion.Lerp(robotStartRotation, robotTargetRotation, t);
            goalBox.transform.rotation = Quaternion.Lerp(goalBoxStartRotation, goalBoxTargetRotation, t);

            controlledCamera.transform.position = Vector3.Lerp(startCameraPosition, desiredCameraPosition, t);
            controlledCamera.transform.rotation = Quaternion.Lerp(startCameraRotation, desiredCameraRotation, t);

            mainUiGroupToHide.alpha = 1.0f - hudBlinkingCurve.Evaluate(animationState);
        }

        public void StartEnding(RobotController robot, GoalBox goalBox)
        {
            this.robot = robot;
            this.goalBox = goalBox;

            robotStartPosition = robot.transform.position;
            robotStartRotation = robot.transform.rotation;
            goalBoxStartRotation = goalBox.transform.rotation;

            robotTargetPosition = goalBox.transform.position + Vector3.up * 3f;
            robotTargetRotation = Quaternion.Euler(0, 180, 0);
            goalBoxTargetRotation = Quaternion.identity;

            startCameraPosition = controlledCamera.transform.position;
            startCameraRotation = controlledCamera.transform.rotation;

            desiredCameraPosition = robotTargetPosition + new Vector3(0, 3, -10);
            desiredCameraRotation = Quaternion.Euler(30, 0, 0);

            robot.StopExecution();
            robot.Rigidbody.Kinematic = true;
            robot.Rigidbody.Entity.LinearVelocity = BEPUutilities.Vector3.Zero;
            robot.Rigidbody.Entity.AngularVelocity = BEPUutilities.Vector3.Zero;
            var goalBoxRigid = goalBox.GetComponent<BepuRigidbody>();
            goalBoxRigid.Kinematic = true;
            goalBoxRigid.Entity.LinearVelocity = BEPUutilities.Vector3.Zero;
            goalBoxRigid.Entity.AngularVelocity = BEPUutilities.Vector3.Zero;

            controlledCamera.enabled = false;

            robot.enabled = false;

            mainUiGroupToHide.interactable = false;

            animationState = 0.0f;
            started = true;
        }

        public void RevertEnding()
        {
            if (!started) return;

            controlledCamera.transform.position = startCameraPosition;
            controlledCamera.transform.rotation = startCameraRotation;

            // don't have to reset robot and goalbox because that's part of the simulation

            mainUiGroupToHide.interactable = true;
            mainUiGroupToHide.alpha = 1.0f;

            controlledCamera.enabled = true;
            started = false;
            animationState = 0.0f;
        }
    }
}
