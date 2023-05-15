namespace RecoDeli.Scripts.Gameplay.Robot
{
    public class SpinRightAction : RotationAction
    {
        public override string Name => "SPIN RIGHT";
        public override RobotThrusterFlag ThrustersState => RobotThrusterFlag.FrontLeft | RobotThrusterFlag.BackRight;

        public SpinRightAction() : base(1.0f, false) 
        { 
        }
    }
}
