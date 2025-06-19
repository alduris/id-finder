using System.Collections.Generic;
using System.Linq;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs.LizardCosmetics
{
    /// <summary>
    /// Holder for <see cref="Subholder"/>s.
    /// </summary>
    public class CosmeticsItemContainer() : IElement, ISaveInHistory
    {
        private readonly List<Subholder> subholders = [];

        /// <summary>Total height of the element.</summary>
        public float Height => subholders.Sum(x => x.Height) + 8f * (subholders.Count - 1);

        /// <summary>Save key</summary>
        public string SaveKey => "Holder";

        /// <summary>Creates elements</summary>
        /// <param name="x">Starting x position</param>
        /// <param name="y">Starting y position</param>
        /// <param name="elements">List to dump created elements in</param>
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

        /// <summary>Recreates inputs from save data</summary>
        /// <param name="data">The JSON representation to recreate from</param>
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

        /// <summary>Turns the inputs into a format convertable to JSON</summary>
        /// <returns>The JSON representation of all subholders</returns>
        public JObject ToSaveData()
        {
            var list = new JObject();
            foreach (var child in subholders)
            {
                list.Add(child.SaveKey, child.ToSaveData());
            }
            return list;
        }

        /// <summary>Generates history tab representation</summary>
        /// <returns>String representation for history tab</returns>
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

        /// <summary>
        /// Adds a subholder to the container
        /// </summary>
        /// <param name="subholder"></param>
        public void Add(Subholder subholder)
        {
            subholders.Add(subholder);
        }

        /// <summary>
        /// Helper method for generating a <see cref="OneOfSubholder"/>. Useful when <c>using static</c> this class.
        /// </summary>
        /// <param name="name">The name of the subholder</param>
        /// <param name="subholders">The child subholders</param>
        /// <returns>The created subholder</returns>
        public static Subholder OneOf(string name, params List<Subholder> subholders) => new OneOfSubholder(name, subholders);

        /// <summary>
        /// Helper method for generating a <see cref="ToggleableSubholder"/>. Useful when <c>using static</c> this class.
        /// </summary>
        /// <param name="name">The name of the subholder</param>
        /// <param name="child">The child subholder</param>
        /// <returns>The created subholder</returns>
        public static Subholder Toggleable(string name, Subholder child) => new ToggleableSubholder(name, child);

        /// <summary>
        /// Helper method for generating a <see cref="MultiItemSubholder"/>. Useful when <c>using static</c> this class.
        /// </summary>
        /// <param name="name">The name of the subholder</param>
        /// <param name="subholders">The child subholders</param>
        /// <returns>The created subholder</returns>
        public static Subholder Group(string name, params List<Subholder> subholders) => new MultiItemSubholder(name, subholders);

        /// <summary>
        /// Generates a <see cref="NoneSubholder"/>. Useful when <c>using static</c> this class.
        /// </summary>
        /// <returns>The created subholder</returns>
        public static Subholder None() => new NoneSubholder();

        /// <summary>
        /// Generates a <see cref="LabelSubholder"/>. Useful when <c>using static</c> this class.
        /// </summary>
        /// <param name="text">The text to put in the subholder</param>
        /// <returns>The created subholder</returns>
        public static Subholder Label(string text) => new LabelSubholder(text);
    }
}
