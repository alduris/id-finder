using System;
using FinderMod.Inputs.LizardCosmetics;
using UnityEngine;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Search.Options.LizardCosmetics
{
    internal class PeachLizardCosmetics : BaseLizardCosmetics, ICanGPUSometimes
    {
        private readonly SpineSpikesCosmetic spineSpikesCosmetic;
        private readonly BumpHawkCosmetic bumpHawkCosmetic;
        private readonly LongShoulderScalesCosmetic longShoulderScalesCosmetic;
        private readonly ShortBodyScalesCosmetic shortBodyScalesCosmetic;

        private readonly TailTuftCosmetic tailTuftCosmetic;
        private readonly LongHeadScalesCosmetic longHeadScalesCosmetic;

        private readonly PeachBodyFinCosmetic peachBodyFinCosmetic;
        private readonly TailFinCosmetic tailFinCosmetic;
        private readonly PeachHeadStripesCosmetic peachHeadStripesCosmetic;

        public PeachLizardCosmetics() : base(LizardType.Peach)
        {
            cosmetics.Add(Label("Peach-specific cosmetics group"));
            cosmetics.Add(
                Group("Peach-specific cosmetics",
                    tailFinCosmetic = new TailFinCosmetic(LizardType.Peach),
                    peachBodyFinCosmetic = new PeachBodyFinCosmetic(),
                    peachHeadStripesCosmetic = new PeachHeadStripesCosmetic()
                    ));
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
        public ComputeShader Shader => InternalShaders.peachLizardCosmeticsShader;

        public ICanGPU.GPUInput[] GetGPUInputs()
        {
            return [
                .. spineSpikesCosmetic.GetGPUInputs(true),
                .. bumpHawkCosmetic.GetGPUInputs(true),
                .. longShoulderScalesCosmetic.GetGPUInputs(true),
                .. shortBodyScalesCosmetic.GetGPUInputs(true),
                .. tailTuftCosmetic.GetGPUInputs(true),
                .. longHeadScalesCosmetic.GetGPUInputs(true),
                .. peachHeadStripesCosmetic.GetGPUInputs(false),
                .. tailFinCosmetic.GetGPUInputs(false),
                .. peachBodyFinCosmetic.GetGPUInputs(false),
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
                    case SpineSpikesVars spineSpikesVars:
                        body = true;
                        r += spineSpikesCosmetic.Distance(spineSpikesVars);
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

                    case TailTuftVars tailTuftVars:
                        tail = true;
                        r += tailTuftCosmetic.Distance(tailTuftVars);
                        break;

                    case LongHeadScalesVars longHeadScalesVars:
                        lhs = true;
                        r += longHeadScalesCosmetic.Distance(longHeadScalesVars);
                        break;

                    case PeachBackFinVars peachBackFinVars:
                        r += peachBodyFinCosmetic.Distance(peachBackFinVars);
                        break;
                    case TailFinVars tailFinVars:
                        r += tailFinCosmetic.Distance(tailFinVars);
                        break;
                    case PeachHeadStripesVars peachHeadStripesVars:
                        r += peachHeadStripesCosmetic.Distance(peachHeadStripesVars);
                        break;

                    case LizardRotVars lizardRotVars:
                        r += lizardRotCosmetic!.Distance(lizardRotVars);
                        break;

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
