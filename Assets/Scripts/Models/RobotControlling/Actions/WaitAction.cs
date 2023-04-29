using System.Collections;
using UnityEngine;
using LudumDare.Scripts.Components;

namespace LudumDare.Scripts.Models
{
    public class WaitAction : RobotAction
    {
        public override string Name => "WAIT";

        public WaitAction(float param) : base(param) { }

        public override IEnumerator Execute(RobotController controller)
        {
            yield return new WaitForSeconds(parameter);
        }
    }
}
