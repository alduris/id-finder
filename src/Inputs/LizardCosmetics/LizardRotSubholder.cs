using System.Collections.Generic;
using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Inputs.LizardCosmetics
{
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
            return new JObject()
            {
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
