namespace LudumDare.Scripts.Models
{
    public class TurnLeftAction : RotationAction
    {
        public override RobotThrusterFlag Thrusters => RobotThrusterFlag.FrontLeft | RobotThrusterFlag.BackRight;

        public TurnLeftAction(float param) : base(param, -1.0f) 
        { 
        }
    }
}
