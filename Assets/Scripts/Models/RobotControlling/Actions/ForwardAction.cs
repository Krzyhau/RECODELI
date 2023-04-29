using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardAction : RobotAction
{
    public override string Name => "FORWARD";

    public ForwardAction(float param) : base(param) { }

    public override IEnumerator Execute(RobotController controller)
    {
        controller.CurrentPropellingForce = controller.PropulsionForce;
        yield return new WaitForSeconds(parameter);
        controller.CurrentPropellingForce = 0.0f;
    }
}
