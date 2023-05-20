using UnityEngine;
using System.Collections;
using SoftFloat;
using BEPUphysics.Unity;
using System.Collections.Generic;

namespace RecoDeli.Scripts.Gameplay.Robot
{
    public class WaitAction : RobotActionSingle
    {
        public override string Name => "WAIT";
        public override RobotThrusterFlag ThrustersState => RobotThrusterFlag.None;

        public override IEnumerator<int> Execute(RobotController controller, RobotInstruction<float> instruction)
        {
            sfloat remainingTime = (sfloat)instruction.Parameter;
            while(remainingTime > sfloat.Zero)
            {
                yield return 1;
                remainingTime -= controller.Rigidbody.Simulation.TimeStep;
                instruction.UpdateProgress(1.0f - (float)(remainingTime / (sfloat)instruction.Parameter));
            }
        }
    }
}
