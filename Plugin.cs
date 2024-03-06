﻿using System;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using BepInEx.Logging;

#pragma warning disable CS0618
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace FinderMod
{
    [BepInPlugin("alduris.finder", "ID Finder", "1.3.0")]
    internal partial class Plugin : BaseUnityPlugin
    {
        private readonly Options Options;
        public static Plugin instance;
        public static ManualLogSource logger;

        public Plugin()
        {
            try
            {
                instance = this;
                logger = base.Logger;
                Options = new Options(base.Logger);
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
                IsInit = true;

                // Enable if debug needed
                // ApplyDebugHooks(); // see Test/DebugHooks

                MachineConnector.SetRegisteredOI("alduris.finder", Options);
                Logger.LogInfo("Loaded successfully");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }
    }
}