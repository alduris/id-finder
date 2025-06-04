using System.Collections.Generic;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;

namespace FinderMod.Inputs.LizardCosmetics
{
    public abstract class Subholder(string name) : IElement, ISaveInHistory
    {
        public readonly string name = name;
        internal protected Subholder parent = null!;
        public virtual bool Enabled => parent == null || (parent.Enabled && parent.IsToggled);

        public bool IsToggled => (parent is IToggleableChildren toggle ? toggle.IsChildToggled(this) : true) && (parent is null || parent.IsToggled);
        public string SaveKey { get; } = name;

        public abstract float Height { get; }

        public abstract void Create(float x, ref float y, List<UIelement> elements);
        public abstract IEnumerable<string> GetHistoryLines();
        public abstract void FromSaveData(JObject data);
        public abstract JObject ToSaveData();

        public static implicit operator Subholder(CosmeticsItem item) => new CosmeticItemWrapper(item);
    }
}
