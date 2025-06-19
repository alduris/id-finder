using System.Collections.Generic;
using Menu.Remix.MixedUI;
using RWCustom;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Decorative label
    /// </summary>
    /// <param name="text">Text to display</param>
    /// <param name="big">Whether to use a big label</param>
    public class Label(string text, bool big = false) : IElement
    {
        /// <summary>Total height of the element.</summary>
        public float Height => LabelTest.LineHeight(big);

        /// <summary>
        /// Creates the element.
        /// </summary>
        /// <param name="x">What x to start from</param>
        /// <param name="y">What y to start from</param>
        /// <param name="elements">The list of elements to add to</param>
        public void Create(float x, ref float y, List<UIelement> elements)
        {
            y -= Height;
            elements.Add(new OpLabel(x, y, Custom.rainWorld.inGameTranslator.Translate(text), big) { verticalAlignment = OpLabel.LabelVAlignment.Bottom });
        }
    }
}
