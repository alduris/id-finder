using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Yes/No input
    /// </summary>
    public class BoolInput : Input<bool>
    {
        /// <summary>Height of input.</summary>
        public override float InputHeight => 24f;

        /// <summary>
        /// Creates a yes/no input
        /// </summary>
        /// <param name="name">Name to display</param>
        /// <param name="init">Initial value</param>
        public BoolInput(string name, bool init = false) : base(name, init)
        {
            hasBias = true;
        }

        /// <summary>Returns actual UI element to create</summary>
        /// <param name="pos">Position of UI element</param>
        /// <returns>New instance of element</returns>
        protected override UIconfig GetElement(Vector2 pos) => new OpLabelCheckbox(Config(), pos);

        /// <summary>Gets the value of the element</summary>
        /// <param name="element">Element whose value to check</param>
        /// <returns>Value of the input</returns>
        protected override bool GetValue(UIconfig element) => (element as OpLabelCheckbox)!.boolValue;

        /// <summary>
        /// Variant of <see cref="OpCheckBox"/> that displays "Yes" or "No" as text
        /// </summary>
        public class OpLabelCheckbox : OpCheckBox
        {
            /// <summary>Current bool value, without needing the extension method in <see cref="IValueBool"/></summary>
            public bool boolValue;
            private readonly FLabel label;

            /// <summary>
            /// Creates a checkbox element with Yes/No instead of a symbol
            /// </summary>
            /// <param name="config">Configurable</param>
            /// <param name="pos">Position of bottom left</param>
            public OpLabelCheckbox(Configurable<bool> config, Vector2 pos) : base(config, pos)
            {
                _size = new Vector2(40f, 24f);
                fixedSize = size;
                symbolSprite.RemoveFromContainer();
                label = new FLabel(LabelTest.GetFont(false), BaseTab.Translate(config.Value ? "Yes" : "No"))
                {
                    anchorX = 0.5f,
                    anchorY = 0.5f,
                    x = 20f,
                    y = 12f
                };

                foreach (var sprite in rect.sprites)
                {
                    sprite.RemoveFromContainer();
                }
                rect = new DyeableRect(myContainer, Vector2.zero, size, true);

                myContainer.AddChild(label); // add after rectangle so it goes on top

                OnValueChanged += UpdateText;
                boolValue = config.Value;
            }

            private void UpdateText(UIconfig config, string value, string oldValue)
            {
                boolValue = this.GetValueBool();
                label.text = this.GetValueBool() ? "Yes" : "No";
            }

            /// <summary>Performs a graphics update</summary>
            /// <param name="timeStacker">Time between physics ticks, range 0 to 1</param>
            public override void GrafUpdate(float timeStacker)
            {
                base.GrafUpdate(timeStacker);

                label.color = bumpBehav.GetColor(colorEdge);
            }
        }
    }
}
