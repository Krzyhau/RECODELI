using System;

namespace LudumDare.Scripts.Models
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
