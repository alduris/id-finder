using UnityEngine;

#nullable disable
namespace FinderMod
{
    internal static class InternalShaders
    {
        private static AssetBundle assetBundle;

        public static ComputeShader personalityShader;
        public static ComputeShader slugpupBehaviorShader;
        public static ComputeShader slugpupFoodShader;
        public static ComputeShader slugpupStatsShader;
        public static ComputeShader slugpupVarsShader;
        public static ComputeShader scavengerVarsShader;
        public static ComputeShader scavengerSkillsShader;
        public static ComputeShader scavengerColorsShader;
        public static ComputeShader eliteScavengerSkillsShader;
        public static ComputeShader eliteScavengerColorsShader;
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
        public static ComputeShader grappleWormColorsShader;
        public static ComputeShader jetfishVarsShader;
        public static ComputeShader lanternMouseVarsShader;
        public static ComputeShader snailVarsShader;
        public static ComputeShader squidcadaVarsShader;
        public static ComputeShader yeekVarsShader;
        public static ComputeShader barnacleVarsShader;
        public static ComputeShader drillCrabVarsShader;
        public static ComputeShader frogVarsShader;
        public static ComputeShader ratVarsShader;
        public static ComputeShader tardigradeVarsShader;

        public static void LoadShaders()
        {
            assetBundle?.Unload(true);
            assetBundle = AssetBundle.LoadFromFile(AssetManager.ResolveFilePath("shaders/idfinder", false));

            var assets = assetBundle.LoadAllAssets();
            foreach (var asset in assets)
            {
                Plugin.logger.LogDebug($"Found asset: {asset.name} (type: {asset.GetType().FullName})");
            }

            personalityShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/Personality.compute");
            slugpupBehaviorShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/SlugpupBehavior.compute");
            slugpupFoodShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/SlugpupFood.compute");
            slugpupStatsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/SlugpupStats.compute");
            slugpupVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/SlugpupVars.compute");
            scavengerVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/ScavengerVars.compute");
            scavengerSkillsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/ScavengerSkills.compute");
            scavengerColorsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/ScavengerColors.compute");
            eliteScavengerSkillsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/EliteScavengerSkills.compute");
            eliteScavengerColorsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/EliteScavengerColors.compute");
            lizardColorsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/LizardColors.compute");
            lizardVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/LizardVars.compute");
            vultureWingShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/VultureWings.compute");
            vultureKingWingShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/VultureKingWings.compute");
            noodleflyAdultVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/NoodleflyAdultVars.compute");
            noodleflyBabyVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/NoodleflyBabyVars.compute");
            bigSpiderVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/BigSpiderVars.compute");
            centipedeVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/CentipedeVars.compute");
            coalescipedeSizeShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/CoalescipedeSize.compute");
            dropwigVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/DropwigVars.compute");
            eggbugColorsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/EggbugColors.compute");
            grappleWormColorsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/GrappleWormColors.compute");
            jetfishVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/JetfishVars.compute");
            lanternMouseVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/LanternMouseVars.compute");
            snailVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/SnailVars.compute");
            squidcadaVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/SquidcadaVars.compute");
            yeekVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/YeekVars.compute");
            barnacleVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/BarnacleVars.compute");
            drillCrabVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/DrillCrabVars.compute");
            frogVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/FrogVars.compute");
            ratVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/RatVars.compute");
            tardigradeVarsShader = assetBundle.LoadAsset<ComputeShader>("Assets/IDFinder/TardigradeVars.compute");
        }
    }
}
#nullable enable
