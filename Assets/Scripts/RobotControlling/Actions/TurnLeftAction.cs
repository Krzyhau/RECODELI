using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
