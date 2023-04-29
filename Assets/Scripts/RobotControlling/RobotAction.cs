using System.Collections;

public abstract class RobotAction
{
    protected float parameter;

    public abstract string Name { get; }
    public abstract IEnumerator Execute(RobotController controller);

    public RobotAction(float param)
    {
        parameter = param;
    }
}
