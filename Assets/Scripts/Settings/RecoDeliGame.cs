using RecoDeli.Scripts.Level;
using RecoDeli.Scripts.Level.Format;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        private void OnEnable()
        {
            hideFlags &= ~HideFlags.NotEditable;

            // make sure to reload level object prefabs at the start
            LevelObjectData.LoadLevelObjectPrefabs();
        }

        public void SaveConfiguration()
        {
            Save(true);
        }

        static RecoDeliGame()
        {
            // this is needed to prevent comma from being used as decimal indicator in some countries.
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public static void OpenLevel(string levelName)
        {
            LevelLoader.LevelToLoad = levelName;
            SceneManager.LoadScene(GameplaySceneName);
        }

        public static void OpenSimpleLevelList()
        {
            SceneManager.LoadScene(SimpleMapListSceneName);
        }
    }
}
