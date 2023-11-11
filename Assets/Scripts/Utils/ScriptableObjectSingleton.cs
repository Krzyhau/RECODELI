using System;
using UnityEngine;

namespace RecoDeli.Scripts.Utils
{
    public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if(_instance != null) return _instance;

                var path = typeof(T).Name;
                var pathAttribute = Attribute.GetCustomAttribute(typeof(T), typeof(ScriptableObjectSingletonPathAttribute)) as ScriptableObjectSingletonPathAttribute;
                if(pathAttribute != null)
                {
                    path = pathAttribute.Path;
                }

                _instance = Resources.Load<T>(typeof(T).Name);

                if (_instance == null)
                {
                    Debug.LogError($"No instance of ScriptableObjectSingleton with type and name {typeof(T).Name} has been found in Resources directory.");
                }

                return _instance;
            }
        }
    }

    sealed class ScriptableObjectSingletonPathAttribute : Attribute
    {
        public string Path { get; }

        public ScriptableObjectSingletonPathAttribute(string path)
        {
            Path = path;
        }
    }
}
