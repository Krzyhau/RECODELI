using RecoDeli.Scripts.Prototyping;
using System.Collections;
using UnityEngine;

namespace RecoDeli.Scripts.Gameplay.Robot
{
    public abstract class RotationAction : RobotActionSingle
    {

        private float rotationDirection;
        private bool autoRotationMode;

        public RotationAction(float direction, bool autoRotation) : base() 
        {
            rotationDirection = direction;
            autoRotationMode = autoRotation;
        }

        public override IEnumerator Execute(RobotController controller, RobotInstruction<float> instruction)
        {
            if (autoRotationMode)
            {
                yield return AutoRotationMode(controller, instruction);
                yield break;
            }

            var parameter = instruction.Parameter;

            for (float t = 0; t < parameter; t += Time.fixedDeltaTime)
            {
                var accelerationStep = Mathf.Min(Time.fixedDeltaTime, parameter - t);
                var rotationAcceleration = (rotationDirection * controller.RotationSpeed) * accelerationStep;
                controller.Rigidbody.angularVelocity += new Vector3(0.0f, Mathf.Deg2Rad * rotationAcceleration, 0.0f);

                if (parameter - t < Time.fixedDeltaTime) break;
                yield return new WaitForFixedUpdate();
                instruction.UpdateProgress(t / parameter);
            }
        }

        private IEnumerator AutoRotationMode(RobotController controller, RobotInstruction<float> instruction)
        {
            var parameter = Mathf.Sqrt(instruction.Parameter * 2.0f);

            var stepCounts = (parameter / Time.fixedDeltaTime);

            float t = Time.fixedDeltaTime;

            // robot needs to rotate with certain velocity at given time in order to stop perfectly at given angle
            for (int step = 0; step <= stepCounts + 1; step++)
            {
                float currentAngularVelocity = controller.Rigidbody.angularVelocity.y * Mathf.Rad2Deg;
                float desiredAngularVelocity = rotationDirection * controller.RotationSpeed * Mathf.Max(0.0f, parameter - t);

                float newAngularVelocity = Mathf.MoveTowards(currentAngularVelocity, desiredAngularVelocity, controller.RotationSpeed * Time.fixedDeltaTime);
                controller.Rigidbody.angularVelocity = new Vector3(0, Mathf.Deg2Rad * newAngularVelocity, 0);

                yield return new WaitForFixedUpdate();
                instruction.UpdateProgress(step / stepCounts);
                t += Time.fixedDeltaTime;
            }
        }
    }
}
