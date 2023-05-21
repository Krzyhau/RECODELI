namespace RecoDeli.Scripts.Gameplay.Robot
{
    public class ForwardAction : MovementAction
    {
        public override string Name => "FORWARD";

        public ForwardAction() : base(1.0f)
        {
        }
    }
}