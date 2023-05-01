using UnityEngine;
using System.Collections;
using LudumDare.Scripts.Components;

namespace LudumDare.Scripts.Models
{
    public class WaitAction : RobotAction
    {
        public override RobotThrusterFlag Thrusters => RobotThrusterFlag.None;

        public WaitAction(float param) : base(param) 
        { 
        }

        public override IEnumerator Execute(RobotController controller)
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
