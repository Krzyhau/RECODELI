using System;

namespace RecoDeli.Scripts.Gameplay.Robot
{
    [Flags]
    public enum RobotThrusterFlag
    {
        None = 0,
        FrontLeft = 1,
        FrontRight = 2,
        BackLeft = 4,
        BackRight = 8,
    }
}
