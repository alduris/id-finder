using Menu.Remix.MixedUI;
using RWCustom;

namespace FinderMod.Tabs
{
    internal abstract class BaseTab : OpTab
    {
        public BaseTab(OptionInterface option, string name) : base(option, name) { }

        ~BaseTab()
        {
            ClearMemory();
        }

        public abstract void Initialize();
        public abstract void Update();
        public virtual void ClearMemory() { }

        public static string Translate(string text) => Custom.rainWorld.inGameTranslator.TryTranslate(text, out var translated) ? translated : text;
    }
}
