using UnityEngine;
using System.Collections;

namespace RecoDeli.Scripts.Gameplay.Robot
{
    public class WaitAction : RobotActionSingle
    {
        public override string Name => "WAIT";
        public override RobotThrusterFlag ThrustersState => RobotThrusterFlag.None;

        public override IEnumerator Execute(RobotController controller, RobotInstruction<float> instruction)
        {
            float remainingTime = instruction.Parameter;
            while(remainingTime > 0)
            {
                yield return new WaitForFixedUpdate();
                remainingTime -= Time.fixedDeltaTime;
                instruction.UpdateProgress(1.0f - remainingTime / instruction.Parameter);
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
