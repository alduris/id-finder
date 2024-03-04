using System;
using System.Collections.Generic;
using Menu.Remix.MixedUI;

namespace FinderMod.Inputs
{
    public class Label(string text) : BaseInput(text, 0)
    {
        private readonly string text = text;

        public override void AddUI(float x, ref float y, List<UIelement> inputs, Action UpdateQueryBox)
        {
            inputs.Add(new OpLabel(x, y, text));
            y -= LINE_HEIGHT;
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
