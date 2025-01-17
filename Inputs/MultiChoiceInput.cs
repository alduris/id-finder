using System.Linq;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Represents an OpComboBox. However, the value is an int. What does it mean? The index of the selected option as found in the options list passed in.
    /// </summary>
    public class MultiChoiceInput : Input<int>
    {
        private readonly string[] options;
        private readonly float width;

        public MultiChoiceInput(string name, string[] options, int init) : base(name, init)
        {
            this.options = options;
            width = options.Max(x => LabelTest.GetWidth(x)) + 30f;
        }

        public override float Height => 24f;

        protected override UIconfig GetElement(Vector2 pos)
        {
            var el = new OpComboBox(OpUtil.CosmeticBind(options[Value]), pos, width, options);
            el.OnListOpen += El_OnListOpen;
            return el;
        }

        private void El_OnListOpen(UIfocusable self)
        {
            self.MoveToFront();
        }

        protected override int GetValue(UIconfig element)
        {
            var index = options.IndexOf((element as OpComboBox).value);
            return index < 0 ? 0 : index;
        }
    }
}
