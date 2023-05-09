using RecoDeli.Scripts.Prototyping;
using System.Collections;
using UnityEngine;

namespace RecoDeli.Scripts.Gameplay.Robot
{
    public abstract class RotationAction : RobotActionSingle
    {

        private float rotationDirection;

        public RotationAction(float direction) : base() 
        {
            rotationDirection = direction;
        }

        public override IEnumerator Execute(RobotController controller, RobotInstruction<float> instruction)
        {
            var parameter = instruction.Parameter;
            var deltaRotation = parameter * rotationDirection * controller.RotationSpeed;

            for (float t = 0; t < parameter; t += Time.fixedDeltaTime)
            {
                if (RotationMethodSelector.ShouldUseFreeMethod)
                {
                    var accelerationStep = Mathf.Min(Time.fixedDeltaTime, parameter - t);
                    var rotationAcceleration = (2.0f * rotationDirection * controller.RotationSpeed) * accelerationStep;
                    controller.Rigidbody.angularVelocity += new Vector3(0.0f, Mathf.Deg2Rad * rotationAcceleration, 0.0f);
                }
                else
                {
                    var t1 = controller.RotationCurve.Evaluate(t / parameter);
                    var t2 = controller.RotationCurve.Evaluate(Mathf.Min(1.0f, (t + Time.fixedDeltaTime) / parameter));
                    var desiredAngle = deltaRotation * t1;
                    var desiredNextAngle = deltaRotation * t2;
                    var desiredAngularVelocity = (desiredNextAngle - desiredAngle) / Time.fixedDeltaTime;
                    controller.Rigidbody.angularVelocity = new Vector3(0, Mathf.Deg2Rad * desiredAngularVelocity, 0);
                }

                if (parameter - t < Time.fixedDeltaTime) break;
                yield return new WaitForFixedUpdate();
                instruction.UpdateProgress(t / parameter);
            }

            if (!RotationMethodSelector.ShouldUseFreeMethod)
            {
                controller.Rigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
}
