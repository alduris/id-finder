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
        // private readonly TailTuftCosmetic tailTuft;

        private readonly SpineSpikesCosmetic extraSpineSpikes;
        private readonly BumpHawkCosmetic bumpHawk;
        private readonly LongShoulderScalesCosmetic longSScales1;
        private readonly ShortBodyScalesCosmetic shortBScales;

        private readonly TailTuftCosmetic extraTailTuft;
        private readonly LongShoulderScalesCosmetic longSScales2;

        private readonly LongHeadScalesCosmetic longHeadScales;

        public ZoopLizardCosmetics() : base(LizardType.Zoop)
        {
            cosmetics.Add(
                OneOf(
                    "Major cosmetic group",
                    wingScales = new WingScalesCosmetic(),
                    spineSpikes = new SpineSpikesCosmetic()
                    )
                );
            // cosmetics.Add(tailTuft = new TailTuftCosmetic());

            // normal lizard cases
            cosmetics.Add(
                OneOf(
                    "Extra cosmetic 1",
                    extraSpineSpikes = new SpineSpikesCosmetic(),
                    bumpHawk = new BumpHawkCosmetic(),
                    longSScales1 = new LongShoulderScalesCosmetic(),
                    shortBScales = new ShortBodyScalesCosmetic()
                    )
                );
            cosmetics.Add(
                OneOf(
                    "Extra cosmetic 2",
                    extraTailTuft = new TailTuftCosmetic(),
                    longSScales2 = new LongShoulderScalesCosmetic()
                    )
                );
            cosmetics.Add(Toggleable("Has LongHeadScales", longHeadScales = new LongHeadScalesCosmetic()));
        }

        public override float Execute(XORShift128 Random)
        {
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
                        r += 100f;
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
                        r += 100f;
                    }
                    break;
                default: throw new InvalidOperationException("Result was not WingScales or SpineSpikes");
            }

            results.MoveNext();
            if (results.Current is not TailTuftVars) new InvalidOperationException("Result was not TailTuft");

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
                            r += 100f;
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
                            r += 100f;
                        }
                        break;
                    case LongShoulderScalesVars longShoulderScalesVars:
                        if (longSScales1.Active)
                        {
                            r += DistanceIf(longShoulderScalesVars.minSize, longSScales1.MinSizeInput);
                            r += DistanceIf(longShoulderScalesVars.maxSize, longSScales1.MaxSizeInput);
                            r += DistanceIf(longShoulderScalesVars.numScales, longSScales1.NumScalesInput);
                            r += DistanceIf(longShoulderScalesVars.graphic, longSScales1.GraphicInput);
                            if (longSScales1.ScaleTypeInput.enabled && longSScales1.ScaleTypeInput.value != longShoulderScalesVars.scaleType) r += 1f;
                            r += DistanceIf(longShoulderScalesVars.colored, longSScales1.ColoredInput);
                        }
                        else if (longSScales1.Enabled && !longSScales1.Toggled)
                        {
                            r += 100f;
                        }
                        break;
                    case ShortBodyScalesVars shortBodyScalesVars:
                        if (shortBScales.Active)
                        {
                            r += DistanceIf(shortBodyScalesVars.numScales, shortBScales.NumScalesInput);
                            if (shortBScales.ScaleTypeInput.enabled && shortBScales.ScaleTypeInput.value != shortBodyScalesVars.scaleType) r += 1f;
                        }
                        else if (shortBScales.Enabled && !shortBScales.Toggled)
                        {
                            r += 100f;
                        }
                        break;
                    case TailTuftVars:
                        cont = false;
                        if (extraTailTuft.Enabled && !extraTailTuft.Toggled) r += 100f;
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
                            r += 100f;
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
                            if (extraTailTuft.Enabled && !extraTailTuft.Toggled) r += 100f;
                            break;
                        case LongShoulderScalesVars longShoulderScalesVars:
                            if (longSScales2.Active)
                            {
                                r += DistanceIf(longShoulderScalesVars.minSize, longSScales2.MinSizeInput);
                                r += DistanceIf(longShoulderScalesVars.maxSize, longSScales2.MaxSizeInput);
                                r += DistanceIf(longShoulderScalesVars.numScales, longSScales2.NumScalesInput);
                                r += DistanceIf(longShoulderScalesVars.graphic, longSScales2.GraphicInput);
                                if (longSScales2.ScaleTypeInput.enabled && longSScales2.ScaleTypeInput.value != longShoulderScalesVars.scaleType) r += 1f;
                                r += DistanceIf(longShoulderScalesVars.colored, longSScales2.ColoredInput);
                            }
                            else if (longSScales2.Enabled && !longSScales2.Toggled)
                            {
                                r += 100f;
                            }
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
                                r += 100f;
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
                            r += 100f;
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
