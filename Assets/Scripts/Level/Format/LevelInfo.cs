using System;
using UnityEngine;

namespace RecoDeli.Scripts.Level.Format
{
    [Serializable]
    public class LevelInfo
    {
        public string Name;
        public Vector3 CameraPosition;
        public Vector2 CameraBoundsOffset;
        public Vector2 CameraBoundsSize;
        public string MusicTrackName;
    }
}
