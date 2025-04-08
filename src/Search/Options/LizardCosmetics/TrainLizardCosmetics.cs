using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Search.Util.LizardUtil;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;

namespace FinderMod.Search.Options.LizardCosmetics
{
    public class TrainLizardCosmetics : BaseLizardCosmetics
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
            // This is basically just the same thing as RedLizardCosmetics.Execute but it's train lizard this time.
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
                            r += DistanceIf(spineSpikesVars.spineLength, mainSpineSpikesCosmetic.LengthInput);
                            r += DistanceIf(spineSpikesVars.numScales, mainSpineSpikesCosmetic.NumScalesInput);
                            r += DistanceIf(spineSpikesVars.graphic, mainSpineSpikesCosmetic.GraphicInput);
                        }
                        else
                        {
                            // We know it's body here because the type-specific LSS comes before the type-specific spinespikes
                            // which means that either body or foundLSS must be true prior
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
                        }
                        break;
                    case LongShoulderScalesVars longShoulderScalesVars:
                        if (foundLSS)
                        {
                            // We found an LSS prior so we know this one is the type-specific one.
                            // Since we deal with the type-specific one afterwards, we deal with the previous one now
                            body = true;
                            if (longShoulderScalesCosmetic.Active)
                            {
                                r += DistanceIf(specificLSS.minSize, longShoulderScalesCosmetic.MinSizeInput);
                                r += DistanceIf(specificLSS.maxSize, longShoulderScalesCosmetic.MaxSizeInput);
                                r += DistanceIf(specificLSS.numScales, longShoulderScalesCosmetic.NumScalesInput);
                                r += DistanceIf(specificLSS.graphic, longShoulderScalesCosmetic.GraphicInput);
                                if (longShoulderScalesCosmetic.ScaleTypeInput.enabled && longShoulderScalesCosmetic.ScaleTypeInput.value != specificLSS.scaleType)
                                    r += longShoulderScalesCosmetic.ScaleTypeInput.bias;
                                r += DistanceIf(specificLSS.colored, longShoulderScalesCosmetic.ColoredInput);
                            }
                            else if (longShoulderScalesCosmetic.Enabled && !longShoulderScalesCosmetic.Toggled)
                            {
                                r += MISSING_PENALTY;
                            }
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

                    case TailFinVars tailFinVars:
                        if (mainTailFinCosmetic.Active)
                        {
                            r += DistanceIf(tailFinVars.spineLength, mainTailFinCosmetic.LengthInput);
                            r += DistanceIf(tailFinVars.undersideSize, mainTailFinCosmetic.UndersideSizeInput);
                            r += DistanceIf(tailFinVars.spineScaleX, mainTailFinCosmetic.ScaleXInput);
                            r += DistanceIf(tailFinVars.numScales, mainTailFinCosmetic.NumScalesInput);
                            r += DistanceIf(tailFinVars.graphic, mainTailFinCosmetic.GraphicInput);
                            r += DistanceIf(tailFinVars.colored, mainTailFinCosmetic.ColoredInput);
                        }
                        else if (mainTailFinCosmetic.Enabled && !mainTailFinCosmetic.Toggled)
                        {
                            r += MISSING_PENALTY;
                        }
                        break;
                    case TailTuftVars tailTuftVars:
                        {
                            var cosmetic = foundLSS ? mainTailTuftCosmetic : tailTuftCosmetic;
                            if (!foundLSS) tail = true;
                            if (cosmetic.Active)
                            {
                                r += DistanceIf(tailTuftVars.numScales, cosmetic.NumScalesInput);
                                r += DistanceIf(tailTuftVars.scaleType, cosmetic.ScaleTypeInput);
                            }
                            else if (cosmetic.Enabled && !cosmetic.Toggled)
                            {
                                r += MISSING_PENALTY;
                            }
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

            // Deal with type-specific LSS. We know that this is not default because the type-specific LSS is guaranteed.
            r += DistanceIf(specificLSS.minSize, mainLongShoulderScalesCosmetic.MinSizeInput);
            r += DistanceIf(specificLSS.maxSize, mainLongShoulderScalesCosmetic.MaxSizeInput);
            r += DistanceIf(specificLSS.numScales, mainLongShoulderScalesCosmetic.NumScalesInput);
            r += DistanceIf(specificLSS.graphic, mainLongShoulderScalesCosmetic.GraphicInput);
            if (mainLongShoulderScalesCosmetic.ScaleTypeInput.enabled && mainLongShoulderScalesCosmetic.ScaleTypeInput.value != specificLSS.scaleType)
                r += mainLongShoulderScalesCosmetic.ScaleTypeInput.bias;
            r += DistanceIf(specificLSS.colored, mainLongShoulderScalesCosmetic.ColoredInput);

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
