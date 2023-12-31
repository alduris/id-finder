﻿using System;
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
    internal partial class FinderPlugin : BaseUnityPlugin
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

                // Enable if debug needed
                // ApplyDebugHooks(); // see Test/DebugHooks

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