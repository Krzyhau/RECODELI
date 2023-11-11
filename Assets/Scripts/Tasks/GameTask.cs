using System;
using UnityEngine;

namespace RecoDeli.Scripts.Tasks
{
    [Serializable]
    public struct GameTask
    {
        [Serializable]
        public enum ActionType
        {
            None,
            Task,
            Special
        }

        public string Title;

        public string SenderName;
        public string SenderAddress;

        public int AdditionalReceivers;

        [TextArea(5, 5)]
        public string Description;

        public ActionType Action;
        public string ActionParameter;
    }
}
