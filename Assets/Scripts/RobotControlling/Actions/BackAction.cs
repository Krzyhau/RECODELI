using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAction : RobotAction
{
    public string Name => "BACK";

    public IEnumerator Execute(RobotController controller, float value)
    {
        yield return 0;
    }
}
