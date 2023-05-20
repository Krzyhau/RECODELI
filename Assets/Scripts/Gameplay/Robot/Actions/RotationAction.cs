using BEPUphysics.Unity;
using BEPUutilities;
using RecoDeli.Scripts.Prototyping;
using SoftFloat;
using System.Collections;
using System.Collections.Generic;
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

        public override IEnumerator<int> Execute(RobotController controller, RobotInstruction<float> instruction)
        {
            var robotEntity = controller.Rigidbody.Entity;
            var deltaTime = controller.Rigidbody.Simulation.TimeStep;
            var rawParameter = (sfloat)instruction.Parameter;

            var direction = (sfloat)rotationDirection * (sfloat)rawParameter.Sign();
            var parameter = libm.sqrtf(sfloat.Abs(rawParameter) * sfloat.Two);

            var stepCounts = (parameter / deltaTime);

            sfloat t = deltaTime;

            // robot needs to rotate with certain velocity at given time in order to stop perfectly at given angle
            for (sfloat step = sfloat.Zero; step <= stepCounts + sfloat.One; step += sfloat.One)
            {
                sfloat currentAngularVelocity = MathHelper.ToDegrees(robotEntity.AngularVelocity.Y);
                sfloat desiredAngularVelocity = direction * (sfloat)controller.RotationSpeed * sfloat.Max(sfloat.Zero, parameter - t);

                sfloat newAngularVelocity = MathHelper.MoveTowards(currentAngularVelocity, desiredAngularVelocity, (sfloat)controller.RotationSpeed * deltaTime);
                robotEntity.AngularVelocity = new BEPUutilities.Vector3(sfloat.Zero, MathHelper.ToRadians(newAngularVelocity), sfloat.Zero);


                yield return 1;
                instruction.UpdateProgress((float)(step / stepCounts));
                t += deltaTime;
            }
        }
    }
}
