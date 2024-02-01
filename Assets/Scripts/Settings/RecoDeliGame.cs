using RecoDeli.Scripts.Assets.Scripts.Leaderboards;
using RecoDeli.Scripts.Leaderboards;
using RecoDeli.Scripts.Level;
using RecoDeli.Scripts.Level.Format;
using RecoDeli.Scripts.SaveManagement;
using RecoDeli.Scripts.UI;
using RecoDeli.Scripts.Utils;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RecoDeli.Scripts.Settings
{
    public static class RecoDeliGame
    {
        public static RecoDeliSettings Settings => RecoDeliSettings.Instance;

        static RecoDeliGame()
        {
            MainThreadExecutor.Initialize(System.Threading.SynchronizationContext.Current);

            UserSettingsProvider.ApplySettings();

            // this is needed to prevent comma from being used as decimal indicator in some countries.
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // make sure to reload level object prefabs at the start
            LevelObjectData.LoadLevelObjectPrefabs();

            SaveManager.Initialize();

        }

        // empty, but will call static constructor automatically when needed
        public static void Initialize() { } 

        public static void OpenLevel(string levelName)
        {
            LevelLoader.LevelToLoad = levelName;
            SceneManager.LoadScene(Settings.GameplaySceneName);
        }

        public static void OpenMainMenuFromGameplay()
        {
            MainMenuInterface.StartInTaskMenu = true;
            SceneManager.LoadScene(Settings.MainMenuSceneName);
        }

        public static LeaderboardProvider CreateNewLeaderboardProvider(string levelName)
        {
            return new SaveBasedLeaderboardProvider(levelName);
        }

        public static void QuitThisFuckingPieceOfShitImmediately()
        {
#if UNITY_STANDALONE
            Application.Quit();
#endif
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
