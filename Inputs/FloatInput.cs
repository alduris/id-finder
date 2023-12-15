using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;
using static FinderMod.OpUtil;

namespace FinderMod.Inputs
{
    public class FloatInput : BaseInput
    {
        private readonly float min;
        private readonly float max;
        private float value;

        public FloatInput(string name, float min, float max) : base(name, 1)
        {
            this.min = min;
            this.max = max;
        }
        public FloatInput(string name) : this(name, 0f, 1f) { }

        public override UIelement GetUI(float tx, float x, ref float y)
        {
            var input = new OpFloatSlider(CosmeticBind(value), new Vector2(x, y - 4f), 160, 4) { min = min, max = max };
            input.OnValueChanged += (_, _, _) => { value = input.GetValueFloat(); };
            return input;
        }

        public override float? GetValue(int index)
        {
            return Enabled ? value : null;
        }

        public override string ToString()
        {
            return Name + ": " + value;
        }
    }
}
