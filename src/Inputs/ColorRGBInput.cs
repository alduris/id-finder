using System.Collections.Generic;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace FinderMod.Inputs
{
    /// <summary>
    /// RGB color input
    /// </summary>
    public class ColorRGBInput : Input<Color>
    {
        /// <summary>Height of input.</summary>
        public override float InputHeight => 150f;
        private OpColorPicker.PickerMode pickerMode = OpColorPicker.PickerMode.HSL;

        /// <summary>
        /// Creates an RGB color input with a preset initial color
        /// </summary>
        /// <param name="name">Name of input</param>
        /// <param name="color">Initial value</param>
        public ColorRGBInput(string name, Color color) : base(name, color)
        {
            inputOnNewLine = true;
        }

        /// <summary>
        /// Creates an RGB color input with a random initial color
        /// </summary>
        /// <param name="name">Name of input</param>
        public ColorRGBInput(string name) : this(name, new Color(Random.value, Random.value, Random.value)) { }

        /// <summary>Returns actual UI element to create</summary>
        /// <param name="pos">Position of UI element</param>
        /// <returns>New instance of element</returns>
        protected override UIconfig GetElement(Vector2 pos)
        {
            var picker = new OpColorPicker(Config(), pos);
            picker._SwitchMode(pickerMode);
            picker.OnChange += () => pickerMode = picker._mode;
            return picker;
        }

        /// <summary>Gets the value of the element</summary>
        /// <param name="element">Element whose value to check</param>
        /// <returns>Value of the input</returns>
        protected override Color GetValue(UIconfig element)
        {
            return (element as OpColorPicker)!.valueColor;
        }

        /// <summary>Converts the element into a format convertable to JSON via Newtonsoft.</summary>
        /// <returns>A struct containing the data for easy conversion</returns>
        public override JObject ToSaveData()
        {
            return new JObject
            {
                ["enabled"] = enabled,
                ["r"] = value.r,
                ["g"] = value.g,
                ["b"] = value.b,
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
            value = new Color((float)data["r"]!, (float)data["g"]!, (float)data["b"]!);
            bias = (int)data["bias"]!;
        }

        /// <summary>Returns the string representation for the input on the history tab.</summary>
        /// <returns>The strings to represent the input on the history tab.</returns>
        public override IEnumerable<string> GetHistoryLines()
        {
            if (enabled)
            {
                yield return $"{name}: rgb({value.r}, {value.g}, {value.b})" + (bias != 1 ? $" (bias: {bias})" : "");
            }
            yield break;
        }
    }

}
