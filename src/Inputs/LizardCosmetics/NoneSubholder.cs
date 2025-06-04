using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace FinderMod.Inputs.LizardCosmetics
{
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
}
