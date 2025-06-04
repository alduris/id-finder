using System.Collections.Generic;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs.LizardCosmetics
{
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
}
