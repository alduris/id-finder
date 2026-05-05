using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Search.Util.LizardUtil;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;

namespace FinderMod.Search.Options.LizardCosmetics
{
    internal class EelLizardCosmetics : BaseLizardCosmetics
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

            // Tracking variables so we know which inputs to poll for distance if we already encountered them once.
            // This is necessary because the user does not necessarily know which order they may come in.
            LongShoulderScalesCosmetic otherLSS = null!;
            ShortBodyScalesCosmetic otherSBS = null!;
            TailTuftCosmetic otherTT = null!;

            foreach (var result in GetResults(Random))
            {
                switch (result)
                {
                    case AxolotlGillsVars axolotlGillsVars:
                        r += axolotlGillsCosmetic.Distance(axolotlGillsVars);
                        break;
                    case TailGeckoScalesVars tailGeckoScalesVars:
                        r += tailGeckoScalesCosmetic.Distance(tailGeckoScalesVars);
                        break;

                    case LongShoulderScalesVars longShoulderScalesVars:
                        {
                            bool tryBoth = true;
                            if (!foundEelGroup)
                            {
                                foundEelGroup = true;
                                wasLSSGroup = true;
                            }
                            else
                            {
                                body = true;
                                if (otherLSS != null)
                                {
                                    r += otherLSS.Distance(longShoulderScalesVars);
                                    tryBoth = false;
                                }
                            }

                            if (tryBoth)
                            {
                                float a = longShoulderScalesCosmetic.Distance(longShoulderScalesVars);
                                float b = mainLongShoulderScalesCosmetic.Distance(longShoulderScalesVars);

                                if ((a <= b && longShoulderScalesCosmetic.Enabled) || !mainLongShoulderScalesCosmetic.Enabled)
                                {
                                    otherLSS = mainLongShoulderScalesCosmetic;
                                    r += a;
                                }
                                else
                                {
                                    otherLSS = longShoulderScalesCosmetic;
                                    r += b;
                                }
                            }
                        }
                        break;
                    case ShortBodyScalesVars shortBodyScalesVars:
                        {
                            bool tryBoth = true;
                            if (!foundEelGroup)
                            {
                                foundEelGroup = true;
                                wasLSSGroup = false;
                            }
                            else
                            {
                                body = true;
                                if (otherSBS != null)
                                {
                                    r += otherSBS.Distance(shortBodyScalesVars);
                                    tryBoth = false;
                                }
                            }

                            if (tryBoth)
                            {
                                float a = shortBodyScalesCosmetic.Distance(shortBodyScalesVars);
                                float b = mainShortBodyScalesCosmetic.Distance(shortBodyScalesVars);
                                
                                if ((a <= b && shortBodyScalesCosmetic.Enabled) || !mainShortBodyScalesCosmetic.Enabled)
                                {
                                    otherSBS = mainShortBodyScalesCosmetic;
                                    r += a;
                                }
                                else
                                {
                                    otherSBS = shortBodyScalesCosmetic;
                                    r += b;
                                }
                            }
                        }
                        break;

                    case TailFinVars tailFinVars:
                        {
                            finishedEelGroup = true;
                            TailFinCosmetic input = wasLSSGroup ? lssTailFinCosmetic : sbsTailFinCosmetic;
                            r += input.Distance(tailFinVars);
                        }
                        break;
                    case TailTuftVars tailTuftVars:
                        {
                            bool tryBoth = true;
                            if (!finishedEelGroup)
                            {
                                finishedEelGroup = true;
                            }
                            else
                            {
                                tail = true;
                                
                                if (otherTT != null)
                                {
                                    r += otherTT.Distance(tailTuftVars);
                                    tryBoth = false;
                                }
                            }

                            if (tryBoth)
                            {
                                float a = tailTuftCosmetic.Distance(tailTuftVars);
                                float b = mainTailTuftCosmetic.Distance(tailTuftVars);

                                if ((a <= b && tailTuftCosmetic.Enabled) || !mainTailTuftCosmetic.Enabled)
                                {
                                    otherTT = mainTailTuftCosmetic;
                                    r += a;
                                }
                                else
                                {
                                    otherTT = tailTuftCosmetic;
                                    r += b;
                                }
                            }
                        }
                        break;

                    case SpineSpikesVars spineSpikesVars:
                        body = true;
                        r += spineSpikesCosmetic.Distance(spineSpikesVars);
                        break;
                    case BumpHawkVars bumpHawkVars:
                        body = true;
                        r += bumpHawkCosmetic.Distance(bumpHawkVars);
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

            if (!wasLSSGroup && mainLongShoulderScalesCosmetic.Enabled && mainLongShoulderScalesCosmetic.Toggled)
            {
                r += MISSING_PENALTY;
            }
            else if (wasLSSGroup && mainShortBodyScalesCosmetic.Enabled && mainShortBodyScalesCosmetic.Toggled)
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
