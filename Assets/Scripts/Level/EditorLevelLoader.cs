using RecoDeli.Scripts.Level.Format;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;

namespace RecoDeli.Scripts.Level
{
    [ExecuteAlways]
    public class EditorLevelLoader : LevelLoader
    {
        private void OnEnable()
        {
            EditorSceneManager.sceneSaving += OnSceneSaving;
        }

        private void OnDisable()
        {
            EditorSceneManager.sceneSaving -= OnSceneSaving;
        }
        private void OnSceneSaving(Scene scene, string path)
        {
            SaveCurrentLevel();
        }

        public override void SaveCurrentLevel()
        {
            base.SaveCurrentLevel();
            AssetDatabase.Refresh();
        }

        public override void LoadLevel(LevelData levelData)
        {
            Undo.RecordObject(this, "Level Loader");

            base.LoadLevel(levelData);
            foreach(Transform child in levelContainer.transform)
            {
                Undo.RegisterCreatedObjectUndo(child.gameObject, "Load Level Object for the level");
            }

            Undo.SetCurrentGroupName($"Load level {LevelToLoad}");
            Undo.IncrementCurrentGroup();
        }

        public override void MakeEmptyLevel()
        {
            if (IsEmptyLevel) return;

            Undo.RecordObject(this, "Level Loader");

            base.MakeEmptyLevel();

            Undo.SetCurrentGroupName($"Create empty level");
            Undo.IncrementCurrentGroup();
        }


        protected override void ClearLevelObjects()
        {
            for (int i = levelContainer.transform.childCount - 1; i >= 0; i--)
            {
                Undo.DestroyObjectImmediate(levelContainer.transform.GetChild(i).gameObject);
            }
        }
    }
}

#else

namespace RecoDeli.Scripts.Level
{
    public class EditorLevelLoader : LevelLoader
    {
    }
}

#endif