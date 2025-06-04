using System.Collections.Generic;
using System.Linq;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

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
    }
}
