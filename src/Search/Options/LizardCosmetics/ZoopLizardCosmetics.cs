using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Search.Util.LizardUtil;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;

namespace FinderMod.Search.Options.LizardCosmetics
{
    public class ZoopLizardCosmetics : BaseLizardCosmetics
    {
        private readonly WingScalesCosmetic wingScales;
        private readonly SpineSpikesCosmetic spineSpikes;
        private readonly TailTuftCosmetic tailTuft;

        private readonly SpineSpikesCosmetic extraSpineSpikes;
        private readonly BumpHawkCosmetic bumpHawk;
        private readonly LongShoulderScalesCosmetic longSScales;
        private readonly ShortBodyScalesCosmetic shortBScales;

        private readonly TailTuftCosmetic extraTailTuft;

        private readonly LongHeadScalesCosmetic longHeadScales;

        public ZoopLizardCosmetics() : base(LizardType.Zoop)
        {
            cosmetics.Add(
                OneOf(
                    "Zoop-specific cosmetic",
                    wingScales = new WingScalesCosmetic(),
                    spineSpikes = new SpineSpikesCosmetic(type)
                    )
                );
            cosmetics.Add(tailTuft = new TailTuftCosmetic(type));

            // normal lizard cases
            cosmetics.Add(
                OneOf(
                    "Body cosmetic",
                    extraSpineSpikes = new SpineSpikesCosmetic(type),
                    bumpHawk = new BumpHawkCosmetic(type),
                    longSScales = new LongShoulderScalesCosmetic(type),
                    shortBScales = new ShortBodyScalesCosmetic(type),
                    None()
                    )
                );
            cosmetics.Add(Toggleable("Has TailTuft", extraTailTuft = new TailTuftCosmetic(type)));
            cosmetics.Add(Toggleable("Has LongHeadScales", longHeadScales = new LongHeadScalesCosmetic()));
        }

        public override float Execute(XORShift128 Random)
        {
            // TODO: need to redo this probably
            float r = 0f;
            var results = GetResults(Random).GetEnumerator();

            // Zoop lizard stuff
            results.MoveNext();
            switch (results.Current)
            {
                case WingScalesVars wingScalesVars:
                    if (wingScales.Active)
                    {
                        r += DistanceIf(wingScalesVars.scaleLength, wingScales.LengthInput);
                        r += DistanceIf(wingScalesVars.numScales, wingScales.NumScalesInput);
                    }
                    else if (wingScales.Enabled && !wingScales.Toggled)
                    {
                        r += MISSING_PENALTY;
                    }
                    break;
                case SpineSpikesVars spineSpikesVars:
                    if (spineSpikes.Active)
                    {
                        r += DistanceIf(spineSpikesVars.spineLength, spineSpikes.LengthInput);
                        r += DistanceIf(spineSpikesVars.numScales, spineSpikes.NumScalesInput);
                        r += DistanceIf(spineSpikesVars.graphic, spineSpikes.GraphicInput);
                    }
                    else if (spineSpikes.Enabled && !spineSpikes.Toggled)
                    {
                        r += MISSING_PENALTY;
                    }
                    break;
                default: throw new InvalidOperationException("Result was not WingScales or SpineSpikes");
            }

            results.MoveNext();
            if (results.Current is TailTuftVars tailTuftVars)
            {

                if (tailTuft.Active)
                {
                    r += DistanceIf(tailTuftVars.numScales, tailTuft.NumScalesInput);
                    r += DistanceIf(tailTuftVars.scaleType, tailTuft.ScaleTypeInput);
                }
                else if (tailTuft.Enabled && !tailTuft.Toggled)
                {
                    r += MISSING_PENALTY;
                }
            }
            else new InvalidOperationException("Result was not TailTuft");

            // Generic lizard shenanigans
            if (results.MoveNext())
            {
                bool cont = true;
                bool lhs = true;
                switch (results.Current)
                {
                    case SpineSpikesVars spineSpikesVars:
                        if (extraSpineSpikes.Active)
                        {
                            r += DistanceIf(spineSpikesVars.spineLength, extraSpineSpikes.LengthInput);
                            r += DistanceIf(spineSpikesVars.numScales, extraSpineSpikes.NumScalesInput);
                            r += DistanceIf(spineSpikesVars.graphic, extraSpineSpikes.GraphicInput);
                        }
                        else if (extraSpineSpikes.Enabled && !extraSpineSpikes.Toggled)
                        {
                            r += MISSING_PENALTY;
                        }
                        break;
                    case BumpHawkVars bumpHawkVars:
                        if (bumpHawk.Active)
                        {
                            r += DistanceIf(bumpHawkVars.spineLength, bumpHawk.SpineLenInput);
                            r += DistanceIf(bumpHawkVars.numBumps, bumpHawk.NumBumpsInput);
                            r += DistanceIf(bumpHawkVars.colored, bumpHawk.ColoredInput);
                        }
                        else if (bumpHawk.Enabled && !bumpHawk.Toggled)
                        {
                            r += MISSING_PENALTY;
                        }
                        break;
                    case LongShoulderScalesVars longShoulderScalesVars:
                        if (longSScales.Active)
                        {
                            r += DistanceIf(longShoulderScalesVars.minSize, longSScales.MinSizeInput);
                            r += DistanceIf(longShoulderScalesVars.maxSize, longSScales.MaxSizeInput);
                            r += DistanceIf(longShoulderScalesVars.numScales, longSScales.NumScalesInput);
                            r += DistanceIf(longShoulderScalesVars.graphic, longSScales.GraphicInput);
                            if (longSScales.ScaleTypeInput.enabled && longSScales.ScaleTypeInput.value != longShoulderScalesVars.scaleType) r += longSScales.ScaleTypeInput.bias;
                            r += DistanceIf(longShoulderScalesVars.colored, longSScales.ColoredInput);
                        }
                        else if (longSScales.Enabled && !longSScales.Toggled)
                        {
                            r += MISSING_PENALTY;
                        }
                        break;
                    case ShortBodyScalesVars shortBodyScalesVars:
                        if (shortBScales.Active)
                        {
                            r += DistanceIf(shortBodyScalesVars.numScales, shortBScales.NumScalesInput);
                            if (shortBScales.ScaleTypeInput.enabled && shortBScales.ScaleTypeInput.value != shortBodyScalesVars.scaleType) r += shortBScales.ScaleTypeInput.bias;
                        }
                        else if (shortBScales.Enabled && !shortBScales.Toggled)
                        {
                            r += MISSING_PENALTY;
                        }
                        break;
                    case TailTuftVars:
                        cont = false;
                        if (extraTailTuft.Enabled && !extraTailTuft.Toggled) r += MISSING_PENALTY;
                        break;
                    case LongHeadScalesVars longHeadScalesVars:
                        cont = false;
                        lhs = false;
                        if (longHeadScales.Active)
                        {
                            r += DistanceIf(longHeadScalesVars.length, longHeadScales.LengthInput);
                            r += DistanceIf(longHeadScalesVars.width, longHeadScales.WidthInput);
                            r += DistanceIf(longHeadScalesVars.rigor, longHeadScales.RigorInput);
                            r += DistanceIf(longHeadScalesVars.colored, longHeadScales.ColoredInput);
                        }
                        else if (longHeadScales.Enabled && !longHeadScales.Toggled)
                        {
                            r += MISSING_PENALTY;
                        }
                        break;
                    case SnowAccumulationVars:
                        cont = false;
                        lhs = false;
                        break;
                    default: throw new InvalidOperationException("Extra cosmetic 1 was not a valid type");
                }

                if (cont && results.MoveNext())
                {
                    switch (results.Current)
                    {
                        case TailTuftVars:
                            if (extraTailTuft.Enabled && !extraTailTuft.Toggled) r += MISSING_PENALTY;
                            break;
                        case LongHeadScalesVars longHeadScalesVars:
                            lhs = false;
                            if (longHeadScales.Active)
                            {
                                r += DistanceIf(longHeadScalesVars.length, longHeadScales.LengthInput);
                                r += DistanceIf(longHeadScalesVars.width, longHeadScales.WidthInput);
                                r += DistanceIf(longHeadScalesVars.rigor, longHeadScales.RigorInput);
                                r += DistanceIf(longHeadScalesVars.colored, longHeadScales.ColoredInput);
                            }
                            else if (longHeadScales.Enabled && !longHeadScales.Toggled)
                            {
                                r += MISSING_PENALTY;
                            }
                            break;
                        case SnowAccumulationVars: break;
                        default: throw new InvalidOperationException("Extra cosmetic 2 was not a valid type");
                    }
                }

                if (lhs && results.MoveNext())
                {
                    if (results.Current is LongHeadScalesVars longHeadScalesVars)
                    {
                        if (longHeadScales.Active)
                        {
                            r += DistanceIf(longHeadScalesVars.length, longHeadScales.LengthInput);
                            r += DistanceIf(longHeadScalesVars.width, longHeadScales.WidthInput);
                            r += DistanceIf(longHeadScalesVars.rigor, longHeadScales.RigorInput);
                            r += DistanceIf(longHeadScalesVars.colored, longHeadScales.ColoredInput);
                        }
                        else if (longHeadScales.Enabled && !longHeadScales.Toggled)
                        {
                            r += MISSING_PENALTY;
                        }
                    }
                    else if (results.Current is not SnowAccumulationVars)
                    {
                        throw new InvalidOperationException("Result was not LongHeadScales");
                    }
                }
            }

            return r;
        }
    }
}
