using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardAction : RobotAction
{
    public string Name => "FORWARD";

    public IEnumerator Execute(RobotController controller, float value)
    {
        yield return 0;
    }
}
