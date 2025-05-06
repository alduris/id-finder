using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Search.Util.LizardUtil;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;

namespace FinderMod.Search.Options.LizardCosmetics
{
    public class EelLizardCosmetics : BaseLizardCosmetics
    {
        private readonly AxolotlGillsCosmetic axolotlGillsCosmetic;
        private readonly TailGeckoScalesCosmetic tailGeckoScalesCosmetic;
        private readonly LongShoulderScalesCosmetic mainLongShoulderScalesCosmetic;
        private readonly ShortBodyScalesCosmetic mainShortBodyScalesCosmetic;
        private readonly TailFinCosmetic lssTailFinCosmetic;
        private readonly TailFinCosmetic sbsTailFinCosmetic;
        private readonly TailTuftCosmetic mainTailTuftCosmetic;

        private readonly SpineSpikesCosmetic spineSpikesCosmetic;
        private readonly BumpHawkCosmetic bumpHawkCosmetic;
        private readonly LongShoulderScalesCosmetic longShoulderScalesCosmetic;
        private readonly ShortBodyScalesCosmetic shortBodyScalesCosmetic;

        private readonly TailTuftCosmetic tailTuftCosmetic;

        private readonly LongHeadScalesCosmetic longHeadScalesCosmetic;

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
            cosmetics.Add(Label("Eel-specific cosmetics group"));
            cosmetics.Add(axolotlGillsCosmetic = new AxolotlGillsCosmetic());
            cosmetics.Add(tailGeckoScalesCosmetic = new TailGeckoScalesCosmetic());
            cosmetics.Add(
                OneOf(
                    "Back-tail cosmetics",
                    Group(
                        "LongShoulderScales group",
                        mainLongShoulderScalesCosmetic = new LongShoulderScalesCosmetic(type),
                        lssTailFinCosmetic = new TailFinCosmetic(type)),
                    Group(
                        "ShortBodyScales group",
                        mainShortBodyScalesCosmetic = new ShortBodyScalesCosmetic(type),
                        OneOf(
                            "Tail decoration",
                            sbsTailFinCosmetic = new TailFinCosmetic(type),
                            mainTailTuftCosmetic = new TailTuftCosmetic(type)
                            )
                        )
                    )
                );
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
            bool foundEelGroup = false;
            bool wasLSSGroup = false;
            bool finishedEelGroup = false;
            bool body = false;
            bool tail = false;
            bool lhs = false;

            foreach (var result in GetResults(Random))
            {
                switch (result)
                {
                    case AxolotlGillsVars axolotlGillsVars:
                        r += DistanceIf(axolotlGillsVars.rigor, axolotlGillsCosmetic.RigorInput);
                        r += DistanceIf(axolotlGillsVars.numGills, axolotlGillsCosmetic.NumGillsInput);
                        r += DistanceIf(axolotlGillsVars.graphic, axolotlGillsCosmetic.GraphicInput);
                        break;
                    case TailGeckoScalesVars tailGeckoScalesVars:
                        r += DistanceIf(tailGeckoScalesVars.rows, tailGeckoScalesCosmetic.RowsInput);
                        r += DistanceIf(tailGeckoScalesVars.lines, tailGeckoScalesCosmetic.LinesInput);
                        break;

                    case LongShoulderScalesVars longShoulderScalesVars:
                        {
                            LongShoulderScalesCosmetic input;
                            if (foundEelGroup)
                            {
                                body = true;
                                input = longShoulderScalesCosmetic;
                            }
                            else
                            {
                                foundEelGroup = true;
                                wasLSSGroup = true;
                                input = mainLongShoulderScalesCosmetic;
                            }

                            if (input.Active)
                            {
                                r += DistanceIf(longShoulderScalesVars.minSize, input.MinSizeInput);
                                r += DistanceIf(longShoulderScalesVars.maxSize, input.MaxSizeInput);
                                r += DistanceIf(longShoulderScalesVars.numScales, input.NumScalesInput);
                                r += DistanceIf(longShoulderScalesVars.graphic, input.GraphicInput);
                                if (input.ScaleTypeInput.enabled && input.ScaleTypeInput.value != longShoulderScalesVars.scaleType)
                                    r += input.ScaleTypeInput.bias;
                                r += DistanceIf(longShoulderScalesVars.colored, input.ColoredInput);
                            }
                            else if (input.Enabled && !input.Toggled)
                            {
                                r += MISSING_PENALTY;
                            }
                        }
                        break;
                    case ShortBodyScalesVars shortBodyScalesVars:
                        {
                            ShortBodyScalesCosmetic input;
                            if (foundEelGroup)
                            {
                                body = true;
                                input = shortBodyScalesCosmetic;
                            }
                            else
                            {
                                foundEelGroup = true;
                                wasLSSGroup = false;
                                input = mainShortBodyScalesCosmetic;
                            }

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
                        }
                        break;

                    case TailFinVars tailFinVars:
                        {
                            finishedEelGroup = true;
                            TailFinCosmetic input = wasLSSGroup ? lssTailFinCosmetic : sbsTailFinCosmetic;
                            if (wasLSSGroup || input.Active)
                            {
                                r += DistanceIf(tailFinVars.spineLength, input.LengthInput);
                                r += DistanceIf(tailFinVars.undersideSize, input.UndersideSizeInput);
                                r += DistanceIf(tailFinVars.spineScaleX, input.ScaleXInput);
                                r += DistanceIf(tailFinVars.numScales, input.NumScalesInput);
                                r += DistanceIf(tailFinVars.graphic, input.GraphicInput);
                                r += DistanceIf(tailFinVars.colored, input.ColoredInput);
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
                            if (finishedEelGroup)
                            {
                                tail = true;
                                input = tailTuftCosmetic;
                            }
                            else
                            {
                                finishedEelGroup = true;
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

                    case LizardRotVars lizardRotVars:
                        r += DistanceIf(lizardRotVars.numLegs, lizardRotCosmetic.NumTentaclesInput);
                        r += DistanceIf(lizardRotVars.numDeadLegs, lizardRotCosmetic.NumDeadTentaclesInput);
                        r += DistanceIf(lizardRotVars.numEyes, lizardRotCosmetic.NumEyesInput);
                        break;

                    default:
                        throw new InvalidOperationException("Unexpected result! " + result.GetType().Name);
                }
            }

            if (!wasLSSGroup && mainLongShoulderScalesCosmetic.Enabled && mainLongShoulderScalesCosmetic.Toggled)
            {
                r += MISSING_PENALTY;
            }
            if (wasLSSGroup && mainShortBodyScalesCosmetic.Enabled && mainShortBodyScalesCosmetic.Toggled)
            {
                r += MISSING_PENALTY;
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
