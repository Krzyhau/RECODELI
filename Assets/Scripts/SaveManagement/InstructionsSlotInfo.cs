using RecoDeli.Scripts.Gameplay.Robot;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RecoDeli.Scripts.SaveManagement
{
    [Serializable]
    public struct InstructionsSlotInfo
    {
        public bool Completed;
        public int RecordedInstructionCount;
        public float RecordedTime;

        public string InstructionsData;

        [XmlIgnore]
        public List<RobotInstruction> Instructions
        {
            get => RobotInstruction.StringToList(InstructionsData);
            set => InstructionsData = RobotInstruction.ListToString(value);
        }
    }
}