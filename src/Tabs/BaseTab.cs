using Menu.Remix.MixedUI;

namespace FinderMod.Tabs
{
    public abstract class BaseTab : OpTab
    {
        public BaseTab(OptionInterface option, string name) : base(option, name) { }

        public abstract void Initialize();
        public abstract void Update();
        public virtual void ClearMemory() { }
    }
}
