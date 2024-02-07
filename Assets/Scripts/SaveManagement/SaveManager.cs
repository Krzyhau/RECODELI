using System.IO;
using System.Xml.Serialization;
using System;
using UnityEngine;
using RecoDeli.Scripts.Level.Format;
using System.Xml;
using System.Text;

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

        public static string GetPrefsSaveSlotKey(int slot)
        {
            return "SaveData" + slot;
        }

        public static bool ShouldUsePlayerPrefs()
        {
            return Application.platform == RuntimePlatform.WebGLPlayer;
        }

        public static bool TryLoadSlot(int slot, out SaveData data)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(SaveData));

                if (ShouldUsePlayerPrefs())
                {
                    var prefsKey = GetPrefsSaveSlotKey(slot);

                    if (!PlayerPrefs.HasKey(prefsKey))
                    {
                        data = null;
                        return false;
                    }

                    using var stringReader = new StringReader(PlayerPrefs.GetString(prefsKey));
                    data = (SaveData)serializer.Deserialize(stringReader);
                }
                else
                {
                    var path = GetSaveSlotPath(slot);

                    if (!File.Exists(path))
                    {
                        data = null;
                        return false;
                    }

                    using var fileStream = new FileStream(path, FileMode.Open);
                    data = (SaveData)serializer.Deserialize(fileStream);
                }

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
                var saveBuilder = new StringBuilder();

                var xmlWriter = XmlWriter.Create(saveBuilder, new XmlWriterSettings()
                {
                    OmitXmlDeclaration = true,
                    Indent = true
                });
                var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                new XmlSerializer(typeof(SaveData)).Serialize(xmlWriter, save, namespaces);

                var saveString = saveBuilder.ToString();

                if (ShouldUsePlayerPrefs())
                {
                    PlayerPrefs.SetString(GetPrefsSaveSlotKey(currentSlot), saveString);
                }
                else
                {
                    File.WriteAllText(GetSaveSlotPath(currentSlot), saveString);
                }

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
