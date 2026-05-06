using System;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using BepInEx;
using BepInEx.Logging;
using FinderMod.Search;
using UnityEngine;

#pragma warning disable CS0618
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace FinderMod
{
    [BepInPlugin("alduris.finder", "ID Finder", VERSION)]
    internal sealed class Plugin : BaseUnityPlugin
    {
        private readonly Interface Options;
        public static Plugin instance = null!;
        public static ManualLogSource logger = null!;
        public const string VERSION = "2.3";
        public static readonly Version CurrentVersion = new(VERSION);

        public Plugin()
        {
            try
            {
                instance = this;
                logger = base.Logger;
                Options = new Interface(this, base.Logger);
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
            On.ProcessManager.ActualProcessSwitch += ProcessManager_ActualProcessSwitch;
            On.OptionInterface.ErrorScreen += OptionInterface_ErrorScreen;
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

                // Register UI
                MachineConnector.SetRegisteredOI("alduris.finder", Options);
                IsInit = true;

                // Register Dev Console interactions
                if (ModManager.ActiveMods.Any(x => x.id == "slime-cubed.devconsole"))
                    Commands.Register();

                // Register shaders
                InternalShaders.LoadShaders();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void ProcessManager_ActualProcessSwitch(On.ProcessManager.orig_ActualProcessSwitch orig, ProcessManager self, ProcessManager.ProcessID ID, float fadeOutSeconds)
        {
            orig(self, ID, fadeOutSeconds);
            ClearMemory();
        }

        private void OptionInterface_ErrorScreen(On.OptionInterface.orig_ErrorScreen orig, OptionInterface self, Exception ex, bool isInit)
        {
            orig(self, ex, isInit);
            if (self is Interface options)
            {
                options.CustomErrorScreen(ex);
            }
        }

        private void ClearMemory()
        {
            Options.ClearMemory();
        }
    }
}