using System;
using System.IO;
using BepInEx.Logging;
using FinderMod.Tabs;
using Menu.Remix;
using UnityEngine;

namespace FinderMod
{
    internal class Interface : OptionInterface
    {
        private readonly ManualLogSource logger;
        internal static Texture2D? logoTex;


        public Interface(Plugin modInstance, ManualLogSource loggerSource)
        {
            logger = loggerSource;
        }

        internal static void LoadFile(string fileName, ref Texture2D? tex)
        {
            if (Futile.atlasManager.GetAtlasWithName(fileName) != null) return;
            string path = AssetManager.ResolveFilePath(Path.Combine("Illustrations", fileName + ".png"));
            tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            AssetManager.SafeWWWLoadTexture(ref tex, "file:///" + path, true, true);
            Futile.atlasManager.LoadAtlasFromTexture(fileName, tex, false);
        }

        public override void Initialize()
        {
            base.Initialize();

#if DEBUG
            // While testing, it's convenient to have an easy way to reload the shaders
            // In production they only need to be loaded once
            InternalShaders.LoadShaders();
#endif

            // Load resources
            LoadFile("idfinder-logo", ref logoTex);

            // Initialize tabs
            Tabs =
            [
                new SearchTab(this),
                new ValuesTab(this),
                new HistoryTab(this),
                new HelpTab(this),
            ];

            foreach (var tab in Tabs)
            {
                (tab as BaseTab)!.Initialize();
            }

            logger.LogInfo("Initialized options menu");
        }

        public override void Update()
        {
            base.Update();

            foreach (var tab in Tabs)
            {
                (tab as BaseTab)!.Update();
            }
        }

        public void ClearMemory()
        {
            if (Tabs == null) return;
            foreach (var tab in Tabs)
            {
                (tab as BaseTab)?.ClearMemory();
            }
        }

        internal void CustomErrorScreen(Exception ex)
        {
            // This is after the default error screen gets applied so we have to unload that one first
            Tabs[0]._Unload();
            ConfigContainer.activeTab = null;

            // Init our own custom error screen
            Tabs[0] = new ErrorTab(this, ex);
            ConfigContainer._ChangeActiveTab(0);
            ConfigContainer.menuTab.tabCtrler.Change();
            ConfigContainer.instance._FocusNewElement(ConfigContainer.menuTab.BackButton, true);
        }
    }
}
