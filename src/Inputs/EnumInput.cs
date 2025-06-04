using System;
using System.Collections.Generic;
using System.Linq;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class EnumInput<T> : Input<T> where T : struct, Enum
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

        public override float InputHeight => 24f;

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
                if (Enum.TryParse(element.value, out T result))
                {
                    return result;
                }
                else if (int.TryParse(element.value, out int i))
                {
                    return (T)Enum.GetValues(typeof(T)).GetValue(i);
                }
            }
            return init;
        }


        public override JObject ToSaveData()
        {
            return new JObject
            {
                ["enabled"] = enabled,
                ["value"] = value.ToString(),
                ["bias"] = bias
            };
        }

        public override void FromSaveData(JObject data)
        {
            enabled = (bool)data["enabled"]!;
            value = (T)Enum.Parse(typeof(T), (string)data["value"]!);
            bias = (int)data["bias"]!;
        }
    }
}
