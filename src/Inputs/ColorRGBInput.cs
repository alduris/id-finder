using System.Collections.Generic;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class ColorRGBInput : Input<Color>
    {
        public override float InputHeight => 150f;
        private OpColorPicker.PickerMode pickerMode = OpColorPicker.PickerMode.HSL;

        public ColorRGBInput(string name, Color color) : base(name, color)
        {
            inputOnNewLine = true;
        }

        public ColorRGBInput(string name) : this(name, new Color(Random.value, Random.value, Random.value)) { }

        protected override UIconfig GetElement(Vector2 pos)
        {
            var picker = new OpColorPicker(Config(), pos);
            picker._SwitchMode(pickerMode);
            picker.OnChange += () => pickerMode = picker._mode;
            return picker;
        }

        protected override Color GetValue(UIconfig element)
        {
            return (element as OpColorPicker)!.valueColor;
        }

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

        public override void FromSaveData(JObject data)
        {
            enabled = (bool)data["enabled"]!;
            value = new Color((float)data["r"]!, (float)data["g"]!, (float)data["b"]!);
            bias = (int)data["bias"]!;
        }

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
