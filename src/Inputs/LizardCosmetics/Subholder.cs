using System.Collections.Generic;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs.LizardCosmetics
{
    /// <summary>
    /// Specialized container for lizard cosmetics inputs with some special logic relevant to them.
    /// </summary>
    /// <param name="name"></param>
    public abstract class Subholder(string name) : IElement, ISaveInHistory
    {
        /// <summary>Name</summary>
        public readonly string name = name;
        /// <summary>Parent subholder, or null if it is the top subholder</summary>
        internal protected Subholder parent = null!;

        /// <summary>Whether or not the subholder is enabled</summary>
        public virtual bool Enabled => parent == null || (parent.Enabled && parent.IsToggled);

        /// <summary>Checks whether the subholder is toggled, if any of its ancestors are <see cref="IToggleableChildren"/></summary>
        public bool IsToggled => (parent is not IToggleableChildren toggle || toggle.IsChildToggled(this)) && (parent is null || parent.IsToggled);

        /// <summary>Save key</summary>
        public string SaveKey { get; } = name;

        /// <summary>Total height of the element.</summary>
        public abstract float Height { get; }

        /// <summary>Creates elements</summary>
        /// <param name="x">Starting x position</param>
        /// <param name="y">Starting y position</param>
        /// <param name="elements">List to dump created elements in</param>
        public abstract void Create(float x, ref float y, List<UIelement> elements);


        /// <summary>Generates history tab representation</summary>
        /// <returns>String representation for history tab</returns>
        public abstract IEnumerable<string> GetHistoryLines();

        /// <summary>Recreates inputs from save data</summary>
        /// <param name="data">The JSON representation to recreate from</param>
        public abstract void FromSaveData(JObject data);

        /// <summary>Turns the inputs into a format convertable to JSON</summary>
        /// <returns>The JSON representation of this particular subholder</returns>
        public abstract JObject ToSaveData();


        /// <summary>Wraps the <see cref="CosmeticsItem"/> in a <see cref="CosmeticsItemWrapper"/> subholder</summary>
        /// <param name="item">The item to wrap</param>
        public static implicit operator Subholder(CosmeticsItem item) => new CosmeticsItemWrapper(item);
    }
}
