using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Integer input with an explicit minimum and maximum, inclusive.
    /// </summary>
    /// <param name="name">Name of input</param>
    /// <param name="min">Minimum, inclusive</param>
    /// <param name="max">Maximum, inclusive</param>
    public class IntInput(string name, int min, int max) : RangedInput<int>(name, (min + max) / 2, min, max)
    {
        /// <summary>Height of input.</summary>
        public override float InputHeight => 24f;

        /// <summary>Returns actual UI element to create</summary>
        /// <param name="pos">Position of UI element</param>
        /// <returns>New instance of element</returns>
        protected override UIconfig GetElement(Vector2 pos)
        {
            return new OpSlider(ConfigRange(), pos - new Vector2(0, 3f), 160);
        }

        /// <summary>Gets the value of the element</summary>
        /// <param name="element">Element whose value to check</param>
        /// <returns>Value of the input</returns>
        protected override int GetValue(UIconfig element) => (element as OpSlider).GetValueInt();
    }
}
