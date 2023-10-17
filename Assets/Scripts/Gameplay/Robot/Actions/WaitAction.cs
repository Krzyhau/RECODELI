using UnityEngine;
using System.Collections;
using BEPUutilities.FixedMath;
using BEPUphysics.Unity;
using System.Collections.Generic;

namespace RecoDeli.Scripts.Gameplay.Robot
{
    public class WaitAction : RobotActionSingle
    {
        public override string Name => "WAIT";

        public override IEnumerator<int> Execute(RobotController controller, RobotInstruction<float> instruction)
        {
            fint remainingTime = (fint)instruction.Parameter;
            while(remainingTime > (fint)0)
            {
                yield return 1;
                remainingTime -= controller.Rigidbody.Simulation.TimeStep;
                instruction.UpdateProgress(1.0f - (float)(remainingTime / (fint)instruction.Parameter));
            }
        }
    }
}
