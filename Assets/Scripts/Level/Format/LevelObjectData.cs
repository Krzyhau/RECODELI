using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using System.ComponentModel;
using System.Xml.Serialization;
using BEPUphysics.Unity;
using UnityEditor;

namespace RecoDeli.Scripts.Level.Format
{
    [Serializable]
    [XmlType("Object")]
    public class LevelObjectData
    {
        public string PrefabName;
        public Vector3 Position;
        public Vector4 Rotation;
        public Vector3 Scale;
        public LevelObjectAdditionalData AdditionalData;

        public bool IsValid => PrefabName != "";

        private static LevelObjectPrefab[] levelObjectPrefabs;
        static LevelObjectData()
        {
            levelObjectPrefabs = Resources.LoadAll<LevelObjectPrefab>(LevelFormatSettings.ObjectPrefabsPath);
        }

        public LevelObjectData()
        {
            PrefabName = "";
            Position = Vector3.zero;
            Rotation = new Vector4(0f, 0f, 0f, 1f);
            AdditionalData = new LevelObjectAdditionalData();
        }
        public LevelObjectData(GameObject gameObject) {

            var prefabInfo = gameObject.GetComponent<LevelObjectPrefab>();
            PrefabName = (prefabInfo != null) ? prefabInfo.Name : "Unknown";

            Position = gameObject.transform.position;
            Rotation = new Vector4(
                gameObject.transform.rotation.x,
                gameObject.transform.rotation.y,
                gameObject.transform.rotation.z,
                gameObject.transform.rotation.w
            );
            Scale = gameObject.transform.localScale;
            AdditionalData = LevelObjectAdditionalData.Extract(gameObject);
        }


        public GameObject ToGameObject(BepuSimulation levelContainer)
        {
            var prefabInfo = levelObjectPrefabs.Where(p=>p.Name == PrefabName).FirstOrDefault();
            if (prefabInfo == null) return null;

            GameObject instance = null;
            if (Application.isPlaying)
            {
                instance = GameObject.Instantiate(prefabInfo.gameObject, levelContainer.transform).gameObject;
            }
            else
            {
                instance = PrefabUtility.InstantiatePrefab(prefabInfo.gameObject, levelContainer.transform) as GameObject;
            }
            instance.transform.name = prefabInfo.Name;
            instance.transform.position = Position;
            instance.transform.rotation = new Quaternion(
                Rotation.x,
                Rotation.y,
                Rotation.z,
                Rotation.w
            );
            instance.transform.localScale = Scale;

            LevelObjectAdditionalData.Apply(instance.gameObject, AdditionalData);

            return instance;
        }
    }
}
