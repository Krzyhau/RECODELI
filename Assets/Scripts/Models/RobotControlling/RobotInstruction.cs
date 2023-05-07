using LudumDare.Scripts.Components;
using System;
using System.Collections;

namespace LudumDare.Scripts.Models
{
    public abstract class RobotInstruction
    {
        public RobotAction Action { get; private set; }

        public RobotInstruction(RobotAction action)
        {
            Action = action;
        }

        public abstract IEnumerator Execute(RobotController controller);

        public abstract string[] ParameterToStrings();
        public abstract void SetParameterFromStrings(string[] paramStrings);
    }

    public class RobotInstruction<T> : RobotInstruction
    {
        public T Parameter { get; set; }

        public RobotInstruction(RobotAction<T> action) : base(action)
        {
        }
        public RobotInstruction(RobotAction<T> action, T parameter) : base(action)
        {
            Parameter = parameter;
        }

        public override IEnumerator Execute(RobotController controller)
        {
            yield return ((RobotAction<T>)Action).Execute(controller, Parameter);
        }

        public override string[] ParameterToStrings()
        {
            return ((RobotAction<T>)Action).ParameterToStrings(Parameter);
        }

        public override void SetParameterFromStrings(string[] paramStrings)
        {
            Parameter = ((RobotAction<T>)Action).ParameterFromStrings(paramStrings);
        }
    }
}
