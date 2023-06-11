using RecoDeli.Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace RecoDeli.Scripts.Settings
{
    [CreateAssetMenu(fileName = "RECODELI Settings", menuName = "RECODELI/Settings", order = 1)]
    public class RecoDeliSettings : ScriptableObjectSingleton<RecoDeliSettings>
    {
        public const string LevelFormatExtension = ".rdlvl";

        [Header("Paths")]
        public string GameplaySceneName;
        public string SimpleMapListSceneName;
        public string LevelObjectPrefabsPath;
        public string LevelsDirectoryPath;

        [Header("Assets")]
        public AudioMixerGroup MainAudioMixerGroup;
        public AudioMixerGroup MusicAudioMixerGroup;
        public Texture2D LevelFileThumbnail;
    }
}
