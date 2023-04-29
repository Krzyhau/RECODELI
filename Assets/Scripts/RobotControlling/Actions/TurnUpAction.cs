using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnUpAction : RobotAction
{
    public string Name => "TURN UP";

    public IEnumerator Execute(RobotController controller, float value)
    {
        controller.PitchVelocity = controller.RotationSpeed;
        yield return new WaitForSeconds(value);
        controller.PitchVelocity = 0.0f;
    }
}
