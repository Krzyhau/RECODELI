using System.IO;
using System.Xml.Serialization;
using System;
using UnityEngine;
using RecoDeli.Scripts.Level.Format;
using System.Xml;

namespace RecoDeli.Scripts.SaveManagement
{
    public static class SaveManager
    {
        private static bool loaded;
        private static SaveData save;

        private static float lastStorageTime;

        public static SaveData CurrentSave => save;

        public static string UserSaveDataPath => Application.persistentDataPath + "/SaveData.xml";

        static SaveManager()
        {
            Initialize();
        }

        public static void Initialize()
        {
            if (loaded)
            {
                return;
            }
            Load();
            Application.quitting += Save;
        }

        public static void Load()
        {
            if (loaded) return;

            lastStorageTime = Time.realtimeSinceStartup;

            if(!File.Exists(UserSaveDataPath))
            {
                save = new SaveData();
                loaded = true;
                Debug.Log("Created new empty save");
                return;
            }

            try
            {
                var serializer = new XmlSerializer(typeof(SaveData));
                using var fileStream = new FileStream(UserSaveDataPath, FileMode.Open);
                
                save = (SaveData)serializer.Deserialize(fileStream);
                Debug.Log("Save data loaded successfully");

                loaded = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error while loading data: {ex.Message}. Using empty save file instead.");
                save = new SaveData();
                loaded = true;
            }
        }

        public static void Save()
        {
            if (!loaded) return;

            save.PlayTime += Time.realtimeSinceStartup - lastStorageTime;
            lastStorageTime = Time.realtimeSinceStartup;

            try
            {
                using var fileStream = new FileStream(UserSaveDataPath, FileMode.Create);

                XmlWriter xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings()
                {
                    OmitXmlDeclaration = true,
                    Indent = true
                });
                var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                new XmlSerializer(typeof(SaveData)).Serialize(xmlWriter, save, namespaces);

                Debug.Log("Save data saved successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error while saving save data: {ex.Message}");
                Debug.LogError(ex.InnerException?.Message);
                Debug.LogError(ex.StackTrace);
            }
        }
    }
}
