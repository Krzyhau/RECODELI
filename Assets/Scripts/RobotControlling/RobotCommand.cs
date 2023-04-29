
using System.Collections;

public class RobotCommand
{
    public RobotAction Action;
    public float Parameter;

    public IEnumerator Execute(RobotController controller)
    {
        yield return Action.Execute(controller, Parameter);
    }
}

