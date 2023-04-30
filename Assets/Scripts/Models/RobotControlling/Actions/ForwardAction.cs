namespace LudumDare.Scripts.Models
{
    public class ForwardAction : MovementAction
    {
        public override RobotThrusterFlag Thrusters => RobotThrusterFlag.BackRight | RobotThrusterFlag.BackLeft;

        public ForwardAction(float param) : base(param, 1.0f)
        {
        }
    }
}