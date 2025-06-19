using System.Collections.Generic;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs.LizardCosmetics
{
    /// <summary>
    /// Subholder containing a <see cref="Label"/>.
    /// </summary>
    /// <param name="text">The text to put in the label</param>
    public sealed class LabelSubholder(string text) : Subholder(text)
    {
        private readonly Label label = new(text);

        /// <summary>Total height of the element.</summary>
        public override float Height => label.Height;

        /// <summary>
        /// Creates the label.
        /// </summary>
        /// <param name="x">The starting x position</param>
        /// <param name="y">The starting y position</param>
        /// <param name="elements">List to dump created elements in</param>
        public override void Create(float x, ref float y, List<UIelement> elements)
        {
            label.Create(x, ref y, elements);
        }

        /// <summary>
        /// Does nothing, as labels have no save data.
        /// </summary>
        public override void FromSaveData(JObject data)
        {
        }

        /// <summary>
        /// Returns an empty object, as labels have no save data.
        /// </summary>
        public override JObject ToSaveData()
        {
            return [];
        }

        /// <summary>
        /// Does nothing, as labels have no data to display.
        /// </summary>
        public override IEnumerable<string> GetHistoryLines()
        {
            yield break;
        }
    }
}
