namespace RecoDeli.Scripts.Gameplay.Robot
{
    public class BackwardAction : MovementAction
    {
        public override string Name => "BACKWARD";

        public BackwardAction() : base(-1.0f)
        {
        }
    }
}