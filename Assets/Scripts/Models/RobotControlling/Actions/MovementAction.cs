using System.Collections;
using UnityEngine;
using LudumDare.Scripts.Components;

namespace LudumDare.Scripts.Models
{
    public abstract class MovementAction : RobotAction
    {
        private float movementDirection;

        public MovementAction(float param, float direction) : base(param) 
        { 
            movementDirection = direction;
        }

        public override IEnumerator Execute(RobotController controller)
        {
            for(float t = 0; t < parameter; t += Time.fixedDeltaTime)
            {
                yield return new WaitForFixedUpdate();
                var accelerationStep = Mathf.Min(Time.fixedDeltaTime, parameter - t);
                var flyingForce = controller.transform.forward * movementDirection * controller.PropulsionForce * accelerationStep;
                controller.Rigidbody.AddForce(flyingForce, ForceMode.VelocityChange);
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
