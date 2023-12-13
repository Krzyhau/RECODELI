using RecoDeli.Scripts.Settings;
using System;
using UnityEngine;

namespace RecoDeli.Scripts.Audio
{
    [Serializable]
    [CreateAssetMenu(fileName = "MusicTrack", menuName = "RECODELI/Music Track")]
    public class MusicTrack : ScriptableObject
    {
        public AudioClip editorTrack;
        public float editorTrackVolume;
        public AudioClip simulationTrack;
        public float simulationTrackVolume;

        [Header("Dynamic Fade")]
        public bool dynamicFade;
        public float fadeTime;

        [Header("Looping")]
        public float loopStart;
        public float loopEnd;

        public static MusicTrack FindByName(string name)
        {
            string path = RecoDeliGame.Settings.MusicTrackPath + name;
            var track = Resources.Load<MusicTrack>(path);
            return track;
        }
    }
}
