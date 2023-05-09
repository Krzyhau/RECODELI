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
            for(float t = 0; t < instruction.Parameter; t += Time.fixedDeltaTime)
            {
                yield return new WaitForFixedUpdate();
                var accelerationStep = Mathf.Min(Time.fixedDeltaTime, instruction.Parameter - t);
                var flyingForce = controller.transform.forward * movementDirection * controller.PropulsionForce * accelerationStep;
                controller.Rigidbody.AddForce(flyingForce, ForceMode.VelocityChange);

                instruction.UpdateProgress(t / instruction.Parameter);
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
