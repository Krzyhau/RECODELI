using RecoDeli.Scripts.Level.Format;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace RecoDeli.Scripts.Settings
{
    [FilePath("ProjectSettings/RecoDeli.asset", FilePathAttribute.Location.ProjectFolder)]
    [Serializable]
    public class RecoDeliGame : ScriptableSingleton<RecoDeliGame>
    {
        [SerializeField] private string gameplaySceneName = "Gameplay";
        [SerializeField] private string simpleMapListSceneName = "Simple Map List";

        [SerializeField] private string levelFormatExtension = ".rdlvl";
        [SerializeField] private string levelObjectPrefabsPath = "Prefabs/Level Objects/";
        [SerializeField] private string levelsDirectoryPath = "Levels/";

        public static string GameplaySceneName => instance.gameplaySceneName;
        public static string SimpleMapListSceneName => instance.simpleMapListSceneName;
        public static string LevelFormatExtension => instance.levelFormatExtension;
        public static string LevelObjectPrefabsPath => instance.levelObjectPrefabsPath;
        public static string LevelsDirectoryPath => instance.levelsDirectoryPath;

        static RecoDeliGame()
        {
            // this is needed to prevent comma from being used as decimal indicator in some countries.
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }
        private void OnEnable()
        {
            hideFlags &= ~HideFlags.NotEditable;

            // make sure to reload level object prefabs at the start
            LevelObjectData.LoadLevelObjectPrefabs(); 
        }

        public void Save()
        {
            Save(true);
        }
    }
}
