using RecoDeli.Scripts.Settings;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace RecoDeli.Scripts.Level
{
    [ScriptedImporter(1, RecoDeliSettings.LevelFormatExtension)]
    public class LevelFormatImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var textAssetString = File.ReadAllText(ctx.assetPath);
            var textAsset = new TextAsset(textAssetString);

            ctx.AddObjectToAsset("Level Data", textAsset, RecoDeliGame.Settings.LevelFileThumbnail);
        }
    }
}
