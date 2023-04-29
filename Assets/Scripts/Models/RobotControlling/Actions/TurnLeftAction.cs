namespace LudumDare.Scripts.Models
{
    public class TurnLeftAction : RotationAction
    {
        public override string Name => "TURN LEFT";

        public override RobotThrusterFlag Thrusters => RobotThrusterFlag.FrontRight | RobotThrusterFlag.BackLeft;

        public TurnLeftAction(float param) : base(param, 1.0f) 
        { 
        }
    }
}
