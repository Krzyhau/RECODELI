using BEPUphysics.Unity;
using BEPUutilities;
using RecoDeli.Scripts.Prototyping;
using BEPUutilities.FixedMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecoDeli.Scripts.Gameplay.Robot
{
    public abstract class SpinAction : RobotActionSingle
    {
        private float rotationDirection;

        public SpinAction(float direction) : base()
        {
            rotationDirection = direction;
        }

        public override IEnumerator<int> Execute(RobotController controller, RobotInstruction<float> instruction)
        {
            var robotEntity = controller.Rigidbody.Entity;
            var deltaTime = controller.Rigidbody.Simulation.TimeStep;

            var parameter = (fint)instruction.Parameter;
            var absParameter = fint.Abs(parameter);
            var direction = (fint)rotationDirection * fint.Sign(parameter);

            for (fint t = (fint)0; t < absParameter; t += deltaTime)
            {
                var accelerationStep = fint.Min(deltaTime, absParameter - t);
                var rotationAcceleration = (direction * (fint)controller.RotationSpeed) * accelerationStep;
                robotEntity.AngularVelocity += new BEPUutilities.Vector3((fint)0, MathHelper.ToRadians(rotationAcceleration), (fint)0);

                if (absParameter - t < deltaTime) break;
                yield return 1;
                instruction.UpdateProgress((float)(t / absParameter));
            }
        }
    }
}
