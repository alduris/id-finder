using System;
using System.Collections.Generic;
using System.Linq;
using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Inputs.LizardCosmetics
{
    public class CosmeticsItemContainer : IElement, ISaveInHistory
    {
        private readonly List<Subholder> subholders = [];

        public CosmeticsItemContainer() { }

        public float Height => subholders.Sum(x => x.Height) + 8f * (subholders.Count - 1);

        public string SaveKey => "Holder";

        public void Create(float x, ref float y, List<UIelement> elements)
        {
            if (subholders.Count > 0)
            {
                y += 8f;
                foreach (var subholder in subholders)
                {
                    y -= 8f;
                    subholder.Create(x, ref y, elements);
                }
            }
        }

        public void FromSaveData(JObject data)
        {
            HashSet<Subholder> saveable = [.. subholders];
            foreach (var kvp in data)
            {
                var child = saveable.FirstOrDefault(x => x.SaveKey == kvp.Key);
                if (child != null)
                {
                    child.FromSaveData((kvp.Value as JObject)!);
                    saveable.Remove(child);
                }
            }
        }

        public JObject ToSaveData()
        {
            var list = new JObject();
            foreach (var child in subholders)
            {
                list.Add(child.SaveKey, child.ToSaveData());
            }
            return list;
        }

        public IEnumerable<string> GetHistoryLines()
        {
            foreach (var child in subholders)
            {
                foreach (var str in child.GetHistoryLines())
                {
                    yield return str;
                }
            }
        }

        public void Add(Subholder subholder)
        {
            subholders.Add(subholder);
        }

        public static Subholder OneOf(string name, params List<Subholder> subholders) => new OneOfSubholder(name, subholders);
        public static Subholder Toggleable(string name, Subholder child) => new ToggleableSubholder(name, child);
        public static Subholder Group(string name, params List<Subholder> subholders) => new MultiItemSubholder(name, subholders);
        public static Subholder None() => new NoneSubholder();
        public static Subholder Label(string text) => new LabelSubholder(text);

        public abstract class Subholder(string name) : IElement, ISaveInHistory
        {
            public readonly string name = name;
            internal protected Subholder parent = null!;
            public virtual bool Enabled => parent == null || (parent.Enabled && parent.IsToggled);
            // public virtual bool Enabled => parent == null || (parent.Enabled && (parent.parent is not OneOfSubholder oneOf || oneOf.Selected == parent));
            public bool IsToggled => (parent is not ToggleableSubholder toggleable || toggleable.Toggled) && (parent is not OneOfSubholder oneOf || oneOf.Selected == this) && (parent is null || parent.IsToggled);
            public string SaveKey { get; } = name;

            public abstract float Height { get; }

            public abstract void Create(float x, ref float y, List<UIelement> elements);
            public abstract IEnumerable<string> GetHistoryLines();
            public abstract void FromSaveData(JObject data);
            public abstract JObject ToSaveData();

            public static implicit operator Subholder(CosmeticsItem item) => new CosmeticItemWrapper(item);
        }

        public class OneOfSubholder : Subholder
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
        }

        public class ToggleableSubholder : Subholder
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
        }

        public class MultiItemSubholder : Subholder
        {
            protected readonly Group holdingGroup;
            protected readonly List<Subholder> children;

            public MultiItemSubholder(string name, List<Subholder> children) : base(name)
            {
                this.children = children;
                holdingGroup = new Group(children.Cast<IElement>().ToList(), "Multi item subholder");
                foreach (var child in children)
                {
                    child.parent = this;
                }
            }

            public override float Height => holdingGroup.Height;

            public override void Create(float x, ref float y, List<UIelement> elements)
            {
                holdingGroup.Create(x, ref y, elements);
            }

            public override void FromSaveData(JObject data)
            {
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
                    ["children"] = list
                };
            }

            public override IEnumerable<string> GetHistoryLines()
            {
                foreach (var child in children)
                {
                    foreach (var str in child.GetHistoryLines())
                    {
                        yield return str;
                    }
                }
            }
        }

        public class CosmeticItemWrapper : Subholder
        {
            private readonly CosmeticsItem item;

            public CosmeticItemWrapper(CosmeticsItem item) : base(item.cosmeticType.ToString())
            {
                this.item = item;
                item.parent = this;
            }

            public override float Height => item.Height;

            public override void Create(float x, ref float y, List<UIelement> elements)
            {
                item.Create(x, ref y, elements);
            }

            public override void FromSaveData(JObject data)
            {
                item.FromSaveData((JObject)data["inner"]!);
            }

            public override JObject ToSaveData()
            {
                return new JObject()
                {
                    ["inner"] = item.ToSaveData()
                };
            }

            public override IEnumerable<string> GetHistoryLines()
            {
                foreach (var str in item.GetHistoryLines())
                {
                    yield return str;
                }
            }
        }

        public class NoneSubholder : Subholder
        {
            private readonly Group group;

            public NoneSubholder() : base("NONE")
            {
                group = new Group([new Label("No cosmetic")], "None");
            }

            public override float Height => group.Height;

            public override void Create(float x, ref float y, List<UIelement> elements)
            {
                group.Create(x, ref y, elements);
            }

            public override void FromSaveData(JObject data)
            {
            }

            public override JObject ToSaveData()
            {
                return [];
            }

            public override IEnumerable<string> GetHistoryLines()
            {
                yield return "No cosmetic";
            }
        }

        public class LabelSubholder(string text) : Subholder(text)
        {
            private readonly Label label = new(text);

            public override float Height => label.Height;

            public override void Create(float x, ref float y, List<UIelement> elements)
            {
                label.Create(x, ref y, elements);
            }

            public override void FromSaveData(JObject data)
            {
            }

            public override JObject ToSaveData()
            {
                return [];
            }

            public override IEnumerable<string> GetHistoryLines()
            {
                yield break;
            }
        }

        public class LizardRotSubholder : Subholder
        {
            public EnumInput<RotType> RotTypeInput;
            public LizardRotCosmetic RotCosmeticInput;

            public LizardRotSubholder(LizardRotCosmetic lizardRotCosmetic) : base("Lizard rot")
            {
                RotTypeInput = new("Rot state", RotType.None) { forceEnabled = true };
                RotTypeInput.OnValueChanged += (_, v, o) =>
                {
                    bool flag = (v == RotType.None || o == RotType.None);
                    if (v != o && flag) SearchTab.instance.UpdateQueryBox();
                };
                RotCosmeticInput = lizardRotCosmetic;
            }

            public override float Height => RotTypeInput.Height + 6f + RotCosmeticInput.Height;

            public override void Create(float x, ref float y, List<UIelement> elements)
            {
                RotTypeInput.Create(x, ref y, elements);
                if (RotTypeInput.value != RotType.None)
                {
                    y -= 6f;
                    RotCosmeticInput.Create(x, ref y, elements);
                }
            }

            public override void FromSaveData(JObject data)
            {
                RotTypeInput.FromSaveData((JObject)data["type"]!);
                RotCosmeticInput.FromSaveData((JObject)data["cosmetic"]!);
            }

            public override JObject ToSaveData()
            {
                return new JObject() {
                    ["type"] = RotTypeInput.ToSaveData(),
                    ["cosmetic"] = RotCosmeticInput.ToSaveData(),
                };
            }

            public override IEnumerable<string> GetHistoryLines()
            {
                yield return $"Rot type: {RotTypeInput.value}";
                if (RotTypeInput.value != RotType.None)
                {
                    foreach (var line in RotCosmeticInput.GetHistoryLines())
                    {
                        yield return line;
                    }
                }
            }
        }
    }
}
