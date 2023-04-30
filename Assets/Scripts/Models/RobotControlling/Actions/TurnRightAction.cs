namespace LudumDare.Scripts.Models
{
    public class TurnRightAction : RotationAction
    {
        public override RobotThrusterFlag Thrusters => RobotThrusterFlag.FrontLeft | RobotThrusterFlag.BackRight;

        public TurnRightAction(float param) : base(param, -1.0f)
        {
        }
    }
}
