using BEPUphysics.Unity;
using RecoDeli.Scripts.Prototyping;
using SoftFloat;
using System.Collections;
using System.Collections.Generic;
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

        public override IEnumerator<int> Execute(RobotController controller, RobotInstruction<float> instruction)
        {
            var robotEntity = controller.Rigidbody.Entity;

            var deltaTime = controller.Rigidbody.Simulation.TimeStep;

            var parameter = (sfloat)instruction.Parameter;
            var absParameter = sfloat.Abs(parameter);
            var finalMovementDir = (sfloat)parameter.Sign() * (sfloat)movementDirection;
            for (sfloat t = sfloat.Zero; t < absParameter; t += deltaTime)
            {
                yield return 1;

                var accelerationStep = sfloat.Min(deltaTime, absParameter - t);
                var flyingForce = robotEntity.OrientationMatrix.Forward * finalMovementDir * (sfloat)controller.PropulsionForce * accelerationStep;
                robotEntity.LinearVelocity += flyingForce;
                
                robotEntity.AngularVelocity = BEPUutilities.Vector3.MoveTowards(
                    robotEntity.AngularVelocity,
                    BEPUutilities.Vector3.Zero,
                    (sfloat)controller.PropulsionRotationDrag * deltaTime
                );
                instruction.UpdateProgress((float)(t / absParameter));
            }
        }
    }
}
