using RecoDeli.Scripts.Level.Format;
using RecoDeli.Scripts.Settings;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RecoDeli.Scripts.Level
{
    [CustomEditor(typeof(EditorLevelLoader))]
    public class EditorLevelLoaderEditor : Editor
    {
        private string helpText = "";
        private double helpTextTime = 0.0;
        private MessageType helpTextType;

        private EditorLevelLoader levelLoader;
        void OnEnable()
        {
            levelLoader = (EditorLevelLoader)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("simulationManager"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("levelContainer"));

            EditorGUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("New"))
            {
                OnNew();
                return;
            }
            if (GUILayout.Button("Load"))
            {
                OnLoad();
                return;
            }
            if (GUILayout.Button("Save"))
            {
                OnSave();
                return;
            }
            if (GUILayout.Button("Save as..."))
            {
                OnSaveAs();
                return;
            }
            EditorGUILayout.EndHorizontal();

            var levelName = serializedObject.FindProperty("levelPath").stringValue;
            if (levelName == null || levelName.Length == 0) levelName = "(none)";
            EditorGUILayout.LabelField("Loaded level", levelName, EditorStyles.boldLabel);

            if(ShouldShowHelpBox())
            {
                EditorGUILayout.HelpBox(helpText, helpTextType);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private bool ShouldShowHelpBox()
        {
            return helpTextTime > EditorApplication.timeSinceStartup;
        }

        private void ShowHelpBox(string text, MessageType type = MessageType.Info)
        {
            helpText = text;
            helpTextTime = EditorApplication.timeSinceStartup + 5.0f;
            helpTextType = type;
        }


        private void OnNew()
        {
            levelLoader.MakeEmptyLevel();
            ShowHelpBox("Empty level has been created.");
        }

        private void OnLoad()
        {
            string levelsEditorPath = LevelLoader.GetLevelsDirectoryPath();
            string levelPath = EditorUtility.OpenFilePanel(
                "Open level file",
                levelsEditorPath, 
                LevelFormatSettings.Extension.Replace(".", "")
            );

            if (levelPath.Length == 0) return;
            if (!levelPath.EndsWith(LevelFormatSettings.Extension))
            {
                ShowHelpBox("Cannot load a level file that doesn't end with extension " + LevelFormatSettings.Extension, MessageType.Error);
                return;
            }

            if (!levelPath.StartsWith(levelsEditorPath))
            {
                ShowHelpBox("Cannot load level files outside of the levels directory.", MessageType.Error);
                return;
            }

            var levelName = levelPath.Substring(levelsEditorPath.Length).Replace(LevelFormatSettings.Extension, "");

            levelLoader.LoadLevel(levelName);
            ShowHelpBox($"Loaded level {levelName}.");
        }

        private void OnSave()
        {
            levelLoader.SaveCurrentLevel();
            ShowHelpBox("Level has been saved.");
        }

        private void OnSaveAs()
        {
            var levelsEditorPath = LevelLoader.GetLevelsDirectoryPath();
            var levelPath = EditorUtility.SaveFilePanel(
                "Save level",
                levelsEditorPath,
                "Level",
                LevelFormatSettings.Extension.Replace(".", "")
            );

            if (levelPath.Length == 0) return;
            if (!levelPath.EndsWith(LevelFormatSettings.Extension))
            {
                ShowHelpBox("Cannot save a level to a file that doesn't end with extension " + LevelFormatSettings.Extension, MessageType.Error);
                return;
            }

            if (!levelPath.StartsWith(levelsEditorPath))
            {
                ShowHelpBox("Cannot save level files outside of the levels directory.", MessageType.Error);
                return;
            }

            var levelName = levelPath.Substring(levelsEditorPath.Length).Replace(LevelFormatSettings.Extension, "");

            levelLoader.LevelEditorPath = levelName;
            LevelLoader.LevelToLoad = levelName;
            levelLoader.SaveCurrentLevel();
            ShowHelpBox("Level has been saved.");
        }


        [OnOpenAsset]
        private static bool HandleLevelAssetOpening(int instanceID, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID);
            var path = Application.dataPath + AssetDatabase.GetAssetPath(asset).Replace("Assets", "");
            if (!path.EndsWith(LevelFormatSettings.Extension)) return false;
            var levelsEditorPath = LevelLoader.GetLevelsDirectoryPath();
            if (!path.StartsWith(levelsEditorPath)) return false;

            var levelLoader = GameObject.FindObjectOfType<LevelLoader>();
            if (levelLoader == null)
            {
                // attempt to change scene into the gameplay one
                EditorSceneManager.OpenScene("Assets/Resources/Scenes/" + RecoDeliGame.GameplaySceneName + ".unity");
                levelLoader = GameObject.FindObjectOfType<LevelLoader>();
            }

            if (levelLoader == null)
            {
                Debug.LogWarning("Can't open level file - no LevelLoader instance in current scene.");
                return true;
            }

            var levelName = path.Substring(levelsEditorPath.Length).Replace(LevelFormatSettings.Extension, "");

            Debug.Log($"Attempting to load level {levelName}");
            levelLoader.LoadLevel(levelName);
            return true;
        }
    }
}
