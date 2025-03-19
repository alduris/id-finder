using Menu.Remix.MixedUI;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class ColorRGBInput : Input<Color>
    {
        public override float Height => 150f;

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
    }

}
