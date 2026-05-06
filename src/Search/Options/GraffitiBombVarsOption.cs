using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class GraffitiBombVarsOption : Option
    {
        private readonly IntInput DotsInput;
        private readonly ColorHSLInput ColorInput;

        public GraffitiBombVarsOption()
        {
            RepresentedObject = AbstractPhysicalObject.AbstractObjectType.GraffitiBomb;
            elements = [
                DotsInput = new IntInput("Number of dots", 5, 7),
                ColorInput = new ColorHSLInput("Color", true, 50f / 360f, 350f / 360f, false, 1f, 1f, true, 0.6f, 0.8f)
                ];
        }

        public override float Execute(XORShift128 Random)
        {
            var vars = new Variations(Random);
            float r = 0f;
            r += DistanceIf(vars.dots, DotsInput);
            r += DistanceIf(vars.color, ColorInput);
            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var vars = new Variations(Random);
            yield return $"Number of dots: {vars.dots}";
            yield return $"Color: hsl({vars.color.hue}, {vars.color.saturation}, {vars.color.lightness})";
        }

        private struct Variations
        {
            public HSLColor color;
            public int dots;

            private static readonly float[,] hueWeights = new float[,]
            {
                { 5f, 260f },
                { 4f, 80f },
                { 3f, 200f },
                { 2f, 320f },
                { 1f, 140f }
            };

            public Variations(XORShift128 Random)
            {
                var (x, y, z, w) = (Random.x, Random.y, Random.z, Random.w);

                // Dots
                dots = Random.Range(5, 8);

                // Color
                Random.InitState(x, y, z, w);
                float baseHue = 250f;
                float weight = Random.Range(0f, 15f);
                float offset = 0.5f;
                for (int i = 0; i < 5; i++)
                {
                    float num4 = hueWeights[i, 0];
                    if (weight < num4)
                    {
                        offset = (num4 - weight) / num4;
                        baseHue = hueWeights[i, 1];
                        break;
                    }
                    weight -= num4;
                }
                if (offset < 0.5f)
                {
                    baseHue -= Mathf.Lerp(30f, 0f, Mathf.Pow(Mathf.Sin(offset * 3.1415927f), 2f));
                }
                else
                {
                    baseHue += Mathf.Lerp(0f, 30f, 1f - Mathf.Pow(Mathf.Sin(offset * 3.1415927f), 2f));
                }
                float baseLightness = Mathf.Lerp(0.6f, 0.7f, Mathf.DeltaAngle(baseHue, 260f) / 180f);
                color = new HSLColor(baseHue / 360f, 1f, Mathf.Lerp(baseLightness, 0.8f, Random.Value));
            }
        }
    }
}
