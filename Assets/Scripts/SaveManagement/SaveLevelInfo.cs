using System;

namespace RecoDeli.Scripts.SaveManagement
{
    [Serializable]
    public class SaveLevelInfo
    {
        public string LevelName;

        public int ExecutionCount;

        public bool Completed;
        public float FastestTime;
        public int LowestInstructions;

        public InstructionsSlotInfo[] Slots;

        public SaveLevelInfo() 
        {
            Slots = new InstructionsSlotInfo[3];
            Completed = false;
            LevelName = "";
        }
    }
}
