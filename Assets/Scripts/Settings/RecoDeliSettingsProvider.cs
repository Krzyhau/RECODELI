using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.Settings
{
    public class RecoDeliSettingsProvider : SettingsProvider
    {
        SerializedObject serializedObject;

        public RecoDeliSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords) { }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            RecoDeliGame.instance.Save();
            serializedObject = new SerializedObject(RecoDeliGame.instance);
        }

        public override void OnGUI(string searchContext)
        {
            using var guiScope = CreateSettingsWindowGUIScope();
            
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Scene Configuration", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gameplaySceneName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("simpleMapListSceneName"));

            EditorGUILayout.Space(20);

            EditorGUILayout.LabelField("Level Format Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("levelFormatExtension"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("levelObjectPrefabsPath"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("levelsDirectoryPath"));

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                RecoDeliGame.instance.Save();
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateRecoDeliSettingsProvider()
        {
            var provider = new RecoDeliSettingsProvider("Project/RecoDeli", SettingsScope.Project);
            return provider;
        }

        private IDisposable CreateSettingsWindowGUIScope()
        {
            var unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            var type = unityEditorAssembly.GetType("UnityEditor.SettingsWindow+GUIScope");
            return Activator.CreateInstance(type) as IDisposable;
        }
    }
}
