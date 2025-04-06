using System;
using static FinderMod.Search.Util.LizardUtil;
using static FinderMod.Inputs.LizardCosmetics.CosmeticsItemContainer;
using FinderMod.Inputs.LizardCosmetics;

namespace FinderMod.Search.Options.LizardCosmetics
{
    public class CyanLizardCosmetics : BaseLizardCosmetics
    {
        private readonly WingScalesCosmetic wingScalesInput;
        private readonly TailTuftCosmetic tailTuftInput;
        private readonly TailGeckoScalesCosmetic tailGeckoScalesInput;

        public CyanLizardCosmetics() : base(LizardType.Cyan)
        {
            cosmetics.Add(Toggleable("Has WingScales", wingScalesInput = new WingScalesCosmetic()));
            cosmetics.Add(OneOf(
                "Tail cosmetic",
                tailTuftInput = new TailTuftCosmetic(),
                tailGeckoScalesInput = new TailGeckoScalesCosmetic()
                ));
            // cosmetics.Add(new JumpRingsCosmetic());
        }

        public override float Execute(XORShift128 Random)
        {
            float r = 0f;
            bool wingScales = false;

            foreach (var result in GetResults(Random))
            {
                switch (result)
                {
                    case WingScalesVars wingScalesVars:
                        wingScales = true;
                        if (wingScalesInput.Active)
                        {
                            r += DistanceIf(wingScalesVars.scaleLength, wingScalesInput.LengthInput);
                            r += DistanceIf(wingScalesVars.numScales, wingScalesInput.NumScalesInput);
                        }
                        else if (wingScalesInput.Enabled && !wingScalesInput.Toggled)
                        {
                            r += 100f;
                        }
                        break;
                    case TailTuftVars tailTuftVars:
                        if (tailTuftInput.Enabled && !tailTuftInput.Toggled) r += 100f;
                        break;
                    case TailGeckoScalesVars tailGeckoScalesVars:
                        if (tailGeckoScalesInput.Active)
                        {
                            r += DistanceIf(tailGeckoScalesVars.rows, tailGeckoScalesInput.RowsInput);
                            r += DistanceIf(tailGeckoScalesVars.lines, tailGeckoScalesInput.LinesInput);
                        }
                        else if (tailGeckoScalesInput.Enabled && !tailGeckoScalesInput.Toggled)
                        {
                            r += 100f;
                        }
                        break;
                    case JumpRingsVars:
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected result! " + result.GetType().Name);
                }
            }

            if (!wingScales && wingScalesInput.Enabled && wingScalesInput.Toggled) r += 100f;

            return r;
        }
    }
}
