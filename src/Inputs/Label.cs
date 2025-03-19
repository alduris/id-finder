using System.Collections.Generic;
using Menu.Remix.MixedUI;
using RWCustom;

namespace FinderMod.Inputs
{
    public class Label(string text, bool big = false) : IElement
    {
        public float Height => LabelTest.LineHeight(big);

        public void Create(float x, ref float y, List<UIelement> elements)
        {
            y -= Height;
            elements.Add(new OpLabel(x, y, Custom.rainWorld.inGameTranslator.Translate(text), big));
        }
    }
}
