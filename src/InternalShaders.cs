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
        public static ComputeShader scavengerSkillsShader;
        public static ComputeShader scavengerVarsShader;
        public static ComputeShader scavengerColorsShader;
        public static ComputeShader eliteScavengerSkillsShader;
        public static ComputeShader lizardColorsShader;
        public static ComputeShader lizardVarsShader;
        public static ComputeShader vultureWingShader;
        public static ComputeShader vultureKingWingShader;
        public static ComputeShader noodleflyAdultVarsShader;
        public static ComputeShader noodleflyBabyVarsShader;
        public static ComputeShader bigSpiderVarsShader;
        public static ComputeShader centipedeVarsShader;
        public static ComputeShader coalescipedeSizeShader;
        public static ComputeShader dropwigVarsShader;
        public static ComputeShader eggbugColorsShader;

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
            scavengerSkillsShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/ScavengerSkills.compute");
            scavengerVarsShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/ScavengerVars.compute");
            scavengerColorsShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/ScavengerColors.compute");
            eliteScavengerSkillsShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/EliteScavengerSkills.compute");
            lizardColorsShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/LizardColors.compute");
            lizardVarsShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/LizardVars.compute");
            vultureWingShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/VultureWings.compute");
            vultureKingWingShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/VultureKingWings.compute");
            noodleflyAdultVarsShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/NoodleflyAdultVars.compute");
            noodleflyBabyVarsShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/NoodleflyBabyVars.compute");
            bigSpiderVarsShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/BigSpiderVars.compute");
            centipedeVarsShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/CentipedeVars.compute");
            coalescipedeSizeShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/CoalescipedeSize.compute");
            dropwigVarsShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/DropwigVars.compute");
            eggbugColorsShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/EggbugColors.compute");
        }
    }
}
#nullable enable
