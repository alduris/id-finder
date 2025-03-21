﻿using System;
using System.Collections.Generic;
using System.Linq;
using FinderMod.Search.Options;

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

                // Misc Creatures
                { "Lantern Mouse Variations", () => new LanternMouseOption() },
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
                option.name = name;
                return true;
            }
            return false;
        }
    }
}
