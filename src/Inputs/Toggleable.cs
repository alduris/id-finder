using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Menu.Remix.MixedUI;

namespace FinderMod.Inputs
{
    public class Toggleable<E> : IElement where E : IElement
    {
        public readonly BoolInput ToggleInput;
        public readonly E Element;

        public bool Toggled
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ToggleInput.value;
        }

        public Toggleable(string name, bool toggled, E element)
        {
            ToggleInput = new BoolInput(name, toggled) { hasBias = true };
            Element = element;
        }

        public float Height => ToggleInput.Height + (Toggled ? Element.Height + 6f : 0);

        public void Create(float x, ref float y, List<UIelement> elements)
        {
            ToggleInput.Create(x, ref y, elements);
            if (Toggled)
            {
                y -= 6f;
                Element.Create(x, ref y, elements);
            }
        }
    }
}
