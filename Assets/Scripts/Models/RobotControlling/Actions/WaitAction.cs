using UnityEngine;
using System.Collections;
using LudumDare.Scripts.Components;

namespace LudumDare.Scripts.Models
{
    public class WaitAction : RobotActionSingle
    {
        public override string Name => "WAIT";
        public override RobotThrusterFlag ThrustersState => RobotThrusterFlag.None;

        public override IEnumerator Execute(RobotController controller, float parameter)
        {
            float remainingTime = parameter;
            while(remainingTime > 0)
            {
                yield return new WaitForFixedUpdate();
                remainingTime -= Time.fixedDeltaTime;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
