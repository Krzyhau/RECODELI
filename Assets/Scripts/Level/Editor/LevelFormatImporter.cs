using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace RecoDeli.Scripts.Level
{
    [ScriptedImporter(1, LevelFormatSettings.Extension)]
    public class LevelFormatImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var textAssetString = File.ReadAllText(ctx.assetPath);
            var textAsset = new TextAsset(textAssetString);

            Texture2D thumbnail = Resources.Load<Texture2D>("Icons/Level Icon");

            ctx.AddObjectToAsset("Level Data", textAsset, thumbnail);
        }
    }
}
