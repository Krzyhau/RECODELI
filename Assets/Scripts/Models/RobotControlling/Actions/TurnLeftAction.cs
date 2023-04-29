using System.Collections;
using UnityEngine;
using LudumDare.Scripts.Components;

namespace LudumDare.Scripts.Models
{
    public class TurnLeftAction : RobotAction
    {
        public override string Name => "TURN LEFT";

        public TurnLeftAction(float param) : base(param) { }

        public override IEnumerator Execute(RobotController controller)
        {
            controller.YawVelocity = controller.RotationSpeed;
            yield return new WaitForSeconds(parameter);
            controller.YawVelocity = 0.0f;
        }
    }
}
