using System;
using System.Collections.Generic;
using Menu.Remix.MixedUI;

namespace FinderMod.Inputs
{
    public class Label : BaseInput
    {
        private readonly string text;
        public Label(string text) : base(text, 0)
        {
            this.text = text;
        }

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
