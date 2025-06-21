using System;
using System.Linq;
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
    internal sealed class Plugin : BaseUnityPlugin
    {
        private readonly Options Options;
        public static Plugin instance = null!;
        public static ManualLogSource logger = null!;
        public const string VERSION = "2.2.2";
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

                On.ProcessManager.ActualProcessSwitch += ProcessManager_ActualProcessSwitch;

                MachineConnector.SetRegisteredOI("alduris.finder", Options);
                IsInit = true;
                Logger.LogInfo("Loaded successfully");

                if (ModManager.ActiveMods.Any(x => x.id == "slime-cubed.devconsole"))
                    Commands.Register();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }

        private void ProcessManager_ActualProcessSwitch(On.ProcessManager.orig_ActualProcessSwitch orig, ProcessManager self, ProcessManager.ProcessID ID, float fadeOutSeconds)
        {
            orig(self, ID, fadeOutSeconds);
            ClearMemory();
        }

        private void ClearMemory()
        {
            Options.ClearMemory();
        }
    }
}