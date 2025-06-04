using System.Collections.Generic;
using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs.LizardCosmetics
{
    public class ToggleableSubholder : Subholder, IToggleableChildren
    {
        private readonly BoolInput toggleInput;
        private readonly Subholder child;
        public override bool Enabled => toggleInput.enabled && base.Enabled;
        public bool Toggled => toggleInput.value;

        public ToggleableSubholder(string name, Subholder child, bool defaultValue = true) : base(name)
        {
            this.child = child;
            child.parent = this;
            toggleInput = new BoolInput(name, defaultValue) { hasBias = false };
            toggleInput.OnValueChanged += (_, v, o) => { if (v != o) SearchTab.instance.UpdateQueryBox(); };
        }

        public override float Height => toggleInput.Height + (Enabled ? 6f + child.Height : 0f);

        public override void Create(float x, ref float y, List<UIelement> elements)
        {
            toggleInput.Create(x, ref y, elements);
            if (Enabled && Toggled)
            {
                y -= 6f;
                child.Create(x, ref y, elements);
            }
        }

        public override void FromSaveData(JObject data)
        {
            toggleInput.FromSaveData((JObject)data["toggle"]!);
            child.FromSaveData((JObject)data["child"]!);
        }

        public override JObject ToSaveData()
        {
            return new JObject()
            {
                ["toggle"] = toggleInput.ToSaveData(),
                ["child"] = child.ToSaveData()
            };
        }

        public override IEnumerable<string> GetHistoryLines()
        {
            foreach (var str in child.GetHistoryLines())
            {
                yield return str;
            }
        }

        public bool IsChildToggled(Subholder child)
        {
            return Toggled && child == this.child;
        }
    }
}
