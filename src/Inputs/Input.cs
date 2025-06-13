using System;
using System.Collections.Generic;
using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace FinderMod.Inputs
{
    /// <summary>
    /// For generic inputs of any type. When it requires a minimum and maximum, use <see cref="RangedInput{T}"/>.
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    /// <param name="name">The name of the input</param>
    /// <param name="init">The initial value</param>
    public abstract class Input<T>(string name, T init) : IElement, ISaveInHistory
    {
        /// <summary>Display name</summary>
        public string name = name;
        /// <summary>Description displayed at bottom of screen</summary>
        public string description = null!;
        private string LabelText => name + (enabled && !inputOnNewLine ? ":  " : ""); // yes the two spaces are intentional
        /// <summary>Enabled state</summary>
        public bool enabled = true;
        /// <summary>Whether or not element is forcefully enabled</summary>
        public bool forceEnabled = false;
        /// <summary>Whether or not actual input created by <see cref="GetElement(Vector2)"/> is placed separately from label</summary>
        protected bool inputOnNewLine = false;

        /// <summary>Actual internal value</summary>
        public T value = init;
        /// <summary>Bias</summary>
        public int bias = 1;
        /// <summary>Whether input has bias dragger</summary>
        public bool hasBias = false;

        /// <summary>Height of the input. Should be larger than 24f.</summary>
        public abstract float InputHeight { get; }

        /// <summary>Calculated overall height.</summary>
        public float Height => inputOnNewLine ? (enabled || forceEnabled ? 28f + InputHeight : 24f) : InputHeight;


        // Element creation

        /// <summary>
        /// Creates the element.
        /// </summary>
        /// <param name="x">What x to start from</param>
        /// <param name="y">What y to start from</param>
        /// <param name="elements">The list of elements to add to</param>
        public void Create(float x, ref float y, List<UIelement> elements)
        {
            y -= inputOnNewLine ? 24f : InputHeight;
            float topY = y;

            if (!forceEnabled)
            {
                var cb = new OpCheckBox(OpUtil.CosmeticBind(enabled), new(x, y + (inputOnNewLine ? 0f : InputHeight / 2f - 12f)));
                cb.OnValueUpdate += ToggleEnable;
                elements.Add(cb);
            }
            else
            {
                enabled = true;
            }

            float cbOffset = forceEnabled ? 0f : 34f;
            elements.Add(new OpLabel(x + cbOffset, y + (inputOnNewLine ? 12f : InputHeight / 2f) - LabelTest._lineHalfHeight, LabelText) { verticalAlignment = OpLabel.LabelVAlignment.Bottom});

            if (enabled)
            {
                if (inputOnNewLine)
                {
                    y -= 4f + InputHeight;
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
                    var biasLabel = new OpLabel(edge - 30f - LabelTest.GetWidth("Bias:"), topY, "Bias:") { verticalAlignment = OpLabel.LabelVAlignment.Center };
                    biasTicker.OnValueUpdate += (_, _, _) => bias = biasTicker.GetValueInt();
                    elements.Add(biasTicker);
                    elements.Add(biasLabel);
                }
            }
        }

        private void ToggleEnable(UIconfig config, string value, string oldValue)
        {
            if (value != oldValue)
            {
                enabled = (config as OpCheckBox)!.GetValueBool();
                UpdateQueryBox();
            }
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

        /// <summary>Returns actual UI element to create</summary>
        /// <param name="pos">Position of UI element</param>
        /// <returns>New instance of element</returns>
        protected abstract UIconfig GetElement(Vector2 pos);

        /// <summary>Gets the value of the element</summary>
        /// <param name="element">Element whose value to check</param>
        /// <returns>Value of the input</returns>
        protected abstract T GetValue(UIconfig element);


        // Helpers

        /// <summary>Helper method. Creates a <see cref="Configurable{T}"/> with current value</summary>
        /// <returns></returns>
        protected Configurable<T> Config() => OpUtil.CosmeticBind(value);

        /// <summary>
        /// Helper method to update query box in search tab
        /// </summary>
        protected void UpdateQueryBox()
        {
            SearchTab.instance.UpdateQueryBox();
        }


        // Save data

        /// <summary>Save key</summary>
        public string SaveKey => name;


        /// <summary>Converts the element into a format convertable to JSON via Newtonsoft.</summary>
        /// <returns>A struct containing the data for easy conversion</returns>
        public virtual JObject ToSaveData()
        {
            return new JObject
            {
                ["enabled"] = enabled,
                ["value"] = JToken.FromObject(value!),
                ["bias"] = bias
            };
        }

        /// <summary>
        /// Returns the element to the state it was given the save state data.
        /// </summary>
        /// <param name="data">The save data to restore data to</param>
        public virtual void FromSaveData(JObject data)
        {
            enabled = (bool)data["enabled"]!;
            value = data["value"]!.ToObject<T>()!;
            bias = (int)data["bias"]!;
        }

        /// <summary>Returns the string representation for the input on the history tab.</summary>
        /// <returns>The strings to represent the input on the history tab.</returns>
        public virtual IEnumerable<string> GetHistoryLines()
        {
            if (enabled)
            {
                yield return $"{name}: {value}" + (bias != 1 ? $" (bias: {bias})" : "");
            }
            yield break;
        }


        // Event thingy

        /// <summary>Delegate definition for input value change</summary>
        /// <param name="input">Reference to input</param>
        /// <param name="value">New value</param>
        /// <param name="oldValue">Previous value</param>
        public delegate void ValueChanged(Input<T> input, T value, T oldValue);
        /// <summary>Called when input value changes</summary>
        public event ValueChanged? OnValueChanged;
    }

    /// <summary>
    /// <see cref="Input{T}"/> type with an explicit minimum and maximum value
    /// </summary>
    /// <typeparam name="T">The value type. Must be a comparable type.</typeparam>
    public abstract class RangedInput<T> : Input<T> where T : IComparable
    {
        /// <summary>Range minimum</summary>
        public T min;
        /// <summary>Range maximum</summary>
        public T max;

        /// <summary>
        /// Creates an input with an explicit minimum and maximum.
        /// </summary>
        /// <param name="name">Name of input</param>
        /// <param name="init">Initial value</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        public RangedInput(string name, T init, T min, T max) : base(name, init)
        {
            hasBias = true;
            this.min = min;
            this.max = max;
        }

        /// <summary>Helper method. Creates a <see cref="Configurable{T}"/> with a <see cref="ConfigAcceptableRange{T}"/></summary>
        /// <returns></returns>
        protected Configurable<T> ConfigRange() => OpUtil.CosmeticRange(value, min, max);
    }
}
