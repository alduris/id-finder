using System.IO;
using UnityEngine;

#nullable disable
namespace FinderMod
{
    internal static class InternalShaders
    {
        public static ComputeShader personalityShader;
        public static ComputeShader slugpupBehaviorShader;
        public static ComputeShader slugpupFoodShader;
        public static ComputeShader slugpupStatsShader;
        public static ComputeShader slugpupVarsShader;

        public static void LoadShaders()
        {
            var bundle = AssetBundle.LoadFromFile(AssetManager.ResolveFilePath("shaders/idfinder", false));

            var assets = bundle.LoadAllAssets();
            foreach (var asset in assets)
            {
                Plugin.logger.LogDebug($"Found asset: {asset.name} (type: {asset.GetType().FullName})");
            }

            personalityShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/Personality.compute");
            slugpupBehaviorShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/SlugpupBehavior.compute");
            slugpupFoodShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/SlugpupFood.compute");
            slugpupStatsShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/SlugpupStats.compute");
            slugpupVarsShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/SlugpupVars.compute");
        }
    }
}
#nullable enable
