using System.Collections;

public interface RobotAction
{
    public string Name { get; }

    public IEnumerator Execute(RobotController controller, float value);
}
