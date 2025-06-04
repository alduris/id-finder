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

            // Tracking variables so we know which inputs to poll for distance if we already encountered them once.
            // This is necessary because the user does not necessarily know which order they may come in.
            SpineSpikesCosmetic otherSS = null!;
            TailTuftCosmetic otherTT = null!;

            foreach (var result in GetResults(Random))
            {
                switch (result)
                {
                    case WingScalesVars wingScalesVars:
                        r += wingScalesCosmetic.Distance(wingScalesVars);
                        break;
                    case SpineSpikesVars spineSpikesVars:
                        {
                            bool tryBoth = true;
                            if (finishedZoopSpecific)
                            {
                                body = true;
                                if (otherSS != null)
                                {
                                    r += otherSS.Distance(spineSpikesVars);
                                    tryBoth = false;
                                }
                            }

                            if (tryBoth)
                            {
                                float a = spineSpikesCosmetic.Distance(spineSpikesVars);
                                float b = mainSpineSpikesCosmetic.Distance(spineSpikesVars);

                                if (a <= b && (spineSpikesCosmetic.Enabled))
                                {
                                    otherSS = mainSpineSpikesCosmetic;
                                    r += a;
                                }
                                else
                                {
                                    otherSS = spineSpikesCosmetic;
                                    r += b;
                                }
                            }
                        }
                        break;

                    case TailTuftVars tailTuftVars:
                        {
                            bool tryBoth = true;
                            if (!finishedZoopSpecific)
                            {
                                finishedZoopSpecific = true;
                            }
                            else
                            {
                                tail = true;
                                if (otherTT != null)
                                {
                                    otherTT.Distance(tailTuftVars);
                                    tryBoth = false;
                                }
                            }

                            if (tryBoth)
                            {
                                float a = tailTuftCosmetic.Distance(tailTuftVars);
                                float b = mainTailTuftCosmetic.Distance(tailTuftVars);

                                if (a <= b && tailTuftCosmetic.Enabled)
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

                    case LongShoulderScalesVars longShoulderScalesVars:
                        body = true;
                        r += longShoulderScalesCosmetic.Distance(longShoulderScalesVars);
                        break;
                    case ShortBodyScalesVars shortBodyScalesVars:
                        body = true;
                        r += shortBodyScalesCosmetic.Distance(shortBodyScalesVars);
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
