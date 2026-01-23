using System.IO;
using UnityEngine;

#nullable disable
namespace FinderMod
{
    internal static class InternalShaders
    {
        public static ComputeShader personalityShader;

        public static void LoadShaders()
        {
            var bundle = AssetBundle.LoadFromFile(AssetManager.ResolveFilePath("shaders/idfinder", false));

            var assets = bundle.LoadAllAssets();
            foreach (var asset in assets)
            {
                Plugin.logger.LogDebug($"Found asset: {asset.name} (type: {asset.GetType().FullName})");
            }

            personalityShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/Personality.compute");
        }
    }
}
#nullable enable
