using System.Linq;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Represents an OpComboBox. However, the value is an int. What does it mean? The index of the selected option as found in the options list passed in.
    /// </summary>
    public class MultiChoiceInput(string name, string[] options, int init = 0) : Input<int>(name, init)
    {
        private readonly string[] options = options;
        private readonly float width = options.Max(x => LabelTest.GetWidth(x)) + 36f;

        public override float InputHeight => 24f;

        protected override UIconfig GetElement(Vector2 pos)
        {
            return new OpComboBox(OpUtil.CosmeticBind(options[value]), pos, width, options);
        }

        protected override int GetValue(UIconfig element)
        {
            var index = options.IndexOf((element as OpComboBox)!.value);
            return index < 0 ? 0 : index;
        }
    }
}
