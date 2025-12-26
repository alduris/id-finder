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
        public static Plugin instance = null!;
        public static ManualLogSource logger = null!;
        public const string VERSION = "2.3";
        public static readonly Version CurrentVersion = new(VERSION);

        public static readonly ProcessManager.ProcessID FinderProcess = new("IDFinder", true);

        public Plugin()
        {
            try
            {
                instance = this;
                logger = base.Logger;
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

            MenuHooks.Apply();
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
                IsInit = true;

                if (ModManager.ActiveMods.Any(x => x.id == "slime-cubed.devconsole"))
                    Commands.Register();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw;
            }
        }
    }
}