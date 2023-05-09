namespace LudumDare.Scripts.Models
{
    public class TurnLeftAction : RotationAction
    {
        public override string Name => "TURN LEFT";
        public override RobotThrusterFlag ThrustersState => RobotThrusterFlag.FrontLeft | RobotThrusterFlag.BackRight;

        public TurnLeftAction() : base(-1.0f) 
        { 
        }
    }
}
