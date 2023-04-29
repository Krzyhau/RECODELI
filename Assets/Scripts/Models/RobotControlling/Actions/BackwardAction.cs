namespace LudumDare.Scripts.Models
{
    public class BackwardAction : MovementAction
    {
        public override string Name => "BACKWARD";

        public override RobotThrusterFlag Thrusters => RobotThrusterFlag.FrontRight | RobotThrusterFlag.FrontLeft;

        public BackwardAction(float param) : base(param, -1.0f)
        {
        }
    }
}