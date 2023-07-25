using System;
using System.Collections.Generic;
using System.Linq;

namespace RecoDeli.Scripts.SaveManagement
{
    [Serializable]
    public class SaveData
    {
        public float PlayTime;

        public List<SaveLevelInfo> LevelInfos;

        public SaveData() 
        {
            LevelInfos = new();
        }

        public SaveLevelInfo GetLevelInfo(string levelName)
        {
            var level = LevelInfos.Where(info => info.LevelName == levelName).FirstOrDefault();
            if(level == null)
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

        public bool IsLevelComplete(string levelName)
        {
            var level = GetLevelInfo(levelName);
            
            return level != null ? level.Completed : false;
        }
    }
}
