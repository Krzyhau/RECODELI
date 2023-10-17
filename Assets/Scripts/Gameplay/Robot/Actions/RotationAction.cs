using BEPUphysics.Unity;
using BEPUutilities;
using RecoDeli.Scripts.Prototyping;
using BEPUutilities.FixedMath;
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
            var rawParameter = (fint)instruction.Parameter;

            var direction = (fint)rotationDirection * fint.Sign(rawParameter);
            var parameter = fint.Sqrt(fint.Abs(rawParameter) * (fint)2) - deltaTime;

            var stepCounts = (parameter / deltaTime);

            fint t = deltaTime;

            // robot needs to rotate with certain velocity at given time in order to stop perfectly at given angle
            for (fint step = (fint)0; step <= stepCounts + (fint)1; step += (fint)1)
            {
                fint currentAngularVelocity = MathHelper.ToDegrees(robotEntity.AngularVelocity.Y);
                fint desiredAngularVelocity = direction * (fint)controller.RotationSpeed * fint.Max((fint)0, parameter - t);

                fint newAngularVelocity = MathHelper.MoveTowards(currentAngularVelocity, desiredAngularVelocity, (fint)controller.RotationSpeed * deltaTime);
                robotEntity.AngularVelocity = new BEPUutilities.Vector3((fint)0, MathHelper.ToRadians(newAngularVelocity), (fint)0);


                yield return 1;
                instruction.UpdateProgress((float)(step / stepCounts));
                t += deltaTime;
            }
        }
    }
}
