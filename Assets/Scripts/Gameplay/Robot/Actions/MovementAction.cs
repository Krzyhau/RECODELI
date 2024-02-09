using BEPUphysics.Unity;
using RecoDeli.Scripts.Prototyping;
using BEPUutilities.FixedMath;
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

            var parameter = (fint)instruction.Parameter;
            var absParameter = fint.Abs(parameter);
            var finalMovementDir = fint.Sign(parameter) * (fint)movementDirection;
            for (fint t = (fint)0.0001f; t < absParameter; t += deltaTime)
            {
                yield return 1;

                var accelerationStep = fint.Min(deltaTime, absParameter - t);
                var flyingForce = robotEntity.OrientationMatrix.Forward * finalMovementDir * (fint)controller.PropulsionForce * accelerationStep;
                robotEntity.LinearVelocity += flyingForce;
                
                robotEntity.AngularVelocity = BEPUutilities.Vector3.MoveTowards(
                    robotEntity.AngularVelocity,
                    BEPUutilities.Vector3.Zero,
                    (fint)controller.PropulsionRotationDrag * deltaTime
                );
                instruction.UpdateProgress((float)(t / absParameter));
            }
        }

        public override string GetParameterInputSuffix(int parameterIndex) => "sec";
    }
}
