using RecoDeli.Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.Settings
{
    [CreateAssetMenu(fileName = "RECODELI Settings", menuName = "RECODELI/Settings", order = 1)]
    [ScriptableObjectSingletonPath("Settings/RecoDeliSettings")]
    public class RecoDeliSettings : ScriptableObjectSingleton<RecoDeliSettings>
    {
        public const string LevelFormatExtension = ".rdlvl";

        [Header("Paths")]
        public string MainMenuSceneName;
        public string GameplaySceneName;
        public string LevelObjectPrefabsPath;
        public string MusicTrackPath;
        public string LevelsDirectoryPath;

        [Header("Assets")]
        public AudioMixerGroup MainAudioMixerGroup;
        public AudioMixerGroup MusicAudioMixerGroup;
        public ScriptableRendererFeature CRTPassRenderFeature;
        public VolumeProfile GameVolume;
        public VolumeProfile ScreenRenderingVolume;
        public PanelSettings MainLayerUIPanel;
        public PanelSettings AdditionalLayerUIPanel;
        public Texture2D LevelFileThumbnail;

        [Header("Game Settings")]
        public int UserSaveCount;
    }
}
