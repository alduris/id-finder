using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class RatOption : Option
    {
        private static readonly Color coatStartColor = new(0.29f, 0.31f, 0.33f), coatEndColor = new(0.42f, 0.26f, 0.07f);

        private readonly BoolInput bigEyesInput;
        private readonly FloatInput whiskerLengthInput;
        private readonly FloatInput coatDarknessInput;
        private readonly ColorLerpInput coatColorInput;
        private readonly ColorHSLInput headColorInput;

        public RatOption()
        {
            elements = [
                bigEyesInput = new BoolInput("Has big eyes", false) {hasBias = true},
                whiskerLengthInput = new FloatInput("Whisker length", 20f, 35f),
                coatDarknessInput = new FloatInput("Coat darkness", 0.6f, 1f),
                coatColorInput = new ColorLerpInput("Coat color", coatStartColor, coatEndColor) {bias = 3},
                headColorInput = new ColorHSLInput("Head color", true, -0.1f, 0.1f, true, 0f, 0.4f, true, 0.2f, 0.4f) {fixColors = true},
            ];
        }

        private Variations GetVariations(XORShift128 Random)
        {
            var personality = new Personality(Random);
            var bigEyes = Random.Value < 0.05f;
            var whiskerLength = Mathf.Lerp(20f, 35f, personality.dom);
            var coatDarkness = 1f - Mathf.Pow(Random.Value, 2f) * 0.4f;
            var coatColor = Random.Value;
            var headColor = new HSLColor(WrappedRandomVariation(0f, 0.1f, 0.1f, Random), ClampedRandomVariation(0.2f, 0.2f, 0.1f, Random), ClampedRandomVariation(0.3f, 0.1f, 0.1f, Random));
            return new Variations
            {
                bigEyes = bigEyes,
                whiskerLength = whiskerLength,
                coatDarkness = coatDarkness,
                coatColor = coatColor,
                headColor = headColor
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var vars = GetVariations(Random);

            float r = 0f;
            r += DistanceIf(vars.bigEyes, bigEyesInput);
            r += DistanceIf(vars.whiskerLength, whiskerLengthInput);
            r += DistanceIf(vars.coatDarkness, coatDarknessInput);
            r += DistanceIf(vars.coatColor, coatColorInput);
            r += DistanceIf(vars.headColor, headColorInput);

            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var vars = GetVariations(Random);
            yield return $"Big eyes: {vars.bigEyes}";
            yield return $"Whisker length: {vars.whiskerLength}";
            yield return null!;
            yield return $"Coat darkness: {vars.coatDarkness}";
            var coatColor = Color.Lerp(coatStartColor, coatEndColor, vars.coatColor);
            yield return $"Coat color: rgb({coatColor.r}, {coatColor.g}, {coatColor.b})";
            yield return $"Head color: hsl({vars.headColor.hue}, {vars.headColor.saturation}, {vars.headColor.lightness})";
        }

        private struct Variations
        {
            public bool bigEyes;
            public float whiskerLength;
            public float coatDarkness;
            public float coatColor;
            public HSLColor headColor;
        }
    }
}
