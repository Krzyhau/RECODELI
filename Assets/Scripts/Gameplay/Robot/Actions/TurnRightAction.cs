namespace RecoDeli.Scripts.Gameplay.Robot
{
    public class TurnRightAction : RotationAction
    {
        public override string Name => "TURN RIGHT";
        public override RobotThrusterFlag ThrustersState => RobotThrusterFlag.FrontRight | RobotThrusterFlag.BackLeft;

        public TurnRightAction() : base(1.0f, true)
        {
        }
    }
}
