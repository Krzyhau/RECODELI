using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitAction : RobotAction
{
    public string Name => "WAIT";

    public IEnumerator Execute(RobotController controller, float value)
    {
        yield return new WaitForSeconds(value);
    }
}
