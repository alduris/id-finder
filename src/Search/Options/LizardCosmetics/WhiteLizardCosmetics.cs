using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Search.Options.LizardCosmetics
{
    public class WhiteLizardCosmetics : BaseLizardCosmetics
    {
        private readonly BumpHawkCosmetic bumpHawkCosmetic;
        private readonly ShortBodyScalesCosmetic shortBodyScalesCosmetic;
        private readonly LongShoulderScalesCosmetic longShoulderScalesCosmetic;
        private readonly LongHeadScalesCosmetic longHeadScalesCosmetic;
        private readonly TailTuftCosmetic tailTuftCosmetic;

        public WhiteLizardCosmetics() : base(LizardType.White)
        {
            cosmetics.Add(
                OneOf(
                    "Body cosmetic",
                    bumpHawkCosmetic = new BumpHawkCosmetic(type),
                    shortBodyScalesCosmetic = new ShortBodyScalesCosmetic(type),
                    longShoulderScalesCosmetic = new LongShoulderScalesCosmetic(type),
                    longHeadScalesCosmetic = new LongHeadScalesCosmetic(),
                    None()
                    )
                );
            cosmetics.Add(Toggleable("Has tail tuft", tailTuftCosmetic = new TailTuftCosmetic(type)));
        }

        public override float Execute(XORShift128 Random)
        {
            float r = 0f;
            bool body = false;
            bool tail = false;
            var results = GetResults(Random).GetEnumerator();

            if (results.MoveNext())
            {
                body = true;
                switch (results.Current)
                {
                    case BumpHawkVars bumpHawkVars:
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
                    case ShortBodyScalesVars shortBodyScalesVars:
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
                    case LongShoulderScalesVars longShoulderScalesVars:
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
                    case LongHeadScalesVars longHeadScalesVars:
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
                    case TailTuftVars tailTuftVars:
                        tail = true;
                        body = false;
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
                    default:
                        throw new InvalidOperationException("Unexpected result! " + results.Current.GetType().Name);
                }

                if (!tail && results.MoveNext())
                {
                    if (results.Current is TailTuftVars tailTuftVars)
                    {
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
                    }
                    else
                    {
                        throw new InvalidOperationException("Result was not TailTuftVars!");
                    }
                }
            }

            bool wantedBodyCosmetic = bumpHawkCosmetic.Enabled && bumpHawkCosmetic.Toggled;
            wantedBodyCosmetic |= shortBodyScalesCosmetic.Enabled && shortBodyScalesCosmetic.Toggled;
            wantedBodyCosmetic |= longShoulderScalesCosmetic.Enabled && longShoulderScalesCosmetic.Toggled;
            wantedBodyCosmetic |= longHeadScalesCosmetic.Enabled && longHeadScalesCosmetic.Toggled;
            if (!body && wantedBodyCosmetic) r += MISSING_PENALTY;
            if (!tail && tailTuftCosmetic.Enabled && tailTuftCosmetic.Toggled) r += MISSING_PENALTY;

            return r;
        }
    }
}
