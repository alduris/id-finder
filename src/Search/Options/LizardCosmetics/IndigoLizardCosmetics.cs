using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Search.Options.LizardCosmetics
{
    internal class IndigoLizardCosmetics : BaseLizardCosmetics
    {
        private readonly SkinkSpecklesCosmetic skinkSpecklesCosmetic;

        public IndigoLizardCosmetics() : base(LizardType.Indigo)
        {
            cosmetics.Add(skinkSpecklesCosmetic = new SkinkSpecklesCosmetic());
        }

        public override float Execute(XORShift128 Random)
        {
            float r = 0f;
            foreach (var result in GetResults(Random))
            {
                if (result is SkinkSpecklesVars skinkSpecklesVars)
                {
                    r += skinkSpecklesCosmetic.Distance(skinkSpecklesVars);
                }
                else if (result is LizardRotVars lizardRotVars)
                {
                    r += lizardRotCosmetic.Distance(lizardRotVars);
                }
                else
                {
                    throw new InvalidOperationException("Unexpected result! " + result.GetType().Name);
                }
            }
            return r;
        }
    }
}
