using System;
using System.Collections.Generic;
using System.Linq;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Multichoice input based on an <c>enum</c>
    /// </summary>
    /// <typeparam name="T">Enum to operate on</typeparam>
    /// <param name="name">Name of input</param>
    /// <param name="init">Initial value</param>
    public class EnumInput<T>(string name, T init) : Input<T>(name, init) where T : struct, Enum
    {
        private readonly T init = init;
        /// <summary>Name conversion function</summary>
        protected readonly Func<T, string> nameConv = null!;
        /// <summary>Enum values to not include in list</summary>
        public List<T> excludeOptions = [];

        /// <summary>
        /// Creates an enum input with an initial value and a function to convert an enum value into a different string in the combo box
        /// </summary>
        /// <param name="name">Name of input</param>
        /// <param name="init">Initial value</param>
        /// <param name="nameConv">Enum to string conversion function</param>
        public EnumInput(string name, T init, Func<T, string> nameConv) : this(name, init)
        {
            this.nameConv = nameConv;
        }

        /// <summary>Height of input.</summary>
        public override float InputHeight => 24f;

        /// <summary>Returns actual UI element to create</summary>
        /// <param name="pos">Position of UI element</param>
        /// <returns>New instance of element</returns>
        protected override UIconfig GetElement(Vector2 pos)
        {
            var el = new OpResourceSelector(Config(), pos, 160f);
            el._itemList = el._itemList
                .Where(x => !excludeOptions.Contains((T)Enum.Parse(typeof(T), x.name)))
                .Select(x => nameConv != null ? new ListItem(x.name, nameConv((T)Enum.Parse(typeof(T), x.name)), x.value) : x)
                .ToArray();
            return el;
        }

        /// <summary>Gets the value of the element</summary>
        /// <param name="element">Element whose value to check</param>
        /// <returns>Value of the input</returns>
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


        /// <summary>Converts the element into a format convertable to JSON via Newtonsoft.</summary>
        /// <returns>A struct containing the data for easy conversion</returns>
        public override JObject ToSaveData()
        {
            return new JObject
            {
                ["enabled"] = enabled,
                ["value"] = value.ToString(),
                ["bias"] = bias
            };
        }

        /// <summary>
        /// Returns the element to the state it was given the save state data.
        /// </summary>
        /// <param name="data">The save data to restore data to</param>
        public override void FromSaveData(JObject data)
        {
            enabled = (bool)data["enabled"]!;
            value = (T)Enum.Parse(typeof(T), (string)data["value"]!);
            bias = (int)data["bias"]!;
        }
    }
}
