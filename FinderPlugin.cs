using System;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using BepInEx.Logging;
using FinderMod.Test;
using LizardCosmetics;
using RWCustom;
using UnityEngine;

#pragma warning disable CS0618
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace FinderMod
{
    [BepInPlugin("alduris.finder", "ID Finder", "1.2.4")]
    public partial class FinderPlugin : BaseUnityPlugin
    {
        private FinderOptions Options;
        public static FinderPlugin instance;
        public static ManualLogSource logger;

        public FinderPlugin()
        {
            try
            {
                instance = this;
                logger = base.Logger;
                Options = new FinderOptions(this, base.Logger);
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex);
                throw;
            }
        }
        private void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
        }

        private bool IsInit;
        private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self); // do not remove me
            Logger.LogInfo("Initializing mod");
            try
            {
                if (IsInit) return;

                On.RainWorldGame.ShutDownProcess += RainWorldGameOnShutDownProcess;
                On.GameSession.ctor += GameSessionOnctor;

                // DEBUG STUFF REMOVE LATER
                On.LizardGraphics.ctor += LizardGraphics_ctor;
                On.LizardGraphics.AddCosmetic += LizardGraphics_AddCosmetic;
                try
                {
                    LizardTests.TestAllLizards();
                }
                catch (Exception ex)
                {
                    Logger.LogDebug(ex.Message);
                }/**/

                MachineConnector.SetRegisteredOI("alduris.finder", Options);
                IsInit = true;
                Logger.LogInfo("Loaded successfully");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        private void LizardGraphics_ctor(On.LizardGraphics.orig_ctor orig, LizardGraphics self, PhysicalObject ow)
        {
            orig(self, ow);
            if (self.lizard.abstractCreature.creatureTemplate.type == CreatureTemplate.Type.Salamander)
            {
                Logger.LogDebug(self.lizard.abstractCreature.ID.RandomSeed + ": " + (self.blackSalamander ? "Dark" : "Light"));
            }
            else if (self.lizard.abstractCreature.creatureTemplate.type == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SpitLizard)
            {
                Vector3 effect = Custom.RGB2HSL(self.lizard.effectColor);
                Vector3 ivar = Custom.RGB2HSL(self.ivarBodyColor);
                Logger.LogDebug($"{self.lizard.abstractCreature.ID.RandomSeed}: effect({effect.x}f, {effect.y}f, {effect.z}f), ivarbody({ivar.x}f, {ivar.y}f, {ivar.z}f)");
            }
        }

        private int LizardGraphics_AddCosmetic(On.LizardGraphics.orig_AddCosmetic orig, LizardGraphics self, int spriteIndex, LizardCosmetics.Template cosmetic)
        {
            int ret = orig(self, spriteIndex, cosmetic);
            string debugStr = self.lizard.abstractCreature.ID.RandomSeed + ": " + cosmetic.GetType().Name;
            if (cosmetic is Antennae) debugStr += $" (l: {(cosmetic as Antennae).length}, a: {(cosmetic as Antennae).alpha})";
            else if (cosmetic is Whiskers) debugStr += $" (num: {(cosmetic as Whiskers).amount})";
            else if (cosmetic is TailGeckoScales) debugStr += $" (r: {(cosmetic as TailGeckoScales).rows}, l: {(cosmetic as TailGeckoScales).lines})";
            Logger.LogDebug(debugStr);
            return ret;
        }

        private void RainWorldGameOnShutDownProcess(On.RainWorldGame.orig_ShutDownProcess orig, RainWorldGame self)
        {
            orig(self);
            ClearMemory();
        }
        private void GameSessionOnctor(On.GameSession.orig_ctor orig, GameSession self, RainWorldGame game)
        {
            orig(self, game);
            ClearMemory();
        }

        private void ClearMemory()
        {
            // Options.ClearMemory();
        }
    }
}