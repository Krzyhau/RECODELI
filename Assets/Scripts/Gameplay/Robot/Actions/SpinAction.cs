using BEPUphysics.Unity;
using BEPUutilities;
using RecoDeli.Scripts.Prototyping;
using SoftFloat;
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

            var parameter = (sfloat)instruction.Parameter;
            var absParameter = sfloat.Abs(parameter);
            var direction = (sfloat)rotationDirection * (sfloat)parameter.Sign();

            for (sfloat t = sfloat.Zero; t < absParameter; t += deltaTime)
            {
                var accelerationStep = sfloat.Min(deltaTime, absParameter - t);
                var rotationAcceleration = (direction * (sfloat)controller.RotationSpeed) * accelerationStep;
                robotEntity.AngularVelocity += new BEPUutilities.Vector3(sfloat.Zero, MathHelper.ToRadians(rotationAcceleration), sfloat.Zero);

                if (absParameter - t < deltaTime) break;
                yield return 1;
                instruction.UpdateProgress((float)(t / absParameter));
            }
        }
    }
}
