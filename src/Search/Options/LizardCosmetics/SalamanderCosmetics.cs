using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Search.Options.LizardCosmetics
{
    public class SalamanderCosmetics : BaseLizardCosmetics
    {
        private readonly SpineSpikesCosmetic spineSpikesCosmetic;
        private readonly BumpHawkCosmetic bumpHawkCosmetic;
        private readonly AxolotlGillsCosmetic axolotlGillsCosmetic;
        private readonly TailFinCosmetic tailFinCosmetic;

        private readonly BoolInput melanisticInput;

        public SalamanderCosmetics() : base(LizardType.Salamander)
        {
            cosmetics.Add(axolotlGillsCosmetic = new AxolotlGillsCosmetic());
            cosmetics.Add(tailFinCosmetic = new TailFinCosmetic(type));
            cosmetics.Add(OneOf(
                "Body cosmetic",
                spineSpikesCosmetic = new SpineSpikesCosmetic(type),
                bumpHawkCosmetic = new BumpHawkCosmetic(type),
                None()
                ));

            elements.Add(melanisticInput = new BoolInput("Is melanistic") {
                description = "Regions are able to change the chance of this happening, so for some mod regions this may not be accurate."
            });
        }

        public override float Execute(XORShift128 Random)
        {
            float r = 0f;
            bool body = false;

            foreach (var result in GetResults(Random))
            {
                switch (result)
                {
                    case Melanistic melanistic:
                        r += DistanceIf(melanistic.melanistic, melanisticInput);
                        break;

                    case SpineSpikesVars spineSpikesVars:
                        body = true;
                        r += spineSpikesCosmetic.Distance(spineSpikesVars);
                        break;
                    case BumpHawkVars bumpHawkVars:
                        body = true;
                        r += bumpHawkCosmetic.Distance(bumpHawkVars);
                        break;
                    case AxolotlGillsVars axolotlGillsVars:
                        r += axolotlGillsCosmetic.Distance(axolotlGillsVars);
                        break;

                    case TailFinVars tailFinVars:
                        r += tailFinCosmetic.Distance(tailFinVars);
                        break;

                    case LizardRotVars lizardRotVars:
                        r += lizardRotCosmetic.Distance(lizardRotVars);
                        break;

                    default:
                        throw new InvalidOperationException("Unexpected result! " + result.GetType().Name);
                }
            }

            if (!body && (spineSpikesCosmetic.Active || bumpHawkCosmetic.Active))
            {
                r += MISSING_PENALTY;
            }

            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var (x, y, z, w) = (Random.x, Random.y, Random.z, Random.w);
            foreach (var result in GetResults(Random))
            {
                if (result is Melanistic melanistic)
                {
                    yield return $"Is melanistic: {melanistic.melanistic}";
                    break;
                }
            }

            Random.InitState(x, y, z, w);
            foreach (var result in base.GetValues(Random))
            {
                yield return result;
            }
        }

        internal struct Melanistic(bool melanistic)
        {
            public bool melanistic = melanistic;
        }
    }
}
