using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Search.Util.LizardUtil;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;

namespace FinderMod.Search.Options.LizardCosmetics
{
    internal class TrainLizardCosmetics : BaseLizardCosmetics
    {
        private readonly LongShoulderScalesCosmetic mainLongShoulderScalesCosmetic;
        private readonly SpineSpikesCosmetic mainSpineSpikesCosmetic;
        private readonly TailFinCosmetic mainTailFinCosmetic;
        private readonly TailTuftCosmetic mainTailTuftCosmetic;

        private readonly SpineSpikesCosmetic spineSpikesCosmetic;
        private readonly BumpHawkCosmetic bumpHawkCosmetic;
        private readonly LongShoulderScalesCosmetic longShoulderScalesCosmetic;
        private readonly ShortBodyScalesCosmetic shortBodyScalesCosmetic;

        private readonly TailTuftCosmetic tailTuftCosmetic;
        private readonly LongHeadScalesCosmetic longHeadScalesCosmetic;

        public TrainLizardCosmetics() : base(LizardType.Train)
        {
            cosmetics.Add(Label("Train-specific cosmetics group"));
            cosmetics.Add(mainLongShoulderScalesCosmetic = new LongShoulderScalesCosmetic(type));
            cosmetics.Add(mainSpineSpikesCosmetic = new SpineSpikesCosmetic(type));
            cosmetics.Add(
                    OneOf(
                        "Tail cosmetic",
                        mainTailFinCosmetic = new TailFinCosmetic(type),
                        mainTailTuftCosmetic = new TailTuftCosmetic(type)
                    )
                );
            cosmetics.Add(Label("Generic cosmetics group"));
            cosmetics.Add(
                OneOf(
                    "Body cosmetic",
                    spineSpikesCosmetic = new SpineSpikesCosmetic(type),
                    bumpHawkCosmetic = new BumpHawkCosmetic(type),
                    longShoulderScalesCosmetic = new LongShoulderScalesCosmetic(type),
                    shortBodyScalesCosmetic = new ShortBodyScalesCosmetic(type),
                    None()
                    )
                );
            cosmetics.Add(Toggleable("Has TailTuft", tailTuftCosmetic = new TailTuftCosmetic(type)));
            cosmetics.Add(Toggleable("Has LongHeadScales", longHeadScalesCosmetic = new LongHeadScalesCosmetic()));
        }

        public override float Execute(XORShift128 Random)
        {
            // Ok this one is tricky because we have to do some kerfuffling because LongShoulderScales and SpineSpikes can appear multiple times but mean different things
            float r = 0f;
            bool body = false;
            bool tail = false;
            bool lhs = false;

            foreach (var result in GetResults(Random))
            {
                switch (result)
                {
                    case SpineSpikesVars spineSpikesVars:
                        if (spineSpikesVars.id == 2)
                        {
                            r += mainSpineSpikesCosmetic.Distance(spineSpikesVars);
                        }
                        else
                        {
                            r += spineSpikesCosmetic.Distance(spineSpikesVars);
                            body = true;
                        }
                        break;
                    case LongShoulderScalesVars longShoulderScalesVars:
                        if (longShoulderScalesVars.id == 4)
                        {
                            r += mainLongShoulderScalesCosmetic.Distance(longShoulderScalesVars);
                        }
                        else
                        {
                            body = true;
                            r += longShoulderScalesCosmetic.Distance(longShoulderScalesVars);
                        }
                        break;
                    case ShortBodyScalesVars shortBodyScalesVars:
                        body = true;
                        r += shortBodyScalesCosmetic.Distance(shortBodyScalesVars);
                        break;
                    case BumpHawkVars bumpHawkVars:
                        body = true;
                        r += bumpHawkCosmetic.Distance(bumpHawkVars);
                        break;

                    case TailFinVars tailFinVars:
                        r += mainTailFinCosmetic.Distance(tailFinVars);
                        break;
                    case TailTuftVars tailTuftVars:
                        if (tailTuftVars.id == 7)
                        {
                            r += mainTailTuftCosmetic.Distance(tailTuftVars);
                        }
                        else
                        {
                            r += tailTuftCosmetic.Distance(tailTuftVars);
                        }
                        break;

                    case LongHeadScalesVars longHeadScalesVars:
                        lhs = true;
                        r += longHeadScalesCosmetic.Distance(longHeadScalesVars);
                        break;

                    case LizardRotVars lizardRotVars:
                        r += lizardRotCosmetic.Distance(lizardRotVars);
                        break;

                    default:
                        throw new InvalidOperationException("Unexpected result! " + result.GetType().Name);
                }
            }

            bool wantedBodyCosmetic = spineSpikesCosmetic.Enabled && spineSpikesCosmetic.Toggled;
            wantedBodyCosmetic |= bumpHawkCosmetic.Enabled && bumpHawkCosmetic.Toggled;
            wantedBodyCosmetic |= longShoulderScalesCosmetic.Enabled && longShoulderScalesCosmetic.Toggled;
            wantedBodyCosmetic |= shortBodyScalesCosmetic.Enabled && shortBodyScalesCosmetic.Toggled;
            if (!body && wantedBodyCosmetic) r += MISSING_PENALTY;

            if (!tail && tailTuftCosmetic.Enabled && tailTuftCosmetic.Toggled) r += MISSING_PENALTY;

            if (!lhs && longHeadScalesCosmetic.Enabled && longHeadScalesCosmetic.Toggled) r += MISSING_PENALTY;

            return r;
        }
    }
}
