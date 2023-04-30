namespace LudumDare.Scripts.Models
{
    public class TurnRightAction : RotationAction
    {
        public override RobotThrusterFlag Thrusters => RobotThrusterFlag.FrontRight | RobotThrusterFlag.BackLeft;

        public TurnRightAction(float param) : base(param, 1.0f)
        {
        }
    }
}
