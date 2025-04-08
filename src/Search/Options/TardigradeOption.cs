using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class TardigradeOption : Option
    {
        private readonly ColorHSLInput bodyColorInput;
        private readonly ColorHSLInput secondaryColorInput;

        private readonly FloatInput scaleInput;
        private readonly IntInput numSpikesInput;

        public TardigradeOption()
        {
            elements = [
                bodyColorInput = new ColorHSLInput("Body color", true, 0.3f, 0.9f ,true, 0.3f, 0.47f, true, 0.5f, 0.8f),
                secondaryColorInput = new ColorHSLInput("Secondary color", true, 0f, 1f ,true, 0.75f, 1f, true, 0.5f, 0.7f),
                new Whitespace(),
                scaleInput = new FloatInput("General scale", 0.46f, 0.7f),
                numSpikesInput = new IntInput("Spikes per side", 2, 6)
            ];
        }

        private Variations GetVariations(XORShift128 Random)
        {
            var personality = new Personality(Random);

            float scale = 0.55f + ((personality.dom - Mathf.Pow(Random.Value, 2f)) * 0.6f + (1f - Mathf.Pow(Random.Value, 2f)) * 0.4f) * 0.15f;
            var bodyColor = new HSLColor(0.3f + Random.Value * 0.6f, 0.3f + Random.Value * 0.17f, 0.5f + Random.Value * 0.3f);
            var secondaryColor = new HSLColor(Random.Value, 0.75f + Random.Value * 0.25f, 0.5f + Random.Value * 0.2f);
            int spikesPerSide = Random.Range(2, 7);
            /*float spikeWidth = Random.Value;
            float spikeLength = Random.Value;
            float spikeLayBack = Random.Value * 0.7f + 0.3f;
            float spikePuffOut = Random.Value * 0.8f;
            float earWidth = Random.Value;
            float earLength = Random.Value;*/


            return new Variations
            {
                bodyColor = bodyColor,
                secondaryColor = secondaryColor,
                scale = scale,
                numSpikes = spikesPerSide
            };
        }

        public override float Execute(XORShift128 Random)
        {
            var vars = GetVariations(Random);

            float r = 0f;
            r += DistanceIf(vars.bodyColor, bodyColorInput);
            r += DistanceIf(vars.secondaryColor, secondaryColorInput);
            r += DistanceIf(vars.scale, scaleInput);
            r += DistanceIf(vars.numSpikes, numSpikesInput);

            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var vars = GetVariations(Random);

            yield return $"Body color: hsl({vars.bodyColor.hue}, {vars.bodyColor.saturation}, {vars.bodyColor.lightness})";
            yield return $"Secondary color: hsl({vars.secondaryColor.hue}, {vars.secondaryColor.saturation}, {vars.secondaryColor.lightness})";
            yield return null!;
            yield return $"General scale: {vars.scale}";
            yield return $"Spikes per side: {vars.numSpikes}";
        }

        private struct Variations
        {
            public HSLColor bodyColor;
            public HSLColor secondaryColor;
            public float scale;
            public int numSpikes;
        }
    }
}
