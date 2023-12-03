using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menu.Remix.MixedUI;

namespace FinderMod.Tabs
{
    internal abstract class FinderTab : OpTab
    {
        public FinderTab(OptionInterface option, string name) : base(option, name) { }

        public abstract void Initialize();
        public abstract void Update();
        public virtual void ClearMemory() { }
    }
}
