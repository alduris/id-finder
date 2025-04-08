using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Search.Util.LizardUtil;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;

namespace FinderMod.Search.Options.LizardCosmetics
{
    public class ZoopLizardCosmetics : BaseLizardCosmetics
    {
        private readonly WingScalesCosmetic wingScalesCosmetic;
        private readonly SpineSpikesCosmetic mainSpineSpikesCosmetic;
        private readonly TailTuftCosmetic mainTailTuftCosmetic;

        private readonly SpineSpikesCosmetic spineSpikesCosmetic;
        private readonly BumpHawkCosmetic bumpHawkCosmetic;
        private readonly LongShoulderScalesCosmetic longShoulderScalesCosmetic;
        private readonly ShortBodyScalesCosmetic shortBodyScalesCosmetic;

        private readonly TailTuftCosmetic tailTuftCosmetic;

        private readonly LongHeadScalesCosmetic longHeadScalesCosmetic;

        public ZoopLizardCosmetics() : base(LizardType.Zoop)
        {
            cosmetics.Add(Label("Zoop-specific cosmetics group"));
            cosmetics.Add(
                OneOf(
                    "Zoop-specific cosmetic",
                    wingScalesCosmetic = new WingScalesCosmetic(),
                    mainSpineSpikesCosmetic = new SpineSpikesCosmetic(type)
                    )
                );
            cosmetics.Add(mainTailTuftCosmetic = new TailTuftCosmetic(type));

            // normal lizard cases
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
            float r = 0f;
            bool finishedZoopSpecific = false;
            bool body = false;
            bool tail = false;
            bool lhs = false;

            foreach (var result in GetResults(Random))
            {
                switch (result)
                {
                    case WingScalesVars wingScalesVars:
                        if (wingScalesCosmetic.Active)
                        {
                            r += DistanceIf(wingScalesVars.scaleLength, wingScalesCosmetic.LengthInput);
                            r += DistanceIf(wingScalesVars.numScales, wingScalesCosmetic.NumScalesInput);
                        }
                        else if (wingScalesCosmetic.Enabled && !wingScalesCosmetic.Toggled)
                        {
                            r += MISSING_PENALTY;
                        }
                        break;
                    case SpineSpikesVars spineSpikesVars:
                        {
                            SpineSpikesCosmetic input;
                            if (finishedZoopSpecific)
                            {
                                body = true;
                                input = spineSpikesCosmetic;
                            }
                            else
                            {
                                input = mainSpineSpikesCosmetic;
                            }

                            if (input.Active)
                            {
                                r += DistanceIf(spineSpikesVars.spineLength, input.LengthInput);
                                r += DistanceIf(spineSpikesVars.numScales, input.NumScalesInput);
                                r += DistanceIf(spineSpikesVars.graphic, input.GraphicInput);
                            }
                            else if (input.Enabled && !input.Toggled)
                            {
                                r += MISSING_PENALTY;
                            }
                        }
                        break;

                    case TailTuftVars tailTuftVars:
                        {
                            TailTuftCosmetic input;
                            if (finishedZoopSpecific)
                            {
                                tail = true;
                                input = tailTuftCosmetic;
                            }
                            else
                            {
                                finishedZoopSpecific = true;
                                input = mainTailTuftCosmetic;
                            }

                            if (input.Active)
                            {
                                r += DistanceIf(tailTuftVars.numScales, input.NumScalesInput);
                                r += DistanceIf(tailTuftVars.scaleType, input.ScaleTypeInput);
                            }
                            else if (input.Enabled && !input.Toggled)
                            {
                                r += MISSING_PENALTY;
                            }
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

                    case LongHeadScalesVars longHeadScalesVars:
                        lhs = true;
                        if (longHeadScalesCosmetic.Active)
                        {
                            r += DistanceIf(longHeadScalesVars.length, longHeadScalesCosmetic.LengthInput);
                            r += DistanceIf(longHeadScalesVars.width, longHeadScalesCosmetic.WidthInput);
                            r += DistanceIf(longHeadScalesVars.rigor, longHeadScalesCosmetic.RigorInput);
                            r += DistanceIf(longHeadScalesVars.colored, longHeadScalesCosmetic.ColoredInput);
                        }
                        else if (longHeadScalesCosmetic.Enabled && !longHeadScalesCosmetic.Toggled)
                        {
                            r += MISSING_PENALTY;
                        }
                        break;

                    case SnowAccumulationVars: break;
                    default:
                        throw new InvalidOperationException("Unexpected result! " + result.GetType().Name);
                }
            }

            bool wantedBodyCosmetic = spineSpikesCosmetic.Enabled && spineSpikesCosmetic.Toggled;
            wantedBodyCosmetic |= longShoulderScalesCosmetic.Enabled && longShoulderScalesCosmetic.Toggled;
            wantedBodyCosmetic |= shortBodyScalesCosmetic.Enabled && shortBodyScalesCosmetic.Toggled;
            wantedBodyCosmetic |= bumpHawkCosmetic.Enabled && bumpHawkCosmetic.Toggled;
            if (!body && wantedBodyCosmetic) r += MISSING_PENALTY;

            if (!tail && tailTuftCosmetic.Enabled && tailTuftCosmetic.Toggled) r += MISSING_PENALTY;

            if (!lhs && longHeadScalesCosmetic.Enabled && longHeadScalesCosmetic.Toggled) r += MISSING_PENALTY;

            return r;
        }
    }
}
