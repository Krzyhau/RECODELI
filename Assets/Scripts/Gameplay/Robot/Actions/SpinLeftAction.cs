namespace RecoDeli.Scripts.Gameplay.Robot
{
    public class SpinLeftAction : RotationAction
    {
        public override string Name => "SPIN LEFT";
        public override RobotThrusterFlag ThrustersState => RobotThrusterFlag.FrontLeft | RobotThrusterFlag.BackRight;

        public SpinLeftAction() : base(-1.0f, false) 
        { 
        }
    }
}
