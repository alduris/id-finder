using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Special float input that displays the selected hue on the slider bar
    /// </summary>
    public class HueInput : FloatInput
    {
        /// <summary>
        /// Creates a hue input with a range
        /// </summary>
        /// <param name="name">Name of input</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        public HueInput(string name, float min, float max) : base(name, min, max)
        {
            value = Random.Range(min, max);
        }

        /// <summary>
        /// Creates a hue input between 0 and 1
        /// </summary>
        /// <param name="name">Name of input</param>
        public HueInput(string name) : this(name, 0f, 1f) { }

        /// <summary>Returns actual UI element to create</summary>
        /// <param name="pos">Position of UI element</param>
        /// <returns>New instance of element</returns>
        protected override UIconfig GetElement(Vector2 pos)
        {
            var el = (base.GetElement(pos) as OpFloatSlider)!;
            SetColor(el);
            el.OnValueUpdate += (_, _, _) =>
            {
                SetColor(el);
            };
            return el;
        }

        private void SetColor(OpFloatSlider input)
        {
            var value = GetValue(input);
            value -= Mathf.Floor(value);
            input.colorEdge = Custom.HSL2RGB(value, 1f, 0.625f);
            input.colorFill = Custom.HSL2RGB(value, 1f, 0.625f);
        }
    }
}
