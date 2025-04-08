using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class ColorRGBInput : Input<Color>
    {
        public override float InputHeight => 150f;

        public ColorRGBInput(string name, Color color) : base(name, color)
        {
            inputOnNewLine = true;
        }

        public ColorRGBInput(string name) : this(name, new Color(Random.value, Random.value, Random.value)) { }

        protected override UIconfig GetElement(Vector2 pos)
        {
            return new OpColorPicker(Config(), pos);
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
    }

}
