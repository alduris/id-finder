using System;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Creates a slider input that moves between two preset colors
    /// </summary>
    /// <param name="name">Name of input</param>
    /// <param name="end1">Left end color</param>
    /// <param name="end2">Right end color</param>
    public class ColorLerpInput(string name, Color end1, Color end2) : RangedInput<float>(name, 0.5f, 0f, 1f)
    {
        private readonly Color end1 = end1, end2 = end2;

        /// <summary>Height of input.</summary>
        public override float InputHeight => 30f;

        /// <summary>Returns actual UI element to create</summary>
        /// <param name="pos">Position of UI element</param>
        /// <returns>New instance of element</returns>
        protected override UIconfig GetElement(Vector2 pos) => new OpFloatColorSlider(ConfigRange(), pos, 160, end1, end2);

        /// <summary>Gets the value of the element</summary>
        /// <param name="element">Element whose value to check</param>
        /// <returns>Value of the input</returns>
        protected override float GetValue(UIconfig element) => (element as OpFloatSlider).GetValueFloat();

        /// <summary>
        /// Special variant of <see cref="OpFloatSlider"/> that slides between two colors, complete with a color preview box
        /// </summary>
        public class OpFloatColorSlider : OpFloatSlider
        {
            private readonly DyeableRect rect;
            private readonly Color end1, end2;

            /// <summary>
            /// Creates a special slider that slides between two colors with a color preview box
            /// </summary>
            /// <param name="configFloat">Configurable</param>
            /// <param name="pos">Position of bottom left</param>
            /// <param name="length">Length of slider</param>
            /// <param name="end1">Left end color</param>
            /// <param name="end2">Right end color</param>
            public OpFloatColorSlider(ConfigurableBase configFloat, Vector2 pos, int length, Color end1, Color end2) : base(configFloat, pos, length)
            {
                this.end1 = end1;
                this.end2 = end2;
                _dNum = 3;

                rect = new DyeableRect(myContainer, new Vector2(_size.x + 12f, 0f), new Vector2(30f, 30f), true)
                {
                    colorEdge = colorEdge,
                    colorFill = Color.Lerp(end1, end2, this.GetValueFloat()),
                    fillAlpha = 1f
                };
            }

            /// <summary>Performs a graphics update</summary>
            /// <param name="timeStacker">Time between physics ticks, range 0 to 1</param>
            public override void GrafUpdate(float timeStacker)
            {
                base.GrafUpdate(timeStacker);
                rect.colorFill = Color.Lerp(end1, end2, this.GetValueFloat());
                rect.addSize = new Vector2(4f, 4f) * bumpBehav.AddSize;
                rect.GrafUpdate(timeStacker);
            }

            /// <summary>Physics update tick</summary>
            public override void Update()
            {
                base.Update();
                rect.Update();
            }
        }
    }
}
