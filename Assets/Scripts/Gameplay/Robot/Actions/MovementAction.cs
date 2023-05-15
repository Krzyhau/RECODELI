using RecoDeli.Scripts.Prototyping;
using System.Collections;
using UnityEngine;

namespace RecoDeli.Scripts.Gameplay.Robot
{
    public abstract class MovementAction : RobotActionSingle
    {
        private float movementDirection;

        public MovementAction(float direction) : base()
        { 
            movementDirection = direction;
        }

        public override IEnumerator Execute(RobotController controller, RobotInstruction<float> instruction)
        {
            var absParameter = Mathf.Abs(instruction.Parameter);
            var finalMovementDir = Mathf.Sign(instruction.Parameter) * movementDirection;
            for (float t = 0; t < absParameter; t += Time.fixedDeltaTime)
            {
                yield return new WaitForFixedUpdate();
                var accelerationStep = Mathf.Min(Time.fixedDeltaTime, absParameter - t);
                var flyingForce = controller.transform.forward * finalMovementDir * controller.PropulsionForce * accelerationStep;
                controller.Rigidbody.AddForce(flyingForce, ForceMode.VelocityChange);

                
                controller.Rigidbody.angularVelocity = Vector3.MoveTowards(
                    controller.Rigidbody.angularVelocity,
                    Vector3.zero,
                    controller.PropulsionRotationDrag * Time.fixedDeltaTime
                );
                

                instruction.UpdateProgress(t / absParameter);
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
