using System.Collections.Generic;
using System.Linq;
using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs.LizardCosmetics
{
    /// <summary>
    /// Allows a <see cref="Subholder"/> to be picked from a list.
    /// </summary>
    public sealed class OneOfSubholder : Subholder, IToggleableChildren
    {
        private readonly MultiChoiceInput selector;
        private readonly List<Subholder> children;
        private Subholder Selected => children[selector.value];

        /// <summary>
        /// Creates a subholder from which only one child can be displayed at a time. Names of the subholders are used as the items in the selection dropdown.
        /// </summary>
        /// <param name="name">Label for dropdown</param>
        /// <param name="children">Children for selection</param>
        public OneOfSubholder(string name, List<Subholder> children) : base(name)
        {
            this.children = children;
            foreach (var child in children)
            {
                child.parent = this;
            }
            selector = new MultiChoiceInput(name, [.. children.Select(x => x.name)], 0) { hasBias = false };
            selector.OnValueChanged += (_, v, o) => { if (v != o) SearchTab.instance.UpdateQueryBox(); };
        }

        /// <summary>Enabled</summary>
        public override bool Enabled => selector.enabled && base.Enabled;
        /// <summary>Total height of the element.</summary>
        public override float Height => selector.Height + (Enabled ? Selected.Height + 6f : 0f);

        /// <summary>Creates elements</summary>
        /// <param name="x">Starting x position</param>
        /// <param name="y">Starting y position</param>
        /// <param name="elements">List to dump created elements in</param>
        public override void Create(float x, ref float y, List<UIelement> elements)
        {
            selector.Create(x, ref y, elements);
            if (Enabled)
            {
                y -= 6f;
                Selected.Create(x, ref y, elements);
            }
        }

        /// <summary>Recreates inputs from save data</summary>
        /// <param name="data">The JSON representation to recreate from</param>
        public override void FromSaveData(JObject data)
        {
            selector.FromSaveData((JObject)data["selector"]!);
            var list = (JObject)data["children"]!;
            foreach (var child in children)
            {
                if (list.ContainsKey(child.name))
                {
                    child.FromSaveData((JObject)list[child.name]!);
                }
            }
        }

        /// <summary>Turns the inputs into a format convertable to JSON</summary>
        /// <returns>The JSON representation of this particular subholder</returns>
        public override JObject ToSaveData()
        {
            var list = new JObject();
            foreach (var child in children)
            {
                list.Add(child.SaveKey, child.ToSaveData());
            }
            return new JObject()
            {
                ["selector"] = selector.ToSaveData(),
                ["children"] = list
            };
        }

        /// <summary>Generates history tab representation</summary>
        /// <returns>String representation for history tab</returns>
        public override IEnumerable<string> GetHistoryLines()
        {
            foreach (var item in children)
            {
                foreach (var str in item.GetHistoryLines())
                {
                    yield return str;
                }
            }
        }

        /// <summary>
        /// Checks if the subholder is a child and is selected.
        /// </summary>
        /// <param name="child">The child subholder</param>
        /// <returns>Returns true if the subholder is a child and is selected.</returns>
        public bool IsChildToggled(Subholder child)
        {
            return Selected == child;
        }
    }
}
