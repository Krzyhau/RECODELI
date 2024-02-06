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
        private static int currentSlot;

        private static float lastStorageTime;

        public static SaveData CurrentSave => save;
        public static int CurrentSlot => currentSlot;

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
            Load(0);
            Application.quitting += Save;
        }

        public static string GetSaveSlotPath(int slot)
        {
            return Application.persistentDataPath + "/SaveData" + slot + ".xml";
        }

        public static bool TryLoadSlot(int slot, out SaveData data)
        {
            var path = GetSaveSlotPath(slot);

            if (!File.Exists(path))
            {
                data = null;
                return false;
            }

            try
            {
                var serializer = new XmlSerializer(typeof(SaveData));
                using var fileStream = new FileStream(path, FileMode.Open);

                data = (SaveData)serializer.Deserialize(fileStream);
                return true;
            }
            catch
            {
                data = null;
                return false;
            }
        }

        public static void Load(int slot, bool force = false)
        {
            if (loaded && !force) return;

            lastStorageTime = Time.realtimeSinceStartup;

            if(TryLoadSlot(slot, out var loadedSave))
            {
                save = loadedSave;
                currentSlot = slot;
                Debug.Log("Save data loaded successfully.");
                loaded = true;
            }
            else
            {
                Debug.LogError($"Error while loading data. Using empty save file instead.");
                ForceLoadNewSave(slot);
            }
        }

        public static void ForceLoadNewSave(int slot)
        {
            save = new SaveData();
            currentSlot = slot;
            loaded = true;
        }

        public static void Save()
        {
            if (!loaded) return;

            save.PlayTime += Time.realtimeSinceStartup - lastStorageTime;
            lastStorageTime = Time.realtimeSinceStartup;

            try
            {
                using var fileStream = new FileStream(GetSaveSlotPath(currentSlot), FileMode.Create);

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
