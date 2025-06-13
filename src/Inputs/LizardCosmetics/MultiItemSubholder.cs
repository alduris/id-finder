using System.Collections.Generic;
using System.Linq;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs.LizardCosmetics
{
    /// <summary>
    /// Contains a group of <see cref="Subholder"/>s.
    /// </summary>
    public class MultiItemSubholder : Subholder
    {
        /// <summary>Internal group</summary>
        protected readonly Group holdingGroup;
        /// <summary>Child subholders, in list of appropriate type.</summary>
        protected readonly List<Subholder> children;

        /// <summary>
        /// Creates a group of <see cref="Subholder"/>s.
        /// </summary>
        /// <param name="name">Internal name of subholder</param>
        /// <param name="children">List of child subholders</param>
        public MultiItemSubholder(string name, List<Subholder> children) : base(name)
        {
            this.children = children;
            holdingGroup = new Group([.. children.Cast<IElement>()], "Multi item subholder");
            foreach (var child in children)
            {
                child.parent = this;
            }
        }

        /// <summary>Total height of the element.</summary>
        public override float Height => holdingGroup.Height;

        /// <summary>Creates elements</summary>
        /// <param name="x">Starting x position</param>
        /// <param name="y">Starting y position</param>
        /// <param name="elements">List to dump created elements in</param>
        public override void Create(float x, ref float y, List<UIelement> elements)
        {
            holdingGroup.Create(x, ref y, elements);
        }

        /// <summary>Recreates inputs from save data</summary>
        /// <param name="data">The JSON representation to recreate from</param>
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

        /// <summary>Turns the inputs into a format convertable to JSON</summary>
        /// <returns>The JSON representation of this particular subholder</returns>
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

        /// <summary>Generates history tab representation</summary>
        /// <returns>String representation for history tab</returns>
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
