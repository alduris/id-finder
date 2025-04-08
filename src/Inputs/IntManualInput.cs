using Menu.Remix.MixedUI;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class IntManualInput(string name, int init) : RangedInput<int>(name, init)
    {
        public int? minValue;
        public int? maxValue;

        public override float InputHeight => 24f;

        protected override UIconfig GetElement(Vector2 pos)
        {
            return new OpTextBox(ConfigRange(minValue ?? int.MinValue, maxValue ?? int.MaxValue), pos, 80f) { allowSpace = true, accept = OpTextBox.Accept.Int };
        }

        protected override int GetValue(UIconfig element)
        {
            return (element as OpTextBox)!.valueInt;
        }
    }
}
