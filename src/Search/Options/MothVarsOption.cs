using System;
using System.Collections.Generic;
using System.Drawing;
using FinderMod.Inputs;
using UnityEngine;
using Color = UnityEngine.Color; // STOP POPPING UP System.Drawing NOBODY LIKES YOU

namespace FinderMod.Search.Options
{
    public class MothVarsOption : Option
    {
        private readonly bool big;
        private readonly float baseSize;

        private readonly FloatInput ScaleInput;
        private readonly FloatInput ChubInput;
        private readonly FloatInput LegPuffInput;
        private readonly IntInput? AntennaLength;
        private readonly FloatInput AntennaWidth;

        private readonly IntInput? TaggedLegInput;
        private readonly MultiChoiceInput? TagOwnerInput;

        public MothVarsOption(bool big)
        {
            this.big = big;
            baseSize = big ? 1f : 0.3f;
            elements = [
                ScaleInput = new FloatInput("Size", baseSize * 0.78f, baseSize * 1.1f),
                ChubInput = new FloatInput("Chub"),
                LegPuffInput = new FloatInput("Leg puff"),
                ];
            if (big) elements.Add(AntennaLength = new IntInput("Antenna length", Mathf.Max(3, Mathf.RoundToInt(4 * 0.78f * baseSize)), Mathf.Max(3, Mathf.RoundToInt(6 * 1.1f * baseSize))));
            elements.Add(AntennaWidth = new FloatInput("Antenna width", 2f * 0.78f * baseSize, 7f * 1.1f * baseSize));

            // TODO: include colors somehow. It's so strict I almost want to make a custom color input for it.
            // It doesn't use HSL so can't use that and strict enough that the generic RGB input won't do due to its limited axes.
            // Would have to create some multidimensional color picker (by which I mean multiple sliders with a preview)
            // but the issue with that is how do we allow enabling/disabling of it because body and secondary colors are inherantly tied to one another

            // Tag if small
            if (!big)
            {
                elements.Add(new Whitespace());
                elements.Add(TagOwnerInput = new MultiChoiceInput("Tag owner", ["NONE", "GA (green)", "DG (blue)"], 0) { enabled = false });
                elements.Add(TaggedLegInput = new IntInput("Tagged leg", 0, 3) { enabled = false, description = "If tag owner is GA, will always be 2. Disable this input if no tag owner." });
            }

            if (AntennaLength != null)
            {
                AntennaLength.description = "Note: antenna length of 7 is very rare, with only 4 ids having it between ids 0 and 1000000.";
            }
        }

        private MothResults GetResults(XORShift128 Random)
        {
            var p = new Personality(Random);

            float scale = baseSize * 0.9f + ((p.dom - Mathf.Pow(Random.Value, 2f)) * 0.6f + (1f - Mathf.Pow(Random.Value, 2f)) * 0.4f) * 0.2f;

            int antennaeLength = Mathf.Max(Mathf.RoundToInt(Random.Range(4, 7) * scale), 3);
            float antennaeWidth = Random.Range(2f, 7f) * scale;

            Color bodyColor = Color.white;
            Color accentColor = Color.Lerp(new Color(0.5f, 0.25f, 0.1f), new Color(0.6f, 0.5f, 0.4f), Random.Value);
            if (big)
            {
                bodyColor = Color.Lerp(bodyColor, accentColor, Mathf.Pow(Random.Value, 2f) * 0.17f);
            }
            else
            {
                bodyColor = Color.Lerp(Color.Lerp(new Color(0.35f, 0.35f, 0.35f), new Color(0.433f, 0.433f, 0.433f), Mathf.Pow(Random.Value, 2f)), accentColor, Mathf.Pow(Random.Value, 2f) * 0.1f);
            }
            bodyColor = Color.Lerp(bodyColor, new Color(1f, 0.7f, 0f), (p.dom - Mathf.Pow(Random.Value, 2f)) * 0.1f);
            Color secondaryColor = Color.Lerp(Color.Lerp(bodyColor, new HSLColor(Random.Value * 0.17f, 1f, 0.5f).rgb, Random.Value * (!big ? 0.2f : 0.4f)), !big ? Color.white : Color.black, !big ? 0.7f : 0.3f);

            bool hasTag = (double)Random.Value < 0.1;

            Random.Shift();
            float chub = Mathf.Lerp(Random.Value, 0.5f, Random.Value);
            float legPuff = Mathf.Pow(Random.Value, 2f);
            int taggedLeg = Random.Range(0, 4);

            int tagOwner;
            if (hasTag)
            {
                tagOwner = (Random.Value > 0.4f) ? 1 : 2;
            }
            else
            {
                tagOwner = 0;
            }

            if (tagOwner == 0) taggedLeg = -1;
            else if (tagOwner == 1) taggedLeg = 2;

            return new MothResults
            {
                size = scale,
                chub = chub,
                antennaeLength = antennaeLength,
                antennaeWidth = antennaeWidth,
                bodyColor = bodyColor,
                secondaryColor = secondaryColor,
                taggedLeg = taggedLeg,
                tagOwner = tagOwner,
            };
        }
        public override float Execute(XORShift128 Random)
        {
            var results = GetResults(Random);
            float r = 0f;

            r += DistanceIf(results.size, ScaleInput);
            r += DistanceIf(results.chub, ChubInput);
            r += DistanceIf(results.antennaeLength, AntennaLength);
            r += DistanceIf(results.antennaeWidth, AntennaWidth);

            r += DistanceIf(results.taggedLeg, TaggedLegInput);
            r += DistanceIf(results.tagOwner, TagOwnerInput);

            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var results = GetResults(Random);
            yield return $"Size: {results.size}";
            yield return $"Chub: {results.chub}";
            yield return $"Antennae length: {results.antennaeLength}";
            yield return $"Antennae width: {results.antennaeWidth}";
            yield return null!;
            yield return $"Body color: rgb({results.bodyColor.r}, {results.bodyColor.g}, {results.bodyColor.b})";
            yield return $"Secondary color: rgb({results.secondaryColor.r}, {results.secondaryColor.g}, {results.secondaryColor.b})";
            yield return null!;
            yield return $"Tag owner: {results.tagOwner switch { 1 => "GA (green)", 2 => "DG (blue)", _ => "None" }}";
            if (results.tagOwner > 0) yield return $"Tagged leg: {results.taggedLeg}";
        }

        private struct MothResults
        {
            public float size;
            public float chub;
            public int antennaeLength;
            public float antennaeWidth;
            public Color bodyColor;
            public Color secondaryColor;
            public int taggedLeg;
            public int tagOwner;
        }
    }
}
