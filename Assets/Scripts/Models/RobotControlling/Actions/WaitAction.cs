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
            yield return new WaitForSeconds(parameter);
        }
    }
}
