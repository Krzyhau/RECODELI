using RecoDeli.Scripts.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RecoDeli.Scripts.Leaderboards
{
    [CreateAssetMenu(fileName = "AuthorRecords", menuName = "RECODELI/Author Records", order = 2)]
    [ScriptableObjectSingletonPath("Settings/AuthorRecords")]
    public class AuthorRecords : ScriptableObjectSingleton<AuthorRecords>
    {
        [Serializable]
        public struct Record
        {
            public string MapName;
            public float Time;
            public int Instructions;
        }

        public string AuthorName;

        public List<Record> Records;
    }
}
