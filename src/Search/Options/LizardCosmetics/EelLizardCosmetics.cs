using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Search.Util.LizardUtil;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;

namespace FinderMod.Search.Options.LizardCosmetics
{
    public class EelLizardCosmetics : BaseLizardCosmetics
    {
        private readonly AxolotlGillsCosmetic axoGills;
        private readonly TailGeckoScalesCosmetic geckoScales;
        private readonly LongShoulderScalesCosmetic longSScales;
        private readonly ShortBodyScalesCosmetic shortBScales;
        private readonly TailFinCosmetic tailFin1;
        private readonly TailFinCosmetic tailFin2;
        private readonly TailTuftCosmetic tailTuft;

        private readonly SpineSpikesCosmetic spineSpikes;
        private readonly BumpHawkCosmetic bumpHawk;
        private readonly LongShoulderScalesCosmetic extraLongSScales1;
        private readonly ShortBodyScalesCosmetic extraShortBScales;

        private readonly TailTuftCosmetic extraTailTuft;
        private readonly LongShoulderScalesCosmetic extraLongSScales2;

        private readonly LongHeadScalesCosmetic longHeadScales;

        public EelLizardCosmetics() : base(LizardType.Eel)
        {
            // specific to eel lizard
            // axolotl gills
            // tail gecko scales
            // one of:
            // 1. LSS and tail fin
            // 2. SBS and one of:
            //    a. tail fin
            //    b. tail tuft
            cosmetics.Add(axoGills = new AxolotlGillsCosmetic());
            cosmetics.Add(geckoScales = new TailGeckoScalesCosmetic());
            cosmetics.Add(
                OneOf(
                    "Major cosmetic group",
                    Group(
                        "LongShoulderScales group",
                        longSScales = new LongShoulderScalesCosmetic(),
                        tailFin1 = new TailFinCosmetic()),
                    Group(
                        "ShortBodyScales group",
                        shortBScales = new ShortBodyScalesCosmetic(),
                        OneOf(
                            "Tail decoration",
                            tailFin2 = new TailFinCosmetic(),
                            tailTuft = new TailTuftCosmetic()
                            )
                        )
                    )
                );
            // normal lizard cases
            cosmetics.Add(
                OneOf(
                    "Extra cosmetic 1",
                    spineSpikes = new SpineSpikesCosmetic(),
                    bumpHawk = new BumpHawkCosmetic(),
                    extraLongSScales1 = new LongShoulderScalesCosmetic(),
                    extraShortBScales = new ShortBodyScalesCosmetic()
                    )
                );
            cosmetics.Add(
                OneOf(
                    "Extra cosmetic 2",
                    extraTailTuft = new TailTuftCosmetic(),
                    extraLongSScales2 = new LongShoulderScalesCosmetic()
                    )
                );
            cosmetics.Add(Toggleable("Has LongHeadScales", longHeadScales = new LongHeadScalesCosmetic()));
        }

        public override float Execute(XORShift128 Random)
        {
            float r = 0f;
            var results = GetResults(Random).GetEnumerator();

            // Eel-exclusive
            if (results.MoveNext() && results.Current is AxolotlGillsVars axolotlGillsVars)
            {
                if (axoGills.Active)
                {
                    r += DistanceIf(axolotlGillsVars.rigor, axoGills.RigorInput);
                    r += DistanceIf(axolotlGillsVars.numGills, axoGills.NumGillsInput);
                    r += DistanceIf(axolotlGillsVars.graphic, axoGills.GraphicInput);
                }
            }
            else throw new InvalidOperationException("Result was not AxolotlGills");
            if (results.MoveNext() && results.Current is TailGeckoScalesVars tailGeckoScalesVars)
            {
                if (geckoScales.Active)
                {
                    r += DistanceIf(tailGeckoScalesVars.rows, geckoScales.RowsInput);
                    r += DistanceIf(tailGeckoScalesVars.lines, geckoScales.LinesInput);
                }
            }
            else throw new InvalidOperationException("Result was not TailGeckoScales");

            results.MoveNext();
            switch (results.Current)
            {
                case LongShoulderScalesVars longShoulderScalesVars:
                    {
                        if (longSScales.Enabled && !longSScales.Toggled)
                        {
                            r += 100f;
                        }
                        else
                        {
                            if (longSScales.Active)
                            {
                                r += DistanceIf(longShoulderScalesVars.minSize, longSScales.MinSizeInput);
                                r += DistanceIf(longShoulderScalesVars.maxSize, longSScales.MaxSizeInput);
                                r += DistanceIf(longShoulderScalesVars.numScales, longSScales.NumScalesInput);
                                r += DistanceIf(longShoulderScalesVars.graphic, longSScales.GraphicInput);
                                if (longSScales.ScaleTypeInput.enabled && longSScales.ScaleTypeInput.value != longShoulderScalesVars.scaleType) r += 1f;
                                r += DistanceIf(longShoulderScalesVars.colored, longSScales.ColoredInput);
                            }
                        }

                        results.MoveNext();
                        TailFinCosmetic tailFin = tailFin1;
                        if (results.Current is TailFinVars tailFinVars)
                        {
                            if (tailFin.Active)
                            {
                                r += DistanceIf(tailFinVars.spineLength, tailFin.LengthInput);
                                r += DistanceIf(tailFinVars.undersideSize, tailFin.UndersideSizeInput);
                                r += DistanceIf(tailFinVars.spineScaleX, tailFin.ScaleXInput);
                                r += DistanceIf(tailFinVars.numScales, tailFin.NumScalesInput);
                                r += DistanceIf(tailFinVars.graphic, tailFin.GraphicInput);
                                r += DistanceIf(tailFinVars.colored, tailFin.ColoredInput);
                            }
                        }
                        else throw new InvalidOperationException("Result was not TailFin");

                        break;
                    }
                case ShortBodyScalesVars shortBodyScalesVars:
                    {
                        if (shortBScales.Active)
                        {
                            r += DistanceIf(shortBodyScalesVars.numScales, shortBScales.NumScalesInput);
                            if (shortBScales.ScaleTypeInput.enabled && shortBScales.ScaleTypeInput.value != shortBodyScalesVars.scaleType) r += 1f;
                        }
                        else if (shortBScales.Enabled && !shortBScales.Toggled)
                        {
                            r += 100f;
                        }

                        results.MoveNext();
                        switch (results.Current)
                        {
                            case TailFinVars tailFinVars:
                                TailFinCosmetic tailFin = tailFin2;
                                if (tailFin.Active)
                                {
                                    r += DistanceIf(tailFinVars.spineLength, tailFin.LengthInput);
                                    r += DistanceIf(tailFinVars.undersideSize, tailFin.UndersideSizeInput);
                                    r += DistanceIf(tailFinVars.spineScaleX, tailFin.ScaleXInput);
                                    r += DistanceIf(tailFinVars.numScales, tailFin.NumScalesInput);
                                    r += DistanceIf(tailFinVars.graphic, tailFin.GraphicInput);
                                    r += DistanceIf(tailFinVars.colored, tailFin.ColoredInput);
                                }
                                else if (tailFin.Enabled && !tailFin.Toggled)
                                {
                                    r += 100f;
                                }
                                break;
                            case TailTuftVars: // cool
                                if (tailTuft.Enabled && !tailTuft.Toggled) r += 100f;
                                break;
                            default: throw new NotImplementedException("Result was not TailFin or TailTuft!");
                        }
                        break;
                    }
                default: throw new InvalidOperationException("Result was not LongShoulderScales or ShortBodyScales");
            }

            // Generic lizard shenanigans
            if (results.MoveNext())
            {
                bool cont = true;
                bool lhs = true;
                switch (results.Current)
                {
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
                    case BumpHawkVars bumpHawkVars:
                        if (bumpHawk.Active)
                        {
                            r += DistanceIf(bumpHawkVars.spineLength, bumpHawk.SpineLenInput);
                            r += DistanceIf(bumpHawkVars.numBumps, bumpHawk.NumBumpsInput);
                            r += DistanceIf(bumpHawkVars.colored, bumpHawk.ColoredInput);
                        }
                        else if (bumpHawk.Enabled &&  !bumpHawk.Toggled)
                        {
                            r += 100f;
                        }
                        break;
                    case LongShoulderScalesVars longShoulderScalesVars:
                        if (extraLongSScales1.Active)
                        {
                            r += DistanceIf(longShoulderScalesVars.minSize, extraLongSScales1.MinSizeInput);
                            r += DistanceIf(longShoulderScalesVars.maxSize, extraLongSScales1.MaxSizeInput);
                            r += DistanceIf(longShoulderScalesVars.numScales, extraLongSScales1.NumScalesInput);
                            r += DistanceIf(longShoulderScalesVars.graphic, extraLongSScales1.GraphicInput);
                            if (extraLongSScales1.ScaleTypeInput.enabled && extraLongSScales1.ScaleTypeInput.value != longShoulderScalesVars.scaleType) r += 1f;
                            r += DistanceIf(longShoulderScalesVars.colored, extraLongSScales1.ColoredInput);
                        }
                        else if (extraLongSScales1.Enabled && !extraLongSScales1.Toggled)
                        {
                            r += 100f;
                        }
                        break;
                    case ShortBodyScalesVars shortBodyScalesVars:
                        if (extraShortBScales.Active)
                        {
                            r += DistanceIf(shortBodyScalesVars.numScales, extraShortBScales.NumScalesInput);
                            if (extraShortBScales.ScaleTypeInput.enabled && extraShortBScales.ScaleTypeInput.value != shortBodyScalesVars.scaleType) r += 1f;
                        }
                        else if (extraShortBScales.Enabled && !extraShortBScales.Toggled)
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
                            if (extraLongSScales2.Active)
                            {
                                r += DistanceIf(longShoulderScalesVars.minSize, extraLongSScales2.MinSizeInput);
                                r += DistanceIf(longShoulderScalesVars.maxSize, extraLongSScales2.MaxSizeInput);
                                r += DistanceIf(longShoulderScalesVars.numScales, extraLongSScales2.NumScalesInput);
                                r += DistanceIf(longShoulderScalesVars.graphic, extraLongSScales2.GraphicInput);
                                if (extraLongSScales2.ScaleTypeInput.enabled && extraLongSScales2.ScaleTypeInput.value != longShoulderScalesVars.scaleType) r += 1f;
                                r += DistanceIf(longShoulderScalesVars.colored, extraLongSScales1.ColoredInput);
                            }
                            else if (extraLongSScales2.Enabled && !extraLongSScales2.Toggled)
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
                    else
                    {
                        throw new InvalidOperationException("Result was not LongHeadScales");
                    }
                }
            }

            return r;
        }
    }
}
