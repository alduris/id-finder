using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Search.Util.LizardUtil;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;

namespace FinderMod.Search.Options.LizardCosmetics
{
    internal class GreenLizardCosmetics : BaseLizardCosmetics
    {
        private readonly SpineSpikesCosmetic spineSpikesCosmetic;
        private readonly LongShoulderScalesCosmetic longShoulderScalesCosmetic1;
        private readonly ShortBodyScalesCosmetic shortBodyScalesCosmetic;
        private readonly BumpHawkCosmetic bumpHawkCosmetic;
        private readonly TailTuftCosmetic tailTuftCosmetic;
        private readonly LongShoulderScalesCosmetic longShoulderScalesCosmetic2;
        private readonly LongHeadScalesCosmetic longHeadScalesCosmetic;

        public GreenLizardCosmetics() : base(LizardType.Green)
        {
            cosmetics.Add(
                OneOf(
                    "Body cosmetic",
                    spineSpikesCosmetic = new SpineSpikesCosmetic(type),
                    bumpHawkCosmetic = new BumpHawkCosmetic(type),
                    longShoulderScalesCosmetic1 = new LongShoulderScalesCosmetic(type),
                    shortBodyScalesCosmetic = new ShortBodyScalesCosmetic(type),
                    None()
                    )
                );
            cosmetics.Add(
                OneOf(
                    "Tail cosmetic",
                    tailTuftCosmetic = new TailTuftCosmetic(type),
                    longShoulderScalesCosmetic2 = new LongShoulderScalesCosmetic(type),
                    None()
                    )
                );
            cosmetics.Add(Toggleable("Has LongHeadScales", longHeadScalesCosmetic = new LongHeadScalesCosmetic()));
        }

        public override float Execute(XORShift128 Random)
        {
            float r = 0f;
            bool body = false;
            bool tail = false;
            bool lhs = false;

            LongShoulderScalesCosmetic otherLSS = null!;

            foreach (var result in GetResults(Random))
            {
                switch (result)
                {
                    case SpineSpikesVars spineSpikesVars:
                        body = true;
                        r += spineSpikesCosmetic.Distance(spineSpikesVars);
                        break;
                    case BumpHawkVars bumpHawkVars:
                        body = true;
                        r += bumpHawkCosmetic.Distance(bumpHawkVars);
                        break;
                    case LongShoulderScalesVars longShoulderScalesVars:
                        {
                            // This code is more complex to account for the fact that the user does not necessarily know which order these will come in
                            bool tryBoth = true;
                            if (!body)
                            {
                                body = true;
                            }
                            else
                            {
                                tail = true;
                                if (otherLSS != null)
                                {
                                    r += otherLSS.Distance(longShoulderScalesVars);
                                    tryBoth = false;
                                }
                            }

                            if (tryBoth)
                            {
                                float a = longShoulderScalesCosmetic1.Distance(longShoulderScalesVars);
                                float b = longShoulderScalesCosmetic2.Distance(longShoulderScalesVars);

                                if (a <= b && longShoulderScalesCosmetic1.Enabled)
                                {
                                    otherLSS = longShoulderScalesCosmetic2;
                                    r += a;
                                }
                                else
                                {
                                    otherLSS = longShoulderScalesCosmetic1;
                                    r += b;
                                }
                            }
                        }
                        break;
                    case ShortBodyScalesVars shortBodyScalesVars:
                        body = true;
                        r += shortBodyScalesCosmetic.Distance(shortBodyScalesVars);
                        break;

                    case TailTuftVars tailTuftVars:
                        tail = true;
                        r += tailTuftCosmetic.Distance(tailTuftVars);
                        break;

                    case LongHeadScalesVars longHeadScalesVars:
                        lhs = true;
                        r += longHeadScalesCosmetic.Distance(longHeadScalesVars);
                        break;

                    case LizardRotVars lizardRotVars:
                        r += DistanceIf(lizardRotVars.numLegs, lizardRotCosmetic.NumTentaclesInput);
                        r += DistanceIf(lizardRotVars.numDeadLegs, lizardRotCosmetic.NumDeadTentaclesInput);
                        r += DistanceIf(lizardRotVars.numEyes, lizardRotCosmetic.NumEyesInput);
                        break;

                    default:
                        throw new InvalidOperationException("Unexpected result! " + result.GetType().Name);
                }
            }

            bool wantedBodyCosmetic = spineSpikesCosmetic.Enabled && spineSpikesCosmetic.Toggled;
            wantedBodyCosmetic |= bumpHawkCosmetic.Enabled && bumpHawkCosmetic.Toggled;
            wantedBodyCosmetic |= longShoulderScalesCosmetic1.Enabled && longShoulderScalesCosmetic1.Toggled;
            wantedBodyCosmetic |= shortBodyScalesCosmetic.Enabled && shortBodyScalesCosmetic.Toggled;
            if (wantedBodyCosmetic && !body) r += MISSING_PENALTY;

            bool wantedTailCosmetic = tailTuftCosmetic.Enabled && tailTuftCosmetic.Toggled;
            wantedTailCosmetic |= longShoulderScalesCosmetic2.Enabled && longShoulderScalesCosmetic2.Toggled;
            if (wantedTailCosmetic && !tail) r += MISSING_PENALTY;

            if (!lhs && longHeadScalesCosmetic.Enabled && longHeadScalesCosmetic.Toggled) r += MISSING_PENALTY;

            return r;
        }
    }
}
