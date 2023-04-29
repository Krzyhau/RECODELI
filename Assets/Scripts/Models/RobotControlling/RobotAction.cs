using LudumDare.Scripts.Components;
using System.Collections;

namespace LudumDare.Scripts.Models
{
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
}

