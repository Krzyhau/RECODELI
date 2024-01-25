using RecoDeli.Scripts.Settings;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.Assets.Scripts.UI
{
    public static class InterfaceScaleSetter
    {
        public enum Scale
        {
            Small,
            Medium,
            Large
        }

        private static readonly Dictionary<Scale, Vector2Int> resolutions = new()
        {
            {Scale.Small, new(1600,900)},
            {Scale.Medium, new(1366,768)},
            {Scale.Large, new(1280,720)},
        };

        public static void Set(Scale scale)
        {
            var panelSettingInstances = new List<PanelSettings>
            {
                RecoDeliGame.Settings.MainLayerUIPanel,
                RecoDeliGame.Settings.AdditionalLayerUIPanel
            };

            foreach(var panel in panelSettingInstances)
            {
                panel.referenceResolution = resolutions[scale];
            }
        }
    }
}
