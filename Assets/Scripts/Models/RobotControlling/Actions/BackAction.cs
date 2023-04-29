using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAction : RobotAction
{
    public override string Name => "BACK";

    public BackAction(float param) : base(param) { }

    public override IEnumerator Execute(RobotController controller)
    {
        controller.CurrentPropellingForce = -controller.PropulsionForce;
        yield return new WaitForSeconds(parameter);
        controller.CurrentPropellingForce = 0.0f;
    }
}
