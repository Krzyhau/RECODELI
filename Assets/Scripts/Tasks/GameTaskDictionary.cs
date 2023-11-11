using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class GameTaskDictionary : ScriptableObject
    {
        public GameTaskCollectionRecord[] Collections;
    }
}
