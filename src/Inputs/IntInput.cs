using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class IntInput(string name, int min, int max) : RangedInput<int>(name, (min + max) / 2, min, max)
    {
        public override float InputHeight => 24f;

        protected override UIconfig GetElement(Vector2 pos)
        {
            return new OpSlider(ConfigRange(), pos - new Vector2(0, 3f), 160);
        }

        protected override int GetValue(UIconfig element) => (element as OpSlider).GetValueInt();
    }
}
