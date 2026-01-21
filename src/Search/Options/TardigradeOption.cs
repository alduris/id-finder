using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class TardigradeOption : Option
    {
        private readonly ColorHSLInput bodyColorInput;
        private readonly ColorHSLInput secondaryColorInput;

        private readonly FloatInput scaleInput;
        private readonly IntInput spikesPerSideInput;
        private readonly FloatInput spikeWidthInput;
        private readonly FloatInput spikeLengthInput;
        private readonly FloatInput spikeLayBackInput;
        private readonly FloatInput spikePuffOutInput;
        private readonly FloatInput earWidthInput;
        private readonly FloatInput earLenghtInput;

        public TardigradeOption()
        {
            elements = [
                bodyColorInput = new ColorHSLInput("Body color", true, 0.3f, 0.9f ,true, 0.3f, 0.47f, true, 0.5f, 0.8f),
                secondaryColorInput = new ColorHSLInput("Secondary color", true, 0f, 1f ,true, 0.75f, 1f, true, 0.5f, 0.7f),
                new Whitespace(),
                scaleInput = new FloatInput("General scale", 0.46f, 0.7f),
                new Whitespace(),
                spikesPerSideInput = new IntInput("Spikes per side", 2, 6),
                spikeWidthInput = new FloatInput("Spike width"),
                spikeLengthInput = new FloatInput("Spike length"),
                spikeLayBackInput = new FloatInput("Spike lay back", 0.3f, 1f),
                spikePuffOutInput = new FloatInput("Spike puff out", 0f, 0.8f),
                new Whitespace(),
                earWidthInput = new FloatInput("Ear width"),
                earLenghtInput = new FloatInput("Ear length"),
            ];
        }

        public override float Execute(XORShift128 Random)
        {
            var vars = new Variations(Random);

            float r = 0f;
            r += DistanceIf(vars.bodyColor, bodyColorInput);
            r += DistanceIf(vars.secondaryColor, secondaryColorInput);
            r += DistanceIf(vars.scale, scaleInput);
            r += DistanceIf(vars.spikesPerSide, spikesPerSideInput);
            r += DistanceIf(vars.spikeWidth, spikeWidthInput);
            r += DistanceIf(vars.spikeLength, spikeLengthInput);
            r += DistanceIf(vars.spikeLayBack, spikeLayBackInput);
            r += DistanceIf(vars.spikePuffOut, spikePuffOutInput);
            r += DistanceIf(vars.earWidth, earWidthInput);
            r += DistanceIf(vars.earLength, earLenghtInput);

            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var vars = new Variations(Random);

            yield return $"Body color: hsl({vars.bodyColor.hue}, {vars.bodyColor.saturation}, {vars.bodyColor.lightness})";
            yield return $"Secondary color: hsl({vars.secondaryColor.hue}, {vars.secondaryColor.saturation}, {vars.secondaryColor.lightness})";
            yield return null!;
            yield return $"General scale: {vars.scale}";
            yield return null!;
            yield return $"Spikes per side: {vars.spikesPerSide}";
            yield return $"Spike width: {vars.spikeWidth}";
            yield return $"Spike length: {vars.spikeLength}";
            yield return $"Spike lay back: {vars.spikeLayBack}";
            yield return $"Spike puff out: {vars.spikePuffOut}";
            yield return null!;
            yield return $"Ear width: {vars.earWidth}";
            yield return $"Ear length: {vars.earLength}";
        }

        private struct Variations
        {
            public HSLColor bodyColor;
            public HSLColor secondaryColor;
            public float scale;
            public int spikesPerSide;
            public float spikeWidth;
            public float spikeLength;
            public float spikeLayBack;
            public float spikePuffOut;
            public float earWidth;
            public float earLength;

            public Variations(XORShift128 Random)
            {
                var personality = new Personality(Random);

                scale = 0.55f + ((personality.dom - Mathf.Pow(Random.Value, 2f)) * 0.6f + (1f - Mathf.Pow(Random.Value, 2f)) * 0.4f) * 0.15f;
                bodyColor = new HSLColor(0.3f + Random.Value * 0.6f, 0.3f + Random.Value * 0.17f, 0.5f + Random.Value * 0.3f);
                secondaryColor = new HSLColor(Random.Value, 0.75f + Random.Value * 0.25f, 0.5f + Random.Value * 0.2f);
                spikesPerSide = Random.Range(2, 7);
                spikeWidth = Random.Value;
                spikeLength = Random.Value;
                spikeLayBack = Random.Value * 0.7f + 0.3f;
                spikePuffOut = Random.Value * 0.8f;
                earWidth = Random.Value;
                earLength = Random.Value;
            }
        }
    }
}
