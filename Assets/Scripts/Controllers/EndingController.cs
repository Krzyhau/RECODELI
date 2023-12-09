using BEPUphysics.Unity;
using RecoDeli.Scripts.Settings;
using RecoDeli.Scripts.UI;
using System.Collections;
using UnityEngine;

namespace RecoDeli.Scripts.Controllers
{
    public class EndingController : MonoBehaviour
    {
        [SerializeField] private SimulationManager simulationManager;

        [SerializeField] private Vector3 packageOffset;
        [SerializeField] private Vector3 endingCameraOffset;
        [SerializeField] private Vector3 endingCameraEulerAngles;
        [SerializeField] private float animationLength;
        [SerializeField] private AnimationCurve animationCurve;

        private Vector3 robotStartPosition;
        private Quaternion robotStartRotation;
        private Vector3 goalBoxStartLocalPosition;
        private Quaternion goalBoxStartLocalRotation;
        private Vector3 cameraStartPosition;
        private Quaternion cameraStartRotation;

        private Vector3 robotTargetPosition;
        private Quaternion robotTargetRotation;
        private Vector3 goalBoxTargetLocalPosition;
        private Quaternion goalBoxTargetLocalRotation;
        private Vector3 cameraTargetPosition;
        private Quaternion cameraTargetRotation;

        private bool started = false;
        //private bool finalizing = false;
        private float animationFirstPhaseState = 0.0f;

        public bool EndingInProgress => started;
        public bool IsAnimating => EndingInProgress && animationFirstPhaseState < 1.0f;

        public SimulationManager SimulationManager => simulationManager;

        private void Update()
        {
            AnimateFirstPhase();
        }

        private void AnimateFirstPhase()
        {
            if (!IsAnimating) return;

            animationFirstPhaseState += Time.unscaledDeltaTime / animationLength;

            if (animationFirstPhaseState >= 1.0f)
            {
                animationFirstPhaseState = 1.0f;
            }

            var t = animationCurve.Evaluate(animationFirstPhaseState);

            simulationManager.RobotController.transform.position = Vector3.Lerp(robotStartPosition, robotTargetPosition, t);
            simulationManager.RobotController.transform.rotation = Quaternion.Lerp(robotStartRotation, robotTargetRotation, t);
            simulationManager.GoalBox.transform.localPosition = Vector3.Lerp(goalBoxStartLocalPosition, goalBoxTargetLocalPosition, t);
            simulationManager.GoalBox.transform.localRotation = Quaternion.Lerp(goalBoxStartLocalRotation, goalBoxTargetLocalRotation, t);

            simulationManager.DroneCamera.transform.position = Vector3.Lerp(cameraStartPosition, cameraTargetPosition, t);
            simulationManager.DroneCamera.transform.rotation = Quaternion.Lerp(cameraStartRotation, cameraTargetRotation, t);
        }

        public void StartEnding()
        {
            var robot = simulationManager.RobotController;
            var goalBox = simulationManager.GoalBox;

            goalBox.transform.parent = robot.ModelAnimator.transform;

            robotStartPosition = robot.transform.position;
            robotStartRotation = robot.transform.rotation;
            goalBoxStartLocalPosition = goalBox.transform.localPosition;
            goalBoxStartLocalRotation = goalBox.transform.localRotation;

            robotTargetPosition = goalBox.transform.position - packageOffset;
            robotTargetRotation = Quaternion.Euler(0, 180, 0);
            goalBoxTargetLocalPosition = packageOffset;
            goalBoxTargetLocalRotation = Quaternion.identity;

            cameraStartPosition = simulationManager.DroneCamera.transform.position;
            cameraStartRotation = simulationManager.DroneCamera.transform.rotation;

            cameraTargetPosition = robotTargetPosition + endingCameraOffset;
            cameraTargetRotation = Quaternion.Euler(endingCameraEulerAngles);

            robot.StopExecution();
            robot.Rigidbody.Kinematic = true;
            robot.Rigidbody.Entity.LinearVelocity = BEPUutilities.Vector3.Zero;
            robot.Rigidbody.Entity.AngularVelocity = BEPUutilities.Vector3.Zero;
            var goalBoxRigid = goalBox.GetComponent<BepuRigidbody>();
            goalBoxRigid.Kinematic = true;
            goalBoxRigid.Entity.LinearVelocity = BEPUutilities.Vector3.Zero;
            goalBoxRigid.Entity.AngularVelocity = BEPUutilities.Vector3.Zero;

            simulationManager.DroneCamera.enabled = false;

            //robot.enabled = false;
            robot.ModelAnimator.SetTrigger("Celebrate");

            simulationManager.Interface.ShowEndingInterface(true);

            animationFirstPhaseState = 0.0f;
            started = true;
        }

        public void RevertEnding()
        {
            if (!EndingInProgress) return;

            // just have to reset the camera
            // all game entities will be reset with the simulation

            simulationManager.DroneCamera.transform.position = cameraStartPosition;
            simulationManager.DroneCamera.transform.rotation = cameraStartRotation;

            simulationManager.DroneCamera.enabled = true;

            simulationManager.Interface.ShowEndingInterface(false);


            started = false;
            animationFirstPhaseState = 0.0f;
        }

        public void FinalizeEnding()
        {
            StartCoroutine(FinalizeEndingCoroutine());
        }

        private IEnumerator FinalizeEndingCoroutine()
        {
            var robot = simulationManager.RobotController;
            var endingInterface = simulationManager.Interface.EndingInterface;
            endingInterface.FinalizeEndingInterface();
            yield return new WaitForSeconds(0.5f);
            robot.ModelAnimator.SetTrigger("Launching");
            yield return new WaitForSeconds(2.5f);
            RecoDeliGame.OpenMainMenuFromGameplay();
        }
    }
}
