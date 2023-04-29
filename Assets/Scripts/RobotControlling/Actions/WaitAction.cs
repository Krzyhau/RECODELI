using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitAction : RobotAction
{
    public override string Name => "WAIT";

    public WaitAction(float param) : base(param) { }

    public override IEnumerator Execute(RobotController controller)
    {
        yield return new WaitForSeconds(parameter);
    }
}
