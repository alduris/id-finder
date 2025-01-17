using System.Collections.Generic;
using Menu.Remix.MixedUI;

namespace FinderMod.Inputs
{
    public class Whitespace(float size) : IElement
    {
        public float Height => size;

        public void Create(float x, ref float y, List<UIelement> elements)
        {
            y -= size;
        }
    }
}
