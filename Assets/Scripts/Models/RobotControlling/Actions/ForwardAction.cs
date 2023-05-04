namespace LudumDare.Scripts.Models
{
    public class ForwardAction : MovementAction
    {
        public override string Name => "FORWARD";
        public override RobotThrusterFlag ThrustersState => RobotThrusterFlag.BackRight | RobotThrusterFlag.BackLeft;

        public ForwardAction() : base(1.0f)
        {
        }
    }
}