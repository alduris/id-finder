using System;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using BepInEx.Logging;
using FinderMod.Search;

#pragma warning disable CS0618
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace FinderMod
{
    [BepInPlugin("alduris.finder", "ID Finder", VERSION)]
    internal partial class Plugin : BaseUnityPlugin
    {
        private readonly Options Options;
        public static Plugin instance = null!;
        public static ManualLogSource logger = null!;
        public const string VERSION = "2.2";
        public static readonly Version CurrentVersion = new(VERSION);

        public Plugin()
        {
            try
            {
                instance = this;
                logger = base.Logger;
                Options = new Options(this, base.Logger);
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex);
                throw;
            }
        }

        public void OnEnable()
        {
            On.RainWorld.PreModsInit += RainWorld_PreModsInit;
            On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
        }

        private void RainWorld_PreModsInit(On.RainWorld.orig_PreModsInit orig, RainWorld self)
        {
            orig(self);
            OptionRegistry.InitializeDLC();
        }

        private bool IsInit;
        private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self); // do not remove me
            try
            {
                if (IsInit) return;

                On.RainWorldGame.ShutDownProcess += RainWorldGameOnShutDownProcess;
                On.GameSession.ctor += GameSessionOnctor;

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