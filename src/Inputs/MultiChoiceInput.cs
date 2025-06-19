using System.Linq;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Combo box input, with value representing selected option.
    /// </summary>
    /// <param name="name">Name of input</param>
    /// <param name="options">List of options</param>
    /// <param name="init">Initial selection</param>
    public class MultiChoiceInput(string name, string[] options, int init = 0) : RangedInput<int>(name, init, 0, options.Length - 1)
    {
        private readonly string[] options = options;
        private readonly float width = options.Max(x => LabelTest.GetWidth(x)) + 42f;

        /// <summary>Height of input.</summary>
        public override float InputHeight => 24f;

        /// <summary>Returns actual UI element to create</summary>
        /// <param name="pos">Position of UI element</param>
        /// <returns>New instance of element</returns>
        protected override UIconfig GetElement(Vector2 pos)
        {
            return new OpComboBox(OpUtil.CosmeticBind(options[value]), pos, width, options);
        }

        /// <summary>Gets the value of the element</summary>
        /// <param name="element">Element whose value to check</param>
        /// <returns>Value of the input</returns>
        protected override int GetValue(UIconfig element)
        {
            var index = options.IndexOf((element as OpComboBox)!.value);
            return index < 0 ? 0 : index;
        }
    }
}
