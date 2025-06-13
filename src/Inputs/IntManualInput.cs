using Menu.Remix.MixedUI;
using UnityEngine;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Integer input with manually entered value
    /// </summary>
    /// <param name="name">Name of input</param>
    /// <param name="init">Maximum value of input</param>
    public class IntManualInput(string name, int init) : Input<int>(name, init)
    {
        /// <summary>Minimum value, if defined</summary>
        public int? minValue;
        /// <summary>Maximum value, if defined</summary>
        public int? maxValue;

        /// <summary>
        /// Integer input with manually entered value and explicit minimum and maximum.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="init"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        public IntManualInput(string name, int init, int minValue, int maxValue) : this(name, init)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        /// <summary>Height of input.</summary>
        public override float InputHeight => 24f;

        /// <summary>Returns actual UI element to create</summary>
        /// <param name="pos">Position of UI element</param>
        /// <returns>New instance of element</returns>
        protected override UIconfig GetElement(Vector2 pos)
        {
            return new OpTextBox(OpUtil.CosmeticRange(value, minValue ?? int.MinValue, maxValue ?? int.MaxValue), pos, 80f) { allowSpace = true, accept = OpTextBox.Accept.Int };
        }

        /// <summary>Gets the value of the element</summary>
        /// <param name="element">Element whose value to check</param>
        /// <returns>Value of the input</returns>
        protected override int GetValue(UIconfig element)
        {
            return (element as OpTextBox)!.valueInt;
        }
    }
}
