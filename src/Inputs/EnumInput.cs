using System;
using System.Collections.Generic;
using System.Linq;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class EnumInput<T> : Input<T> where T : Enum
    {
        private readonly T init;
        protected readonly Func<T, string> nameConv = null!;
        public List<T> excludeOptions = [];

        public EnumInput(string name, T init) : base(name, init)
        {
            this.init = init;
        }

        public EnumInput(string name, T init, Func<T, string> nameConv) : this(name, init)
        {
            this.nameConv = nameConv;
        }

        public override float Height => 24f;

        protected override UIconfig GetElement(Vector2 pos)
        {
            var el = new OpResourceSelector(Config(), pos, 160f);
            el._itemList = el._itemList
                .Where(x => !excludeOptions.Contains((T)Enum.Parse(typeof(T), x.name)))
                .Select(x => nameConv != null ? new ListItem(x.name, nameConv((T)Enum.Parse(typeof(T), x.name)), x.value) : x)
                .ToArray();
            return el;
        }

        protected override T GetValue(UIconfig element)
        {
            if (element.value != null)
            {
                return (T)Enum.Parse(typeof(T), element.value);
            }
            return init;
        }
    }
}
