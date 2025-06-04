using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Search.Options.LizardCosmetics
{
    public class WhiteLizardCosmetics : BaseLizardCosmetics
    {
        private readonly BumpHawkCosmetic bumpHawkCosmetic;
        private readonly ShortBodyScalesCosmetic shortBodyScalesCosmetic;
        private readonly LongShoulderScalesCosmetic longShoulderScalesCosmetic;
        private readonly LongHeadScalesCosmetic longHeadScalesCosmetic;
        private readonly TailTuftCosmetic tailTuftCosmetic;

        public WhiteLizardCosmetics() : base(LizardType.White)
        {
            cosmetics.Add(
                OneOf(
                    "Body cosmetic",
                    bumpHawkCosmetic = new BumpHawkCosmetic(type),
                    shortBodyScalesCosmetic = new ShortBodyScalesCosmetic(type),
                    longShoulderScalesCosmetic = new LongShoulderScalesCosmetic(type),
                    longHeadScalesCosmetic = new LongHeadScalesCosmetic(),
                    None()
                    )
                );
            cosmetics.Add(Toggleable("Has tail tuft", tailTuftCosmetic = new TailTuftCosmetic(type)));
        }

        public override float Execute(XORShift128 Random)
        {
            float r = 0f;
            bool body = false;
            bool tail = false;

            foreach (var result in GetResults(Random))
            {
                switch (result)
                {
                    case BumpHawkVars bumpHawkVars:
                        body = true;
                        r += bumpHawkCosmetic.Distance(bumpHawkVars);
                        break;
                    case ShortBodyScalesVars shortBodyScalesVars:
                        body = true;
                        r += shortBodyScalesCosmetic.Distance(shortBodyScalesVars);
                        break;
                    case LongShoulderScalesVars longShoulderScalesVars:
                        body = true;
                        r += longShoulderScalesCosmetic.Distance(longShoulderScalesVars);
                        break;
                    case LongHeadScalesVars longHeadScalesVars:
                        body = true;
                        r += longHeadScalesCosmetic.Distance(longHeadScalesVars);
                        break;

                    case TailTuftVars tailTuftVars:
                        tail = true;
                        r += tailTuftCosmetic.Distance(tailTuftVars);
                        break;

                    case LizardRotVars lizardRotVars:
                        r += lizardRotCosmetic.Distance(lizardRotVars);
                        break;

                    default:
                        throw new InvalidOperationException("Unexpected result! " + result.GetType().Name);
                }
            }

            bool wantedBodyCosmetic = bumpHawkCosmetic.Enabled && bumpHawkCosmetic.Toggled;
            wantedBodyCosmetic |= shortBodyScalesCosmetic.Enabled && shortBodyScalesCosmetic.Toggled;
            wantedBodyCosmetic |= longShoulderScalesCosmetic.Enabled && longShoulderScalesCosmetic.Toggled;
            wantedBodyCosmetic |= longHeadScalesCosmetic.Enabled && longHeadScalesCosmetic.Toggled;
            if (!body && wantedBodyCosmetic) r += MISSING_PENALTY;
            if (!tail && tailTuftCosmetic.Enabled && tailTuftCosmetic.Toggled) r += MISSING_PENALTY;

            return r;
        }
    }
}
