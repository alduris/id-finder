using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Search.Options.LizardCosmetics
{
    public class IndigoLizardCosmetics : BaseLizardCosmetics
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
                    r += DistanceIf(skinkSpecklesVars.spots, skinkSpecklesCosmetic.NumSpotsInput);
                }
                else if (result is LizardRotVars lizardRotVars)
                {
                    r += DistanceIf(lizardRotVars.numLegs, lizardRotCosmetic.NumTentaclesInput);
                    r += DistanceIf(lizardRotVars.numDeadLegs, lizardRotCosmetic.NumDeadTentaclesInput);
                    r += DistanceIf(lizardRotVars.numEyes, lizardRotCosmetic.NumEyesInput);
                }
                else
                {
                    throw new InvalidOperationException("Result was not SkinkSpeckles! " + result.GetType().Name);
                }
            }
            return r;
        }
    }
}
