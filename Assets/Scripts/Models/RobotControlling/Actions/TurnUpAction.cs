using System.Collections;
using UnityEngine;
using LudumDare.Scripts.Components;

namespace LudumDare.Scripts.Models
{
    public class TurnUpAction : RobotAction
    {
        public override string Name => "TURN UP";

        public TurnUpAction(float param) : base(param) { }

        public override IEnumerator Execute(RobotController controller)
        {
            controller.PitchVelocity = controller.RotationSpeed;
            yield return new WaitForSeconds(parameter);
            controller.PitchVelocity = 0.0f;
        }
    }
}
