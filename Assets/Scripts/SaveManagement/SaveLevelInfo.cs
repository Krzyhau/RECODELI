namespace RecoDeli.Scripts.SaveManagement
{
    public struct SaveLevelInfo
    {
        public string LevelName;

        public int ExecutionCount;

        public bool Completed;
        public float FastestTime;
        public int LowestInstructions;

        public InstructionsSlotInfo[] InstructionsSlots;
    }
}
