using System.Collections.Generic;
using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs.LizardCosmetics
{
    /// <summary>
    /// Allows a <see cref="Subholder"/> to be toggled on and off, separately from an enable checkbox.
    /// </summary>
    public class ToggleableSubholder : Subholder, IToggleableChildren
    {
        private readonly BoolInput toggleInput;
        private readonly Subholder child;
        private bool Toggled => toggleInput.value;

        /// <summary>
        /// Creates a subholder from which a child subholder can be toggled on and off.
        /// </summary>
        /// <param name="name">Text to display next to toggle</param>
        /// <param name="child">Child subholder to be toggled</param>
        /// <param name="defaultValue">Default toggle value. True by default</param>
        public ToggleableSubholder(string name, Subholder child, bool defaultValue = true) : base(name)
        {
            this.child = child;
            child.parent = this;
            toggleInput = new BoolInput(name, defaultValue) { hasBias = false };
            toggleInput.OnValueChanged += (_, v, o) => { if (v != o) SearchTab.instance.UpdateQueryBox(); };
        }

        /// <summary>Enabled</summary>
        public override bool Enabled => toggleInput.enabled && base.Enabled;
        /// <summary>Total height of the element.</summary>
        public override float Height => toggleInput.Height + (Enabled ? 6f + child.Height : 0f);

        /// <summary>Creates elements</summary>
        /// <param name="x">Starting x position</param>
        /// <param name="y">Starting y position</param>
        /// <param name="elements">List to dump created elements in</param>
        public override void Create(float x, ref float y, List<UIelement> elements)
        {
            toggleInput.Create(x, ref y, elements);
            if (Enabled && Toggled)
            {
                y -= 6f;
                child.Create(x, ref y, elements);
            }
        }

        /// <summary>Recreates inputs from save data</summary>
        /// <param name="data">The JSON representation to recreate from</param>
        public override void FromSaveData(JObject data)
        {
            toggleInput.FromSaveData((JObject)data["toggle"]!);
            child.FromSaveData((JObject)data["child"]!);
        }

        /// <summary>Turns the inputs into a format convertable to JSON</summary>
        /// <returns>The JSON representation of this particular subholder</returns>
        public override JObject ToSaveData()
        {
            return new JObject()
            {
                ["toggle"] = toggleInput.ToSaveData(),
                ["child"] = child.ToSaveData()
            };
        }

        /// <summary>Generates history tab representation</summary>
        /// <returns>String representation for history tab</returns>
        public override IEnumerable<string> GetHistoryLines()
        {
            foreach (var str in child.GetHistoryLines())
            {
                yield return str;
            }
        }

        /// <summary>
        /// Checks if the subholder is the child and is toggled.
        /// </summary>
        /// <param name="child">The subholder to check</param>
        /// <returns>Returns true if the subholder is the child and is toggled on.</returns>
        public bool IsChildToggled(Subholder child)
        {
            return Toggled && child == this.child;
        }
    }
}
