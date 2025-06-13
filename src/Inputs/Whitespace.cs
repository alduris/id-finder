using System.Collections.Generic;
using Menu.Remix.MixedUI;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Creates configurable size gap (neglecting margins)
    /// </summary>
    /// <param name="size">Size of gap. Default 10px</param>
    public class Whitespace(float size = 10f) : IElement
    {
        /// <summary>Total height of the element.</summary>
        public float Height => size;

        /// <summary>
        /// Creates the element. For this particular element, simply changes <c>y</c> by the defined <c>size</c>.
        /// </summary>
        /// <param name="x">What x to start from</param>
        /// <param name="y">What y to start from</param>
        /// <param name="elements">The list of elements to add to</param>
        public void Create(float x, ref float y, List<UIelement> elements)
        {
            y -= size;
        }
    }
}
