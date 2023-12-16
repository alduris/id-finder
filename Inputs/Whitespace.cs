using System;
using System.Collections.Generic;
using Menu.Remix.MixedUI;

namespace FinderMod.Inputs
{
    public class Whitespace : BaseInput
    {
        private const float WHITESPACE_AMOUNT = LINE_HEIGHT / 3;
        public Whitespace() : base("", 0) { }

        public override void AddUI(float x, ref float y, List<UIelement> inputs, Action UpdateQueryBox)
        {
            y -= WHITESPACE_AMOUNT;
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
