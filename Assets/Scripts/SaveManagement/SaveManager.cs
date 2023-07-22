using UnityEngine;

namespace RecoDeli.Scripts.SaveManagement
{
    public static class SaveManager
    {
        private static bool loaded;
        private static SaveData save;

        public static SaveData CurrentSave => save;

        private static void Initialize()
        {
            Load();
            Application.quitting += Save;
        }

        public static void Load()
        {

        }

        public static void Save()
        {

        }
    }
}
