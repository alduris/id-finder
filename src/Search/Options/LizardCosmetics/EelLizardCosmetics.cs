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
            bool wasLSSGroup = false;
            bool body = false;
            bool tail = false;
            bool lhs = false;

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
                            if (longShoulderScalesVars.id == 0)
                            {
                                r += mainLongShoulderScalesCosmetic.Distance(longShoulderScalesVars);
                                wasLSSGroup = true;
                            }
                            else
                            {
                                r += longShoulderScalesCosmetic.Distance(longShoulderScalesVars);
                                body = true;
                            }
                        }
                        break;
                    case ShortBodyScalesVars shortBodyScalesVars:
                        {
                            if (shortBodyScalesVars.id == 0)
                            {
                                r += mainShortBodyScalesCosmetic.Distance(shortBodyScalesVars);
                                wasLSSGroup = false;
                            }
                            else
                            {
                                r += shortBodyScalesCosmetic.Distance(shortBodyScalesVars);
                                body = true;
                            }
                        }
                        break;

                    case TailFinVars tailFinVars:
                        {
                            TailFinCosmetic input = wasLSSGroup ? lssTailFinCosmetic : sbsTailFinCosmetic;
                            r += input.Distance(tailFinVars);
                        }
                        break;
                    case TailTuftVars tailTuftVars:
                        {
                            if (tailTuftVars.id == 0)
                            {
                                r += mainTailTuftCosmetic.Distance(tailTuftVars);
                            }
                            else
                            {
                                r += tailTuftCosmetic.Distance(tailTuftVars);
                                tail = true;
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
