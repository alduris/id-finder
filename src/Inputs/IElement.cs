using System.Collections.Generic;
using Menu.Remix.MixedUI;

namespace FinderMod.Inputs
{
    public interface IElement
    {
        /// <summary>
        /// Total height of the element.
        /// </summary>
        public float Height { get; }

        /// <summary>
        /// Should create the element.
        /// </summary>
        /// <param name="x">What x to start from</param>
        /// <param name="y">What y to start from</param>
        /// <param name="elements">The list of elements to add to</param>
        public void Create(float x, ref float y, List<UIelement> elements);
    }
}
