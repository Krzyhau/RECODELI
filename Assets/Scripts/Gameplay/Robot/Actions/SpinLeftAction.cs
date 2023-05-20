namespace RecoDeli.Scripts.Gameplay.Robot
{
    public class SpinLeftAction : SpinAction
    {
        public override string Name => "SPIN LEFT";
        public override RobotThrusterFlag ThrustersState => RobotThrusterFlag.FrontLeft | RobotThrusterFlag.BackRight;

        public SpinLeftAction() : base(-1.0f) 
        { 
        }
    }
}
