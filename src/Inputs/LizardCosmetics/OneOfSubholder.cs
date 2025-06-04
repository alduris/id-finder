using System.Collections.Generic;
using System.Linq;
using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs.LizardCosmetics
{

    public class OneOfSubholder : Subholder, IToggleableChildren
    {
        private readonly MultiChoiceInput selector;
        private readonly List<Subholder> children;
        internal Subholder Selected => children[selector.value];

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

        public override bool Enabled => selector.enabled && base.Enabled;
        public override float Height => selector.Height + (Enabled ? Selected.Height + 6f : 0f);

        public override void Create(float x, ref float y, List<UIelement> elements)
        {
            selector.Create(x, ref y, elements);
            if (Enabled)
            {
                y -= 6f;
                Selected.Create(x, ref y, elements);
            }
        }

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

        public bool IsChildToggled(Subholder child)
        {
            return Selected == child;
        }
    }
}
