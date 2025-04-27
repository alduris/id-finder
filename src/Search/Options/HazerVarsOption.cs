using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class HazerVarsOption : Option
    {
        private readonly Alternatable<HazerColorGroup> ColorInputs;
        private readonly IntInput NumTentaclesInput, NumScalesInput;

        public HazerVarsOption()
        {
            HazerColorGroup a, b;
            elements = [
                ColorInputs = new Alternatable<HazerColorGroup>(
                    "Alternative skin color", false,
                    a = new HazerColorGroup(
                        new ColorHSLInput("Skin color", true, 0.345f, 0.351f, true, 0.6f, 0.7f, true, 0.7f, 0.8f),
                        new ColorHSLInput("Secondary color", true, 0.245f, 0.451f, true, 0.6f, 1f, true, 0.3f, 0.8f),
                        new ColorHSLInput("Eye color", false, 0.795f, 0.901f, false, 1f, 1f, true, 0.4f, 0.5f)
                        ),
                    b = new HazerColorGroup(
                        new ColorHSLInput("Skin color", true, 0.557f, 0.563f, true, 0.6f, 0.7f, true, 0.7f, 0.8f),
                        new ColorHSLInput("Secondary color", true, 0.457f, 0.663f, true, 0.6f, 1f, true, 0.3f, 0.8f),
                        new ColorHSLInput("Eye color", false, 0.007f, 0.113f, false, 1f, 1f, true, 0.4f, 0.5f)
                        )
                    ) { HasBias = false },
                new Whitespace(),
                NumTentaclesInput = new IntInput("Number of tentacles", 2, 6),
                NumScalesInput = new IntInput("Number of scales", 4, 13),
                ];
            AddEventListeners(a);
            AddEventListeners(b);
            a.SecondColorInput.HueInput.description = b.SecondColorInput.HueInput.description = "Will never differ more than 0.1 from skin color";
        }

        private void AddEventListeners(HazerColorGroup group)
        {
            group.SkinColorInput.OnValueChanged += UpdateEyeColor;
            group.SecondColorInput.OnValueChanged += UpdateEyeColor;

            void UpdateEyeColor(ColorHSLInput _, HSLColor __, HSLColor ___)
            {
                group.EyeColorInput.defaultHue = Custom.Decimal((group.SkinColorInput.HueInput.value + group.SecondColorInput.HueInput.value) / 2 + 0.5f);
                group.EyeColorInput.ForceUpdateColor();
            }
        }

        private HazerResults GetResults(XORShift128 Random)
        {
            var skinColor = new HSLColor(((Random.Value < 0.5f) ? 0.348f : 0.56f) + Mathf.Lerp(-0.03f, 0.03f, Random.Value), 0.6f + Random.Value * 0.1f, 0.7f + Random.Value * 0.1f);
            var secondColor = new HSLColor(skinColor.hue + Mathf.Lerp(-0.1f, 0.1f, Random.Value), Mathf.Lerp(skinColor.saturation, 1f, Random.Value), skinColor.lightness - Random.Value * 0.4f);
            var eyeColor = new HSLColor((skinColor.hue + secondColor.hue) * 0.5f + 0.5f, 1f, 0.4f + Random.Value * 0.1f);

            int numTentacles = ((Random.Value < 0.5f) ? Random.Range(3, 6) : Random.Range(2, 7));
            for (int i = 0; i < numTentacles; i++)
            {
                float length = Mathf.Lerp((34f / numTentacles) * Mathf.Lerp(0.7f, 1.2f, Random.Value), 8f, 0.2f);
                Random.Shift(Math.Max(2, Mathf.RoundToInt(length)) * 2);
            }

            int numScales = Random.Range(4, 14);

            return new HazerResults
            {
                skinColor = skinColor,
                secondColor = secondColor,
                eyeColor = eyeColor,
                numTentacles = numTentacles,
                numScales = numScales
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);
            float r = 0f;
            if (ColorInputs.Enabled)
            {
                var group = ColorInputs.Element;
                r += DistanceIf(results.skinColor, group.SkinColorInput)
                    + DistanceIf(results.secondColor, group.SecondColorInput)
                    + DistanceIf(results.eyeColor, group.EyeColorInput);
            }
            r += DistanceIf(results.numTentacles, NumTentaclesInput)
                + DistanceIf(results.numScales, NumScalesInput);
            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);
            yield return $"Skin color: hsl({results.skinColor.hue}, {results.skinColor.saturation}, {results.skinColor.lightness})";
            yield return $"Second color: hsl({results.secondColor.hue}, {results.secondColor.saturation}, {results.secondColor.lightness})";
            yield return $"Eye color: hsl({results.eyeColor.hue}, {results.eyeColor.saturation}, {results.eyeColor.lightness})";
            yield return null!;
            yield return $"Number of tentacles: {results.numTentacles}";
            yield return $"Number of scales: {results.numScales}";
        }

        private struct HazerResults
        {
            public HSLColor skinColor, secondColor, eyeColor;
            public int numTentacles, numScales;
        }

        /// <summary>
        /// Simplified group for easier access of members
        /// </summary>
        /// <param name="skinColorInput">Skin color input</param>
        /// <param name="secondColorInput">Secondary color input</param>
        /// <param name="eyeColorInput">Eye color input</param>
        private class HazerColorGroup(ColorHSLInput skinColorInput, ColorHSLInput secondColorInput, ColorHSLInput eyeColorInput) : Group([skinColorInput, secondColorInput, eyeColorInput], "Hazer color group")
        {
            public ColorHSLInput SkinColorInput = skinColorInput;
            public ColorHSLInput SecondColorInput = secondColorInput;
            public ColorHSLInput EyeColorInput = eyeColorInput;
        }
    }
}
