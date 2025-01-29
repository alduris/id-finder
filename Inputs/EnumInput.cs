using System;
using System.Linq;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class EnumInput<T> : Input<T> where T : Enum
    {
        private readonly T init;
        protected readonly Func<T, string> nameConv = null;

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
            var el = new OpResourceSelector(OpUtil.CosmeticBind(value), pos, 160f);
            if (nameConv != null)
            {
                el._itemList = el._itemList.Select(x => new ListItem(x.name, nameConv((T)Enum.Parse(typeof(T), x.name)), x.value)).ToArray();
            }
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
