using LudumDare.Scripts.Components;
using System.Collections;

namespace LudumDare.Scripts.Models
{
    public interface IRobotAction
    {
        public string Name { get; }
        public RobotThrusterFlag ThrustersState { get; }
    }

    public interface IRobotAction<T> : IRobotAction
    {
        public IEnumerator Execute(RobotController controller, T parameter);
    }
}

