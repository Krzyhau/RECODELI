using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnLeftAction : RobotAction
{
    public string Name => "TURN LEFT";

    public IEnumerator Execute(RobotController controller, float value)
    {
        controller.YawVelocity = controller.RotationSpeed;
        yield return new WaitForSeconds(value);
        controller.YawVelocity = 0.0f;
    }
}
