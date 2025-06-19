using System.Collections.Generic;
using FinderMod.Search.Options.LizardCosmetics;
using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Inputs.LizardCosmetics
{
    /// <summary>
    /// Special case of <see cref="Subholder"/> containing a rot input type that controls a <see cref="LizardRotCosmetic"/> input group
    /// </summary>
    public sealed class LizardRotSubholder : Subholder
    {
        /// <summary>The rot input</summary>
        public EnumInput<RotType> RotTypeInput;
        /// <summary>The rot cosmetic inputs</summary>
        public LizardRotCosmetic RotCosmeticInput;

        /// <summary>
        /// Creates the subholder. Automatically created in <see cref="BaseLizardCosmetics"/>.
        /// </summary>
        /// <param name="lizardRotCosmetic">Instance of <see cref="LizardRotCosmetic"/> to embed.</param>
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

        /// <summary>Total height of the element.</summary>
        public override float Height => RotTypeInput.Height + 6f + RotCosmeticInput.Height;

        /// <summary>Creates elements</summary>
        /// <param name="x">Starting x position</param>
        /// <param name="y">Starting y position</param>
        /// <param name="elements">List to dump created elements in</param>
        public override void Create(float x, ref float y, List<UIelement> elements)
        {
            RotTypeInput.Create(x, ref y, elements);
            if (RotTypeInput.value != RotType.None)
            {
                y -= 6f;
                RotCosmeticInput.Create(x, ref y, elements);
            }
        }

        /// <summary>Recreates inputs from save data</summary>
        /// <param name="data">The JSON representation to recreate from</param>
        public override void FromSaveData(JObject data)
        {
            RotTypeInput.FromSaveData((JObject)data["type"]!);
            RotCosmeticInput.FromSaveData((JObject)data["cosmetic"]!);
        }

        /// <summary>Turns the inputs into a format convertable to JSON</summary>
        /// <returns>The JSON representation of this particular subholder</returns>
        public override JObject ToSaveData()
        {
            return new JObject()
            {
                ["type"] = RotTypeInput.ToSaveData(),
                ["cosmetic"] = RotCosmeticInput.ToSaveData(),
            };
        }

        /// <summary>Generates history tab representation</summary>
        /// <returns>String representation for history tab</returns>
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
