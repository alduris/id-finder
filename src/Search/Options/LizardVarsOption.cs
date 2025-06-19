using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;
using static FinderMod.Search.Util.LizardUtil;

namespace FinderMod.Search.Options
{
    internal class LizardVarsOption : Option
    {
        private readonly EnumInput<LizardType> TypeInp;
        private readonly FloatInput HeadSizeInput, FatnessInput, TailLengthInput, TailFatnessInput, TailColorInput;

        public LizardVarsOption() : base()
        {
            elements = [
                TypeInp = new EnumInput<LizardType>("Lizard type", LizardType.Green) { forceEnabled = true, excludeOptions = [ LizardType.NONE ] },
                new Whitespace(),
                HeadSizeInput = new FloatInput("Head size", 0.86f, 1.14f),
                FatnessInput = new FloatInput("Fatness", 0.76f, 1.24f),
                TailLengthInput = new FloatInput("Tail length", 0.6f, 1.4f),
                TailFatnessInput = new FloatInput("Tail fatness", 0.7f, 1.1f),
                TailColorInput = new FloatInput("Tail color") { description = "Strength of tail gradient. This will be 0 (none) roughly 50% of the time, and will always be 0 for white lizards." }
            ];
        }

        private struct LizardVarResults
        {
            public float headSize, fatness, tailLength, tailFatness, tailColor;
        }

        private LizardVarResults GetResults(LizardType type, XORShift128 Random)
        {
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

            return new LizardVarResults
            {
                headSize = headSize,
                fatness = fatness,
                tailLength = tailLength,
                tailFatness = tailFatness,
                tailColor = tailColor,
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(TypeInp.value, Random);

            float r = 0f;
            r += DistanceIf(results.headSize, HeadSizeInput);
            r += DistanceIf(results.fatness, FatnessInput);
            r += DistanceIf(results.tailLength, TailLengthInput);
            r += DistanceIf(results.tailFatness, TailFatnessInput);
            r += DistanceIf(results.tailColor, TailColorInput);
            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var (x, y, z, w) = (Random.x, Random.y, Random.z, Random.w);
            foreach (LizardType type in Enum.GetValues(typeof(LizardType)))
            {
                Random.InitState(x, y, z, w);

                var results = GetResults(type, Random);

                yield return type.ToString();
                yield return $"  Head size: {results.headSize}";
                yield return $"  Fatness: {results.fatness}";
                yield return $"  Tail length: {results.tailLength}";
                yield return $"  Tail fatness: {results.tailFatness}";
                yield return $"  Tail color: {results.tailColor}";
                yield return null!;
            }
            yield break;
        }
    }
}
