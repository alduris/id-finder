using System.Collections.Generic;
using System.Linq;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs.LizardCosmetics
{
    public class MultiItemSubholder : Subholder
    {
        protected readonly Group holdingGroup;
        protected readonly List<Subholder> children;

        public MultiItemSubholder(string name, List<Subholder> children) : base(name)
        {
            this.children = children;
            holdingGroup = new Group([.. children.Cast<IElement>()], "Multi item subholder");
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
}
