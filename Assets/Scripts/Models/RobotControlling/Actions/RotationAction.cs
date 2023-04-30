using System.Collections;
using UnityEngine;
using LudumDare.Scripts.Components;
using LudumDare.Scripts.Utils;

namespace LudumDare.Scripts.Models
{
    public abstract class RotationAction : RobotAction
    {
        private float rotationDirection;

        public RotationAction(float param, float direction) : base(param) 
        {
            rotationDirection = direction;
        }

        public override IEnumerator Execute(RobotController controller)
        {
            var currentRotation = controller.transform.eulerAngles.y;
            var deltaRotation = parameter * rotationDirection * controller.RotationSpeed;

            for (float t = 0; t < parameter; t += Time.fixedDeltaTime)
            {
                yield return new WaitForFixedUpdate();

                var t1 = controller.RotationCurve.Evaluate(t / parameter);
                var t2 = controller.RotationCurve.Evaluate((t + Time.fixedDeltaTime) / parameter);
                var desiredAngle = deltaRotation * t1;
                var desiredNextAngle = deltaRotation * t2;
                var desiredAngularVelocity = (desiredNextAngle - desiredAngle) / Time.fixedDeltaTime;
                controller.Rigidbody.angularVelocity = new Vector3(0, Mathf.Deg2Rad * desiredAngularVelocity, 0);
            }
        }
    }
}
