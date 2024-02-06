using RecoDeli.Scripts.Utils;
using System;
using UnityEngine;

namespace RecoDeli.Scripts.Tasks
{
    [Serializable]
    public struct GameTaskCollectionRecord
    {
        public string Name;
        public GameTask[] Tasks;
    }


    [Serializable]
    [CreateAssetMenu(fileName = "Game Tasks", menuName = "RECODELI/Game Task Dictionary")]
    [ScriptableObjectSingletonPath("Settings/Game Tasks")]
    public class GameTaskDictionary : ScriptableObjectSingleton<GameTaskDictionary>
    {
        public GameTaskCollectionRecord[] Collections;
    }
}
