using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Search.Util.LizardUtil;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;

namespace FinderMod.Search.Options.LizardCosmetics
{
    public class YellowLizardCosmetics : BaseLizardCosmetics
    {
        private readonly AntennaeCosmetic antennaeCosmetic;

        private readonly SpineSpikesCosmetic spineSpikesCosmetic;
        private readonly BumpHawkCosmetic bumpHawkCosmetic;
        private readonly LongShoulderScalesCosmetic longShoulderScalesCosmetic;
        private readonly ShortBodyScalesCosmetic shortBodyScalesCosmetic;

        private readonly TailTuftCosmetic tailTuftCosmetic;

        public YellowLizardCosmetics() : base(LizardType.Yellow)
        {
            cosmetics.Add(antennaeCosmetic = new AntennaeCosmetic());
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
                    case SpineSpikesVars spineSpikesVars:
                        body = true;
                        if (spineSpikesCosmetic.Active)
                        {
                            r += DistanceIf(spineSpikesVars.spineLength, spineSpikesCosmetic.LengthInput);
                            r += DistanceIf(spineSpikesVars.numScales, spineSpikesCosmetic.NumScalesInput);
                            r += DistanceIf(spineSpikesVars.graphic, spineSpikesCosmetic.GraphicInput);
                        }
                        else if (spineSpikesCosmetic.Enabled && !spineSpikesCosmetic.Toggled)
                        {
                            r += MISSING_PENALTY;
                        }
                        break;
                    case LongShoulderScalesVars longShoulderScalesVars:
                        body = true;
                        if (longShoulderScalesCosmetic.Active)
                        {
                            r += DistanceIf(longShoulderScalesVars.minSize, longShoulderScalesCosmetic.MinSizeInput);
                            r += DistanceIf(longShoulderScalesVars.maxSize, longShoulderScalesCosmetic.MaxSizeInput);
                            r += DistanceIf(longShoulderScalesVars.numScales, longShoulderScalesCosmetic.NumScalesInput);
                            r += DistanceIf(longShoulderScalesVars.graphic, longShoulderScalesCosmetic.GraphicInput);
                            if (longShoulderScalesCosmetic.ScaleTypeInput.enabled && longShoulderScalesCosmetic.ScaleTypeInput.value != longShoulderScalesVars.scaleType)
                                r += longShoulderScalesCosmetic.ScaleTypeInput.bias;
                            r += DistanceIf(longShoulderScalesVars.colored, longShoulderScalesCosmetic.ColoredInput);
                        }
                        else if (longShoulderScalesCosmetic.Enabled && !longShoulderScalesCosmetic.Toggled)
                        {
                            r += MISSING_PENALTY;
                        }
                        break;
                    case ShortBodyScalesVars shortBodyScalesVars:
                        body = true;
                        if (shortBodyScalesCosmetic.Active)
                        {
                            r += DistanceIf(shortBodyScalesVars.numScales, shortBodyScalesCosmetic.NumScalesInput);
                            if (shortBodyScalesCosmetic.ScaleTypeInput.enabled && shortBodyScalesCosmetic.ScaleTypeInput.value != shortBodyScalesVars.scaleType)
                                r += shortBodyScalesCosmetic.ScaleTypeInput.bias;
                        }
                        else if (shortBodyScalesCosmetic.Enabled && !shortBodyScalesCosmetic.Toggled)
                        {
                            r += MISSING_PENALTY;
                        }
                        break;
                    case BumpHawkVars bumpHawkVars:
                        body = true;
                        if (bumpHawkCosmetic.Active)
                        {
                            // len num col
                            r += DistanceIf(bumpHawkVars.spineLength, bumpHawkCosmetic.SpineLenInput);
                            r += DistanceIf(bumpHawkVars.numBumps, bumpHawkCosmetic.NumBumpsInput);
                            r += DistanceIf(bumpHawkVars.colored, bumpHawkCosmetic.ColoredInput);
                        }
                        else if (bumpHawkCosmetic.Enabled && !bumpHawkCosmetic.Toggled)
                        {
                            r += MISSING_PENALTY;
                        }
                        break;

                    case TailTuftVars tailTuftVars:
                        tail = true;
                        if (tailTuftCosmetic.Active)
                        {
                            r += DistanceIf(tailTuftVars.numScales, tailTuftCosmetic.NumScalesInput);
                            r += DistanceIf(tailTuftVars.scaleType, tailTuftCosmetic.ScaleTypeInput);
                        }
                        else if (tailTuftCosmetic.Enabled && !tailTuftCosmetic.Toggled)
                        {
                            r += MISSING_PENALTY;
                        }
                        break;

                    case AntennaeVars antennaeVars:
                        r += DistanceIf(antennaeVars.length, antennaeCosmetic.LengthInput);
                        r += DistanceIf(antennaeVars.alpha, antennaeCosmetic.AlphaInput);
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected result! " + result.GetType().Name);
                }
            }

            bool wantedBodyCosmetic = spineSpikesCosmetic.Enabled && spineSpikesCosmetic.Toggled;
            wantedBodyCosmetic |= longShoulderScalesCosmetic.Enabled && longShoulderScalesCosmetic.Toggled;
            wantedBodyCosmetic |= shortBodyScalesCosmetic.Enabled && shortBodyScalesCosmetic.Toggled;
            wantedBodyCosmetic |= bumpHawkCosmetic.Enabled && bumpHawkCosmetic.Toggled;
            if (!body && wantedBodyCosmetic) r += MISSING_PENALTY;

            bool wantedTailCosmetic = tailTuftCosmetic.Enabled && tailTuftCosmetic.Toggled;
            if (!tail && wantedTailCosmetic) r += MISSING_PENALTY;

            return r;
        }
    }
}
