using System;
using Menu.Remix.MixedUI;

namespace FinderMod.Inputs
{
    internal class Label : BaseInput
    {
        private readonly string text;
        public Label(string text) : base(text, 0)
        {
            this.text = text;
        }

        public override UIelement GetUI(float tx, float x, ref float y)
        {
            throw new NotImplementedException();
        }

        public override float? GetValue(int index)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return text;
        }
    }
}
