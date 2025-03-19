using FinderMod.Inputs;
using UnityEngine;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Search.Options
{
    public class LizardVarsOption : Option
    {
        private readonly EnumInput<LizardType> typeInp;
        private readonly FloatInput hsInp, fatInp, tlInp, tfInp, tcInp;

        public LizardVarsOption() : base("Lizard Variations")
        {
            elements = [
                typeInp = new EnumInput<LizardType>("Lizard type", LizardType.Green) { forceEnabled = true },
                new Whitespace(),
                hsInp = new FloatInput("Head size", 0.86f, 1.14f),
                fatInp = new FloatInput("Fatness", 0.76f, 1.24f),
                tlInp = new FloatInput("Tail length", 0.6f, 1.4f),
                tfInp = new FloatInput("Tail fatness", 0.7f, 1.1f),
                tcInp = new FloatInput("Tail color") { description = "Strength of tail gradient. This will be 0 (none) roughly 50% of the time, and will always be 0 for white lizards." }
            ];
        }

        public override float Execute(XORShift128 Random)
        {
            LizardType type = typeInp.value;

            float headSize = ClampedRandomVariation(0.5f, 0.07f, 0.5f, Random) * 2f;
            if (Random.Value < 0.5f)
            {
                headSize = 1f;
            }
            float fatness = ClampedRandomVariation(0.5f, 0.12f, 0.5f, Random) * 2f;
            float tailLength = ClampedRandomVariation(0.5f, 0.2f, 0.3f, Random) * 2f;
            float tailFatness = ClampedRandomVariation(0.45f, 0.1f, 0.3f, Random) * 2f;
            float tailColor = 0f;
            if (type != LizardType.White && Random.Value > 0.5f)
            {
                tailColor = Random.Value;
            }
            if (type == LizardType.Red)
            {
                fatness = Mathf.Min(1f, fatness);
                tailFatness = Mathf.Min(1f, tailFatness);
            }
            else if (type == LizardType.Black)
            {
                fatness = ClampedRandomVariation(0.45f, 0.06f, 0.5f, Random) * 2f;
            }
            else if (type == LizardType.Caramel || type == LizardType.Zoop)
            {
                fatness = Mathf.Min(0.8f, fatness);
                tailFatness = Mathf.Min(0.9f, tailFatness);
            }

            float r = 0f;
            r += DistanceIf(headSize, hsInp);
            r += DistanceIf(fatness, fatInp);
            r += DistanceIf(tailLength, tlInp);
            r += DistanceIf(tailFatness, tfInp);
            r += DistanceIf(tailColor, tcInp);
            return r;
        }
    }
}
