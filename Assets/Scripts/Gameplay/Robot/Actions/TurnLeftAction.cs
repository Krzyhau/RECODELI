namespace RecoDeli.Scripts.Gameplay.Robot
{
    public class TurnLeftAction : RotationAction
    {
        public override string Name => "TURN LEFT";

        public TurnLeftAction() : base(-1.0f) 
        { 
        }
    }
}
