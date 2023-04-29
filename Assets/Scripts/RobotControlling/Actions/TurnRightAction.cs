using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
