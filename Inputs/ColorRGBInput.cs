using Menu.Remix.MixedUI;
using UnityEngine;
using static FinderMod.OpUtil;

namespace FinderMod.Inputs
{
    public class ColorRGBInput : BaseInput
    {
        private Color value;

        public ColorRGBInput(string name) : base(name, 3) { }

        public override UIelement GetUI(float tx, float x, ref float y)
        {
            var input = new OpColorPicker(CosmeticBind(value), new Vector2(tx, y));
            y -= input.size.y;
            input.pos = new(input.pos.x, input.pos.y - input.size.y);
            input.OnValueChanged += (_, _, _) => { value = input.valueColor; };
            return input;
        }

        public override float? GetValue(int index)
        {
            if (!Enabled) return null;

            if      (index == 0) return value.r;
            else if (index == 1) return value.g;
            else                 return value.b;
        }

        public override string ToString()
        {
            return $"{Name}: rgb({value.r}, {value.g}, {value.b})";
        }
    }
}
