using System;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Float input with an explicit minimum and maximum
    /// </summary>
    /// <param name="name">Name of input</param>
    /// <param name="min">Minimum value</param>
    /// <param name="max">Maximum value</param>
    public class FloatInput(string name, float min, float max) : RangedInput<float>(name, (min + max) / 2, min, max)
    {

        /// <summary>
        /// Initializes with range from 0 to 1
        /// </summary>
        /// <param name="name">The name of the input</param>
        public FloatInput(string name) : this(name, 0f, 1f) { }

        /// <summary>Height of input.</summary>
        public override float InputHeight => 24f;

        /// <summary>Returns actual UI element to create</summary>
        /// <param name="pos">Position of UI element</param>
        /// <returns>New instance of element</returns>
        protected override UIconfig GetElement(Vector2 pos)
        {
            int dNum = Math.Max(1, -Mathf.FloorToInt(Mathf.Log10(max - min)) + 3);
            return new OpFloatSlider(ConfigRange(), pos - new Vector2(0, 3f), 160) { _dNum = (byte)dNum };
        }

        /// <summary>Gets the value of the element</summary>
        /// <param name="element">Element whose value to check</param>
        /// <returns>Value of the input</returns>
        protected override float GetValue(UIconfig element) => (element as OpFloatSlider).GetValueFloat();
    }
}
