using System;
using FinderMod.Inputs.LizardCosmetics;
using static FinderMod.Search.Util.LizardUtil;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;
using UnityEngine;

namespace FinderMod.Search.Options.LizardCosmetics
{
    internal class ZoopLizardCosmetics : BaseLizardCosmetics, ICanGPUSometimes
    {
        private readonly WingScalesCosmetic mainWingScalesCosmetic;
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
                    mainWingScalesCosmetic = new WingScalesCosmetic(),
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
            cosmetics.Add(Toggleable("Has LongHeadScales", longHeadScalesCosmetic = new LongHeadScalesCosmetic(type)));
        }

        public bool AllowGPU => rotTypeInput == null || rotTypeInput.value == RotType.None;
        public ComputeShader Shader => InternalShaders.zoopLizardCosmeticsShader;

        public ICanGPU.GPUInput[] GetGPUInputs()
        {
            return [
                .. mainWingScalesCosmetic.GetGPUInputs(true),
                .. mainSpineSpikesCosmetic.GetGPUInputs(true),
                .. mainTailTuftCosmetic.GetGPUInputs(true),
                .. spineSpikesCosmetic.GetGPUInputs(true),
                .. bumpHawkCosmetic.GetGPUInputs(true),
                .. longShoulderScalesCosmetic.GetGPUInputs(true),
                .. shortBodyScalesCosmetic.GetGPUInputs(true),
                .. tailTuftCosmetic.GetGPUInputs(true),
                .. longHeadScalesCosmetic.GetGPUInputs(true),
                ];
        }

        public override float Execute(XORShift128 Random)
        {
            float r = 0f;
            bool body = false;
            bool tail = false;
            bool lhs = false;

            foreach (var result in GetResults(Random))
            {
                switch (result)
                {
                    case WingScalesVars wingScalesVars:
                        r += mainWingScalesCosmetic.Distance(wingScalesVars);
                        break;
                    case SpineSpikesVars spineSpikesVars:
                        if (spineSpikesVars.id == 0)
                        {
                            r += mainSpineSpikesCosmetic.Distance(spineSpikesVars);
                        }
                        else
                        {
                            body = true;
                            r += spineSpikesCosmetic.Distance(spineSpikesVars);
                        }
                        break;

                    case TailTuftVars tailTuftVars:
                        if (tailTuftVars.id == 1)
                        {
                            r += mainTailTuftCosmetic.Distance(tailTuftVars);
                        }
                        else
                        {
                            r += tailTuftCosmetic.Distance(tailTuftVars);
                            tail = true;
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
                        r += lizardRotCosmetic!.Distance(lizardRotVars);
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
