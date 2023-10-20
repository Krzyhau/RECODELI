using RecoDeli.Scripts.Level;
using RecoDeli.Scripts.Level.Format;
using RecoDeli.Scripts.SaveManagement;
using System.Globalization;
using UnityEngine.SceneManagement;

namespace RecoDeli.Scripts.Settings
{
    public static class RecoDeliGame
    {
        public static RecoDeliSettings Settings => RecoDeliSettings.Instance;

        static RecoDeliGame()
        {
            SettingsProvider.ApplySettings();

            // this is needed to prevent comma from being used as decimal indicator in some countries.
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // make sure to reload level object prefabs at the start
            LevelObjectData.LoadLevelObjectPrefabs();

            SaveManager.Initialize();

        }

        public static void OpenLevel(string levelName)
        {
            LevelLoader.LevelToLoad = levelName;
            SceneManager.LoadScene(Settings.GameplaySceneName);
        }

        public static void OpenSimpleLevelList()
        {
            SceneManager.LoadScene(Settings.SimpleMapListSceneName);
        }
    }
}
