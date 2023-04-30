using LudumDare.Scripts.Components;
using System.Collections;

namespace LudumDare.Scripts.Models
{
    public abstract class RobotAction
    {
        protected float parameter;

        public abstract RobotThrusterFlag Thrusters { get; }
        public abstract IEnumerator Execute(RobotController controller);

        public RobotAction(float param)
        {
            parameter = param;
        }

        public static RobotAction CreateFromName(string name, float param)
        {
            switch (name.ToUpper())
            {
                case "WAIT": return new WaitAction(param);
                case "FORWARD": return new ForwardAction(param);
                case "BACKWARD": return new BackwardAction(param);
                case "TURN LEFT": return new TurnLeftAction(param);
                case "TURN RIGHT": return new TurnRightAction(param);
                default: return null;
            }
        }
    }
}

