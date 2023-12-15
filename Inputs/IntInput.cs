using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;
using static FinderMod.OpUtil;

namespace FinderMod.Inputs
{
    public class IntInput : BaseInput
    {
        private readonly int min;
        private readonly int max;
        private int value;

        public IntInput(string name, int min, int max) : base(name, 1)
        {
            this.min = min;
            this.max = max;
        }

        public override UIelement GetUI(float tx, float x, ref float y)
        {
            var input = new OpSlider(CosmeticBind(value), new Vector2(x, y - 4f), 160) { min = min, max = max };
            input.OnValueChanged += (_, _, _) => { value = input.GetValueInt(); };
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
