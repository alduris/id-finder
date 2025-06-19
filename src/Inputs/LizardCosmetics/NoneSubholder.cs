using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace FinderMod.Inputs.LizardCosmetics
{
    /// <summary>
    /// Subholder for the special case of no cosmetic. Contains a special message in a wrapper group.
    /// </summary>
    public sealed class NoneSubholder : Subholder
    {
        private readonly Group group;

        /// <summary>
        /// Creates a none subholder
        /// </summary>
        public NoneSubholder() : base("NONE")
        {
            group = new Group([new Label("No cosmetic")], "None");
        }

        /// <summary>Total height of the element.</summary>
        public override float Height => group.Height;

        /// <summary>Creates elements</summary>
        /// <param name="x">Starting x position</param>
        /// <param name="y">Starting y position</param>
        /// <param name="elements">List to dump created elements in</param>
        public override void Create(float x, ref float y, List<UIelement> elements)
        {
            group.Create(x, ref y, elements);
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
        /// Returns "No cosmetic" to display on the history tab.
        /// </summary>
        public override IEnumerable<string> GetHistoryLines()
        {
            yield return "No cosmetic";
        }
    }
}
