using RecoDeli.Scripts.Gameplay.Robot;
using System.Collections.Generic;

namespace RecoDeli.Scripts.SaveManagement
{
    public struct InstructionsSlotInfo
    {
        public bool Completed;
        public int RecordedInstructionCount;
        public float RecordedTime;

        public string InstructionsData;

        public List<RobotInstruction> Instructions
        {
            get => RobotInstruction.StringToList(InstructionsData);
            set => InstructionsData = RobotInstruction.ListToString(value);
        }
    }
}