using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnRightAction : RobotAction
{
    public string Name => "TURN RIGHT";

    public IEnumerator Execute(RobotController controller, float value)
    {
        controller.YawVelocity = -controller.RotationSpeed;
        yield return new WaitForSeconds(value);
        controller.YawVelocity = 0.0f;
    }
}
