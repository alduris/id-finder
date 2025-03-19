using System;
using System.Collections.Generic;
using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using UnityEngine;

namespace FinderMod.Inputs
{
    /// <summary>
    /// For generic inputs of any type. When it requires a minimum and maximum, use <see cref="RangedInput{T}"/>.
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    /// <param name="name">The name of the input</param>
    /// <param name="init">The initial value</param>
    public abstract class Input<T>(string name, T init) : IElement, ISpecialGroupHeight
    {
        public string name = name;
        public string description = null!;
        private string LabelText => name + (enabled && !inputOnNewLine ? ":  " : ""); // yes the two spaces are intentional
        public bool enabled = true;
        public bool forceEnabled = false;
        protected bool inputOnNewLine = false;

        protected internal T value = init;
        protected internal int bias = 1;
        public bool hasBias = false;

        /// <summary>
        /// Height of the input. Should be larger than 24f.
        /// </summary>
        public abstract float Height { get; }

        public float GroupHeight => inputOnNewLine ? (enabled || forceEnabled ? 28f + Height : 24f) : Height;


        // Element creation
        public void Create(float x, ref float y, List<UIelement> elements)
        {
            y -= inputOnNewLine ? 24f : Height;
            float topY = y;

            if (!forceEnabled)
            {
                var cb = new OpCheckBox(OpUtil.CosmeticBind(enabled), new(x, y + (inputOnNewLine ? 0f : Height / 2f - 12f)));
                cb.OnValueUpdate += ToggleEnable;
                elements.Add(cb);
            }
            else
            {
                enabled = true;
            }

            float cbOffset = forceEnabled ? 0f : 34f;
            elements.Add(new OpLabel(x + cbOffset, y + (inputOnNewLine ? 0f : Height / 2f - LabelTest._lineHalfHeight), LabelText));

            if (enabled)
            {
                if (inputOnNewLine)
                {
                    y -= 4f + Height;
                }
                var elX = x + cbOffset + (inputOnNewLine ? 0f : LabelTest.GetWidth(LabelText));
                var element = GetElement(new Vector2(elX, y));
                element.OnValueChanged += ValueChange;
                if (description != null) element.description = description;
                elements.Add(element);

                // Add bias
                if (hasBias)
                {
                    float edge = 600f - 20f - 20f - Mathf.Floor(x / 10f) * 10f; // screen width - scrollbar width - padding - extra padding for the hell of it
                    var biasTicker = new OpDragger(OpUtil.CosmeticRange(bias, 1, 999), new Vector2(edge - 24f, topY));
                    var biasLabel = new OpLabel(edge - 30f - LabelTest.GetWidth("Bias:"), topY, "Bias:");
                    biasTicker.OnValueUpdate += (_, _, _) => bias = biasTicker.GetValueInt();
                    elements.Add(biasTicker);
                    elements.Add(biasLabel);
                }
            }
        }

        private void ToggleEnable(UIconfig config, string value, string oldValue)
        {
            enabled = (config as OpCheckBox)!.GetValueBool();
            UpdateQueryBox();
        }

        private void ValueChange(UIconfig config, string value, string oldValue)
        {
            if (oldValue != value)
            {
                T oldVal = this.value;
                this.value = GetValue(config);
                OnValueChanged?.Invoke(this, this.value, oldVal);
            }
        }

        protected abstract UIconfig GetElement(Vector2 pos);
        protected abstract T GetValue(UIconfig element);


        // Helpers
        protected Configurable<T> Config() => OpUtil.CosmeticBind(value);

        protected void UpdateQueryBox()
        {
            SearchTab.instance.UpdateQueryBox();
        }

        // Event thingy
        public delegate void ValueChanged(Input<T> input, T value, T oldValue);
        public event ValueChanged? OnValueChanged;
    }

    /// <summary>
    /// <see cref="Input"/> type with some extra helper stuff
    /// </summary>
    /// <typeparam name="T">The value type. Must be a comparable type.</typeparam>
    public abstract class RangedInput<T> : Input<T> where T : IComparable
    {
        public RangedInput(string name, T init) : base(name, init)
        {
            hasBias = true;
        }

        protected Configurable<T> ConfigRange(T min, T max) => OpUtil.CosmeticRange(value, min, max);
    }
}
