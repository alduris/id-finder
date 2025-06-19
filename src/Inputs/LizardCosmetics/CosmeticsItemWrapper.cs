using System.Collections.Generic;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs.LizardCosmetics
{
    /// <summary>
    /// <see cref="Subholder"/> that holds a <see cref="CosmeticsItem"/>
    /// </summary>
    public class CosmeticsItemWrapper : Subholder
    {
        private readonly CosmeticsItem item;

        /// <summary>
        /// Wraps a 
        /// </summary>
        /// <param name="item"></param>
        public CosmeticsItemWrapper(CosmeticsItem item) : base(item.cosmeticType.ToString())
        {
            this.item = item;
            item.parent = this;
        }

        /// <seealso cref="IElement.Height"/>
        public override float Height => item.Height;

        /// <summary>Creates elements</summary>
        /// <param name="x">Starting x position</param>
        /// <param name="y">Starting y position</param>
        /// <param name="elements">List to dump created elements in</param>
        public override void Create(float x, ref float y, List<UIelement> elements)
        {
            item.Create(x, ref y, elements);
        }

        /// <summary>Recreates inputs from save data</summary>
        /// <param name="data">The JSON representation to recreate from</param>
        public override void FromSaveData(JObject data)
        {
            item.FromSaveData((JObject)data["inner"]!);
        }

        /// <summary>Turns the inputs into a format convertable to JSON</summary>
        /// <returns>The JSON representation of this particular subholder</returns>
        public override JObject ToSaveData()
        {
            return new JObject()
            {
                ["inner"] = item.ToSaveData()
            };
        }

        /// <summary>Generates history tab representation</summary>
        /// <returns>String representation for history tab</returns>
        public override IEnumerable<string> GetHistoryLines()
        {
            foreach (var str in item.GetHistoryLines())
            {
                yield return str;
            }
        }
    }
}
