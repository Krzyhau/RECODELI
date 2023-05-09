namespace RecoDeli.Scripts.Gameplay.Robot
{
    public class BackwardAction : MovementAction
    {
        public override string Name => "BACKWARD";
        public override RobotThrusterFlag ThrustersState => RobotThrusterFlag.FrontRight | RobotThrusterFlag.FrontLeft;

        public BackwardAction() : base(-1.0f)
        {
        }
    }
}