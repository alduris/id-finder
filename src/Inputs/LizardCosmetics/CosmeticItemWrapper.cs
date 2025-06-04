using System.Collections.Generic;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs.LizardCosmetics
{
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
}
