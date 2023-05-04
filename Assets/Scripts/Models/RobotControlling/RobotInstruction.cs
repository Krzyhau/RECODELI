using LudumDare.Scripts.Components;
using System.Collections;

namespace LudumDare.Scripts.Models
{
    public abstract class RobotInstruction
    {
        public IRobotAction Action { get; private set; }

        public RobotInstruction(IRobotAction action)
        {
            Action = action;
        }

        public abstract IEnumerator Execute(RobotController controller);
    }

    public class RobotInstruction<T> : RobotInstruction
    {
        public T Parameter { get; set; }

        public RobotInstruction(IRobotAction<T> action, T parameter) : base(action)
        {
            Parameter = parameter;
        }

        public override IEnumerator Execute(RobotController controller)
        {
            var typedAction = (IRobotAction<T>)Action;
            yield return typedAction.Execute(controller, Parameter);
        }
    }
}
