using Menu.Remix.MixedUI;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class BoolInput : BaseInput
    {
        private bool value;

        public BoolInput(string name) : base(name, 1) { }

        public override UIelement GetUI(float tx, float x, ref float y)
        {
            var input = new OpSimpleButton(new Vector2(x, y), new Vector2(40f, 24f), value ? "Yes" : "No");
            input.OnClick += (_) =>
            {
                value = !value;
                input.text = value ? "Yes" : "No";
            };
            return input;
        }

        public override float? GetValue(int index)
        {
            return Enabled ? (value ? 1f : 0f) : null;
        }

        public override string ToString()
        {
            return Name + ": " + (value ? "yes" : "no");
        }
    }
}
