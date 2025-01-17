﻿using System;
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
    /// <param name="name">The name of the </param>
    /// <param name="init"></param>
    public abstract class Input<T>(string name, T init) : IElement, ISpecialGroupHeight
    {
        public string name = name;
        private string LabelText => name + (enabled && !inputOnNewLine ? ":  " : ""); // yes the two spaces are intentional
        public bool enabled = true;
        protected bool inputOnNewLine = false;

        public T Value { get; protected set; } = init;

        /// <summary>
        /// Height of the input. Should be larger than 24f.
        /// </summary>
        public abstract float Height { get; }

        public float GroupHeight => inputOnNewLine ? (enabled ? 28f + Height : 24f) : Height;


        // Element creation
        public void Create(float x, ref float y, List<UIelement> elements)
        {
            y -= inputOnNewLine ? 24f : Height;

            var cb = new OpCheckBox(OpUtil.CosmeticBind(enabled), new(x, y + (inputOnNewLine ? 0f : Height / 2f - 12f)));
            cb.OnValueUpdate += (_, t, f) =>
            {
                enabled = cb.GetValueBool();
                UpdateQueryBox();
            };

            elements.Add(new OpLabel(x + 34f, y + (inputOnNewLine ? 0f : Height / 2f - LabelTest._lineHalfHeight), LabelText));

            if (enabled)
            {
                if (inputOnNewLine)
                {
                    y -= 4f + Height;
                }
                var elX = x + 34f + (inputOnNewLine ? 0f : LabelTest.GetWidth(LabelText));
                var element = GetElement(new Vector2(elX, y));
                element.OnValueChanged += Element_OnValueChanged;
                elements.Add(element);
            }
        }

        private void Element_OnValueChanged(UIconfig config, string value, string oldValue)
        {
            T oldVal = Value;
            Value = GetValue(config);
            OnValueChanged?.Invoke(this, Value, oldVal);
        }

        protected abstract UIconfig GetElement(Vector2 pos);
        protected abstract T GetValue(UIconfig element);


        // Helpers
        protected Configurable<T> Config() => OpUtil.CosmeticBind(Value);

        private void UpdateQueryBox()
        {
            SearchTab.instance.UpdateQueryBox();
        }

        // Event thingy
        public delegate void ValueChanged(Input<T> input, T value, T oldValue);
        public event ValueChanged OnValueChanged;
    }

    public abstract class RangedInput<T>(string name, T init) : Input<T>(name, init) where T : IComparable
    {
        // Literally just provides the helper
        protected Configurable<T> ConfigRange(T min, T max) => OpUtil.CosmeticRange(Value, min, max);
    }
}
