using System;
using Menu.Remix.MixedUI;

namespace FinderMod.Inputs
{
    internal class Whitespace : BaseInput
    {
        public Whitespace() : base("", 0) { }

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
            return "";
        }
    }
}
