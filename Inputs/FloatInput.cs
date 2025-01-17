using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class FloatInput(string name, float min, float max, float init) : RangedInput<float>(name, init)
    {
        private readonly float min = min, max = max;

        public override float Height => 24f;

        protected override UIconfig GetElement(Vector2 pos)
        {
            return new OpFloatSlider(ConfigRange(min, max), pos - new Vector2(0, 3f), 160);
        }

        protected override float GetValue(UIconfig element) => (element as OpFloatSlider).GetValueFloat();
    }
}
