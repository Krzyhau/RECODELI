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
            var currentVelocity = controller.Rigidbody.velocity;
            var velocityBoost = movementDirection * parameter * controller.PropulsionForce;
            var desiredVelocity = currentVelocity + controller.transform.forward * velocityBoost;
            
            for(float t = 0; t < parameter; t += Time.fixedDeltaTime)
            {
                yield return new WaitForFixedUpdate();
                controller.Rigidbody.velocity = Vector3.Lerp(currentVelocity, desiredVelocity, t / parameter);
            }
        }
    }
}
