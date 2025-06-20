using BepInEx.Logging;
using FinderMod.Tabs;

namespace FinderMod
{
    internal class Options : OptionInterface
    {
        private readonly ManualLogSource logger;

        public Options(Plugin modInstance, ManualLogSource loggerSource)
        {
            logger = loggerSource;
        }

        public override void Initialize()
        {
            base.Initialize();

            // Initialize tabs
            Tabs =
            [
                new SearchTab(this),
                new ValuesTab(this),
                new HistoryTab(this)
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

    }
}
