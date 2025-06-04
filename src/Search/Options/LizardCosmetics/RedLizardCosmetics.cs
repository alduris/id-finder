using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Search.Util.LizardUtil;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;

namespace FinderMod.Search.Options.LizardCosmetics
{
    public class RedLizardCosmetics : BaseLizardCosmetics
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

        public RedLizardCosmetics() : base(LizardType.Red)
        {
            cosmetics.Add(Label("Red-specific cosmetics group"));
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
            bool foundLSS = false;
            LongShoulderScalesVars specificLSS = default;

            foreach (var result in GetResults(Random))
            {
                switch (result)
                {
                    case SpineSpikesVars spineSpikesVars:
                        if (body || foundLSS)
                        {
                            // We found the LSS prior so we know that this is the type-specific one
                            r += mainSpineSpikesCosmetic.Distance(spineSpikesVars);
                        }
                        else
                        {
                            // We know it's body here because the type-specific LSS comes before the type-specific spinespikes
                            // which means that either body or foundLSS must be true prior
                            body = true;
                            r += spineSpikesCosmetic.Distance(spineSpikesVars);
                        }
                        break;
                    case LongShoulderScalesVars longShoulderScalesVars:
                        if (foundLSS)
                        {
                            // We found an LSS prior so we know this one is the type-specific one.
                            // Since we deal with the type-specific one afterwards, we deal with the previous one now
                            body = true;
                            r += longShoulderScalesCosmetic.Distance(specificLSS);
                        }
                        else
                        {
                            // Either this is the type-specific one or it isn't, if it isn't then we deal with this when we reach the type-specific one in the if block.
                            foundLSS = true;
                        }
                        // You may have noticed we don't deal with the type-specific cosmetic. This is done after the foreach and is kept with this.
                        specificLSS = longShoulderScalesVars;
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
                        {
                            var cosmetic = foundLSS ? mainTailTuftCosmetic : tailTuftCosmetic;
                            if (!foundLSS) tail = true;
                            cosmetic.Distance(tailTuftVars);
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

            // Deal with type-specific LSS. We know that this is not default because the type-specific LSS is guaranteed.
            r += mainLongShoulderScalesCosmetic.Distance(specificLSS);

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
