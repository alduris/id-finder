using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class IntInput(string name, int min, int max, int init) : RangedInput<int>(name, init)
    {
        private readonly int min = min, max = max;

        public override float Height => 24f;

        protected override UIconfig GetElement(Vector2 pos)
        {
            return new OpSlider(ConfigRange(min, max), pos - new Vector2(0, 3f), 160);
        }

        protected override int GetValue(UIconfig element) => (element as OpSlider).GetValueInt();
    }
}
