using System.Collections;
using UnityEngine;
using LudumDare.Scripts.Components;

namespace LudumDare.Scripts.Models
{
    public class TurnRightAction : RobotAction
    {
        public override string Name => "TURN RIGHT";

        public TurnRightAction(float param) : base(param) { }

        public override IEnumerator Execute(RobotController controller)
        {
            controller.YawVelocity = -controller.RotationSpeed;
            yield return new WaitForSeconds(parameter);
            controller.YawVelocity = 0.0f;
        }
    }
}
