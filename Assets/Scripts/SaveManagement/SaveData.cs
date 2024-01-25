using RecoDeli.Scripts.Level;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RecoDeli.Scripts.SaveManagement
{
    [Serializable]
    public class SaveData
    {
        public string Name;
        public float PlayTime;

        public List<SaveLevelInfo> LevelInfos;

        public int CompletedLevelsCount => LevelInfos.Where(level => level.Completed).Count();

        public SaveData() 
        {
            LevelInfos = new();
        }

        public SaveLevelInfo GetCurrentLevelInfo()
        {
            return GetLevelInfoOrCreate(LevelLoader.CurrentlyLoadedLevel);
        }

        public SaveLevelInfo GetLevelInfoOrCreate(string levelName)
        {
            var level = GetLevelInfo(levelName);
            if (level == null)
            {
                level = new SaveLevelInfo();
                level.LevelName = levelName;

                if (levelName != null && levelName.Length > 0)
                {
                    LevelInfos.Add(level);
                }
            }
            return level;
        }

        public SaveLevelInfo GetLevelInfo(string levelName)
        {
            return LevelInfos.Where(info => info.LevelName == levelName).FirstOrDefault();
        }

        public bool IsLevelComplete(string levelName)
        {
            var level = GetLevelInfo(levelName);
            
            return level != null ? level.Completed : false;
        }
    }
}
