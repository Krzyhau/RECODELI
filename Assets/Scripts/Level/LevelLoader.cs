using BEPUphysics.Paths;
using BEPUphysics.Unity;
using RecoDeli.Scripts.Controllers;
using RecoDeli.Scripts.Level;
using RecoDeli.Scripts.Level.Format;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RecoDeli.Scripts.Level
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] protected SimulationManager simulationManager;
        [SerializeField] protected BepuSimulation levelContainer;

        [SerializeField] private string levelPath;

        public string LevelEditorPath { get => levelPath; set => levelPath = value; }
        public static string LevelToLoad { get; set; }

        public bool IsEmptyLevel => (LevelToLoad == "" || LevelToLoad == null) && levelContainer.transform.childCount == 0;

        private void Awake()
        {
            if (!Application.isPlaying) return;

            LoadLevel();
        }

        public static string GetLevelsDirectoryPath()
        {
            return $"{Application.dataPath}{(Application.isEditor ? "/Resources" : "")}/{LevelFormatSettings.LevelsDirectoryPath}";
        }

        public virtual void SaveCurrentLevel()
        {
            if (levelPath == null || levelPath.Length == 0) return;

            var levelData = new LevelData();
            levelData.Info = new LevelInfo();
            levelData.Info.CameraPosition = simulationManager.DroneCamera.transform.position;

            foreach (Transform child in levelContainer.transform)
            {
                var objectData = new LevelObjectData(child.gameObject);
                if (objectData.IsValid) levelData.Objects.Add(objectData);
            }

            var levelText = levelData.ToXML();

            var filePath = $"{GetLevelsDirectoryPath()}{levelPath}{LevelFormatSettings.Extension}";

            File.WriteAllText(filePath, levelText);

            Debug.Log($"Level {levelPath} has been saved.");
        }

        private bool TryLoadLevelString(string path, out string level)
        {
            var resourcePath = $"{LevelFormatSettings.LevelsDirectoryPath}{path}";
            var levelAsset = Resources.Load<TextAsset>(resourcePath);
            
            if(levelAsset != null)
            {
                level = levelAsset.text;
                return true;
            }

            var realFilePath = $"{GetLevelsDirectoryPath()}{path}{LevelFormatSettings.Extension}";
            if (File.Exists(realFilePath))
            {
                level = File.ReadAllText(realFilePath);
                return true;
            }

            level = "";
            return false;
        }

        public void LoadLevel(string path)
        {
            LevelToLoad = path;
            LoadLevel();
        }
        public void LoadLevel()
        {
            if(LevelToLoad == null || LevelToLoad == "")
            {
                if (levelPath == null || levelPath == "")
                {
                    Debug.LogError("No level to load was specified");
                    return;
                }
                LevelToLoad = levelPath;
            }

            if (!TryLoadLevelString(LevelToLoad, out var levelTxt))
            {
                Debug.LogError($"Could not load level \"{LevelToLoad}\".");
                return;
            }
            
            var levelData = LevelData.FromXML(levelTxt);
            LoadLevel(levelData);
        }

        public virtual void LoadLevel(LevelData levelData) 
        {
            levelPath = LevelToLoad;

            ClearLevelObjects();

            simulationManager.DroneCamera.transform.position = levelData.Info.CameraPosition;

            foreach(var objectData in levelData.Objects)
            {
                var objectGameObject = objectData.ToGameObject(levelContainer);
                if (objectGameObject == null) continue;
            }

            simulationManager.SetPhysicsSimulation(levelContainer);
        }

        public virtual void MakeEmptyLevel()
        {
            if (IsEmptyLevel) return;

            LevelToLoad = "";
            levelPath = "";

            ClearLevelObjects();
        }

        protected virtual void ClearLevelObjects()
        {
            for (int i = levelContainer.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(levelContainer.transform.GetChild(i).gameObject);
            }
        }
    }
}
