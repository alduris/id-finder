using System;
using System.Collections.Generic;
using FinderMod.Search.Options;
using FinderMod.Search.Options.LizardCosmetics;
using FinderMod.Tabs;

namespace FinderMod.Search
{
    internal static class OptionRegistry
    {
        private readonly static Dictionary<string, Func<Option>> Options;
        private static bool InitDLC = false;

        static OptionRegistry()
        {
            Plugin.logger.LogInfo("Initializing registry");

            Options = new Dictionary<string, Func<Option>>()
            {
                // Personality
                { "Personality", () => new PersonalityOption() },

                // Scavengers
                { "Scavenger Skills", () => new ScavSkillsOption() },
                { "Scavenger Variations", () => new ScavVarsOption() },
                { "Scavenger Colors", () => new ScavColorsOption() },
                { "Scavenger Back Patterns", () => new ScavBackPatternOption() },

                // Vultures
                { "Vulture Normal Wing Variations", () => new VultureWingOption() },
                { "Vulture King Wing Variations", () => new KingVultureWingOption() },

                // Noodleflies
                { "Noodlefly Adult Variations", () => new NootAdultVarsOption() },
                { "Noodlefly Infant Variations", () => new NootBabyVarsOption() },

                // Lizard
                { "Lizard Variations", () => new LizardVarsOption() },
                { "Lizard Colors", () => new LizardColorsOption() },

                { "Lizard Cosmetics (Black)", () => new BlackLizardCosmetics() },
                { "Lizard Cosmetics (Blue)", () => new BlueLizardCosmetics() },
                { "Lizard Cosmetics (Cyan)", () => new CyanLizardCosmetics() },
                { "Lizard Cosmetics (Green)", () => new GreenLizardCosmetics() },
                { "Lizard Cosmetics (Pink)", () => new PinkLizardCosmetics() },
                { "Lizard Cosmetics (Red)", () => new RedLizardCosmetics() },
                { "Lizard Cosmetics (Salamander)", () => new SalamanderCosmetics() },
                { "Lizard Cosmetics (White)", () => new WhiteLizardCosmetics() },
                { "Lizard Cosmetics (Yellow)", () => new YellowLizardCosmetics() },

                // Big Spiders
                { "Big Spider (Big) Variations", () => new BigSpiderVarsOption(BigSpiderVarsOption.SpiderType.Big) },
                { "Big Spider (Spitter) Variations", () => new BigSpiderVarsOption(BigSpiderVarsOption.SpiderType.Spitter) },

                // Misc Creatures
                { "Lantern Mouse Variations", () => new LanternMouseOption() },
                { "Leviathan Variations", () => new LeviathanVarsOption() },
                { "Miros Bird Variations", () => new MirosBirdVarsOption() },
                { "Snail Variations", () => new SnailOption() },

            };
#if !RELEASE
            // Testing only!!
            Options.Add("!TEST", () => new TestOption());
#endif
        }

        public static List<string> ListOptions() => [.. Options.Keys];

        public static void InitializeDLC()
        {
            if (InitDLC) return;
            InitDLC = true;

            if (ModManager.DLCShared)
            {
                Plugin.logger.LogInfo("Initializing registry with shared DLC options");

                // Lizards
                Options.Add("Lizard Cosmetics (Caramel)", () => new CaramelLizardCosmetics());
                Options.Add("Lizard Cosmetics (Eel)", () => new EelLizardCosmetics());
                Options.Add("Lizard Cosmetics (Zoop)", () => new ZoopLizardCosmetics());

                // Misc
                Options.Add("Miros Vulture Variations", () => new MirosVultureVarsOption());
                Options.Add("Big Spider (Mother) Variations", () => new BigSpiderVarsOption(BigSpiderVarsOption.SpiderType.Mother));
            }

            if (ModManager.MSC)
            {
                Plugin.logger.LogInfo("Initializing registry with MSC options");

                // Elite Scavengers
                Options["Elite Scavenger Skills"] = () => new EliteScavSkillsOption();
                Options["Elite Scavenger Colors"] = () => new EliteScavColorsOption();
                Options["Elite Scavenger Back Patterns"] = () => new EliteScavBackPatternOption();

                // Slugpups
                Options["Slugpup Variations"] = () => new SlupVarsOption();
                Options["Slugpup Stats"] = () => new SlupStatsOption();
                Options["Slugpup Food"] = () => new SlupFoodOption();

                // Lizards
                Options.Add("Lizard Cosmetics (Train)", () => new TrainLizardCosmetics());
            }

            if (ModManager.Watcher)
            {
                Plugin.logger.LogInfo("Initializing registry with Watcher options");

                // Lizards
                Options.Add("Lizard Cosmetics (Indigo)", () => new IndigoLizardCosmetics());
                // you cannot convince me to do blizzards or basilisks if you have peeped the horrors that are their cosmetics code

                // Misc
                Options.Add("Frog Variations", () => new FrogVarsOption());
                Options.Add("Rat Variations", () => new RatOption());
                Options.Add("Sky Whale Variations", () => new SkyWhaleVarsOption());
                Options.Add("Tardigrade Variations", () => new TardigradeOption());
            }
        }

        public static void RegisterOption(string name, Func<Option> register)
        {
            Options[name] = register;
        }

        public static bool TryGetOption(string name, out Option option)
        {
            option = null!;
            if (Options.TryGetValue(name, out var factory))
            {
                option = factory.Invoke();
                option.name = BaseTab.Translate(name);
                return true;
            }
            return false;
        }
    }
}
