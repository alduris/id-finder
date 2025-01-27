using System;
using FinderMod.Inputs;
using static FinderMod.Search.Util.ScavUtil;
using UnityEngine;
using RWCustom;

namespace FinderMod.Search.Options
{
    public class ScavBackPatternOption : Option
    {
        private readonly MultiChoiceInput stInp, spInp, ctInp;
        private readonly IntInput nsInp;
        private readonly FloatInput csInp, rsInp, reInp, gsInp;

        public ScavBackPatternOption() : base("Scavenger Back Patterns")
        {
            elements = [
                new Label("See hover descriptions at bottom for some inputs"),
                new Whitespace(),
                stInp = new MultiChoiceInput("Spine type", ["HardBackSpikes", "WobblyBackTufts"]),
                spInp = new MultiChoiceInput("Spine pattern", ["SpineRidge", "DoubleSpineRidge", "RandomBackBlotch"]),
                nsInp = new IntInput("Number of spines", 2, 40) { description = "SpineRidge range: (2, 37), DoubleSpineRidge range: (2, 40), RandomBackBlotch range: (4, 40)" },
                new Whitespace(),
                ctInp = new MultiChoiceInput("Color type", ["None", "Decoration", "Head"]),
                csInp = new FloatInput("Color strength") { description = "With color type = none, this will always be 0" },
                new Whitespace(),
                rsInp = new FloatInput("Range start", 0.02f, 0.3f) { description = "Where along the back the spines start generating, higher = lower down the back" },
                reInp = new FloatInput("Range end", 0.4f, 1f) { description = "Where along the back the spines stop generating, higher = lower down the back" },
                gsInp = new FloatInput("General spine size")
            ];
        }

        public override float Execute(XORShift128 Random)
        {
            var p = new Personality(Random);

            // Pre-generate some attributes we will need later
            float generalMelanin = Custom.PushFromHalf(Random.Value, 2f);
            float headSize = ClampedRandomVariation(0.5f, 0.5f, 0.1f, Random);
            Random.Shift();
            float eyeSize = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(Random.Value, Mathf.Pow(headSize, 0.5f), Random.Value * 0.4f)), Mathf.Lerp(0.95f, 0.55f, p.sym));
            float narrowEyes = (Random.Value < Mathf.Lerp(0.3f, 0.7f, p.sym)) ? 0f : Mathf.Pow(Random.Value, Mathf.Lerp(0.5f, 1.5f, p.sym));

            // Calculate how far we have to advance the pointer
            #region skip advancement stuff

            Random.Shift(9); // next 9 rolls (eyes angle, fatness * 3, narrow waist * 3, neck thickness * 2)

            if (Random.Value < 0.65f && eyeSize > 0.4f && narrowEyes < 0.3f) // check to add pupils
            {
                // Pupils and stuff
                if (Random.Value < Mathf.Pow(p.sym, 1.5f) * 0.8f) // determine if sympathetic enough to have colored pupils
                {
                    // colored pupils
                    Random.Shift();
                    if (Random.Value < 0.6666667f) Random.Shift();
                }
                // else deep pupils
            }

            Random.Shift(8); // tick 8 more rolls (hands head color * 3, legs size, arm thickness * 2, colored eartler tips, wide teeth)
            int tailSegs = ((Random.Value < 0.5f) ? 0 : Random.Range(1, 5));
            if (Random.Value < 0.25f) Random.Shift(); // unused scruffy calculation that's still done for some reason

            // Color time!
            HSLColor bodyColor, headColor;
            float bodyColorBlack, headColorBlack;

            float bodyHue = Random.Value * 0.1f;
            if (Random.Value < 0.025f)
            {
                bodyHue = Mathf.Pow(Random.Value, 0.4f);
            }
            float accentHue1 = bodyHue + Mathf.Lerp(-1f, 1f, Random.Value) * 0.3f * Mathf.Pow(Random.Value, 2f);
            if (accentHue1 > 1f)
            {
                accentHue1 -= 1f;
            }
            else if (accentHue1 < 0f)
            {
                accentHue1 += 1f;
            }

            // Body color
            bodyColor = new HSLColor(bodyHue, Mathf.Lerp(0.05f, 1f, Mathf.Pow(Random.Value, 0.85f)), Mathf.Lerp(0.05f, 0.8f, Random.Value));
            bodyColor.saturation *= (1f - generalMelanin);
            bodyColor.lightness = Mathf.Lerp(bodyColor.lightness, 0.5f + 0.5f * Mathf.Pow(Random.Value, 0.8f), 1f - generalMelanin);
            bodyColorBlack = Custom.LerpMap((bodyColor.rgb.r + bodyColor.rgb.g + bodyColor.rgb.b) / 3f, 0.04f, 0.8f, 0.3f, 0.95f, 0.5f);
            bodyColorBlack = Mathf.Lerp(bodyColorBlack, Mathf.Lerp(0.5f, 1f, Random.Value), Random.Value * Random.Value * Random.Value);
            bodyColorBlack *= generalMelanin;
            Vector2 vector = new(bodyColor.saturation, Mathf.Lerp(-1f, 1f, bodyColor.lightness * (1f - bodyColorBlack)));
            if (vector.magnitude < 0.5f)
            {
                vector = Vector2.Lerp(vector, vector.normalized, Mathf.InverseLerp(0.5f, 0.3f, vector.magnitude));
                bodyColor = new HSLColor(bodyColor.hue, Mathf.InverseLerp(-1f, 1f, vector.x), Mathf.InverseLerp(-1f, 1f, vector.y));
                bodyColorBlack = Custom.LerpMap((bodyColor.rgb.r + bodyColor.rgb.g + bodyColor.rgb.b) / 3f, 0.04f, 0.8f, 0.3f, 0.95f, 0.5f);
                bodyColorBlack = Mathf.Lerp(bodyColorBlack, Mathf.Lerp(0.5f, 1f, Random.Value), Random.Value * Random.Value * Random.Value);
                bodyColorBlack *= generalMelanin;
            }

            // Magic number electric boogaloo
            float accentHue2;
            if (Random.Value < Custom.LerpMap(bodyColorBlack, 0.5f, 0.8f, 0.9f, 0.3f))
            {
                accentHue2 = accentHue1 + Mathf.Lerp(-1f, 1f, Random.Value) * 0.1f * Mathf.Pow(Random.Value, 1.5f);
                accentHue2 = Mathf.Lerp(accentHue2, 0.15f, Random.Value);
            }
            else
            {
                accentHue2 = ((Random.Value < 0.5f) ? Custom.Decimal(bodyHue + 0.5f) : Custom.Decimal(accentHue1 + 0.5f)) + Mathf.Lerp(-1f, 1f, Random.Value) * 0.25f * Mathf.Pow(Random.Value, 2f);
                if (Random.Value < Mathf.Lerp(0.8f, 0.2f, p.nrg)) // energy
                {
                    accentHue2 = Mathf.Lerp(accentHue2, 0.15f, Random.Value);
                }
            }
            if (accentHue2 > 1f)
            {
                accentHue2 -= 1f;
            }
            else if (accentHue2 < 0f)
            {
                accentHue2 += 1f;
            }

            // Head color
            headColor = new HSLColor((Random.Value < 0.75f) ? accentHue1 : accentHue2, 1f, 0.05f + 0.15f * Random.Value);
            headColor.saturation *= Mathf.Pow(1f - generalMelanin, 2f);
            headColor.lightness = Mathf.Lerp(headColor.lightness, 0.5f + 0.5f * Mathf.Pow(Random.Value, 0.8f), 1f - generalMelanin);
            headColor.saturation *= (0.1f + 0.9f * Mathf.InverseLerp(0.1f, 0f, Custom.DistanceBetweenZeroToOneFloats(bodyColor.hue, headColor.hue) * Custom.LerpMap(Mathf.Abs(0.5f - headColor.lightness), 0f, 0.5f, 1f, 0.3f)));
            if (headColor.lightness < 0.5f)
            {
                headColor.lightness *= (0.5f + 0.5f * Mathf.InverseLerp(0.2f, 0.05f, Custom.DistanceBetweenZeroToOneFloats(bodyColor.hue, headColor.hue)));
            }
            headColorBlack = Custom.LerpMap((headColor.rgb.r + headColor.rgb.g + headColor.rgb.b) / 3f, 0.035f, 0.26f, 0.7f, 0.95f, 0.25f);
            headColorBlack = Mathf.Lerp(headColorBlack, Mathf.Lerp(0.8f, 1f, Random.Value), Random.Value * Random.Value * Random.Value);
            headColorBlack *= 0.2f + 0.7f * generalMelanin;
            headColorBlack = Mathf.Max(headColorBlack, bodyColorBlack);
            headColor.saturation = Custom.LerpMap(headColor.lightness * (1f - headColorBlack), 0f, 0.15f, 1f, headColor.saturation);
            if (headColor.lightness > bodyColor.lightness)
            {
                headColor = bodyColor;
            }
            if (headColor.saturation < bodyColor.saturation * 0.75f) Random.Shift();

            if (Random.Value >= 0.65f) Random.Shift(); // deco color
            Random.Shift(3); // also deco color

            if (Random.Value < 0.2f) Random.Shift(); // eye color

            Random.Shift(2); // belly color
            if (Random.Value < 0.033333335f)
            {
                Random.Shift(3);
            }

            Random.Shift(tailSegs); // tail
            #endregion

            // Scav back thing time!
            bool useHardBackSpikes = Random.Value < 0.1f;
            float colorType = 0;
            float colored = 0;
            ScavBodyScalePattern pattern; // 1 = SpineRidge, 2 = DoubleSpineRidge, 3 = RandomBackBlotch
            float top, bottom;
            int numScales;
            float generalSize;

            // BackTuftsAndRidges constructor
            Random.Shift(2); // graphic
            if (Random.Value < 0.5f) Random.Shift();
            if (Random.Value > generalMelanin)
            {
                colored = Mathf.Pow(Random.Value, 0.5f);
            }
            if (colored > 0)
            {
                colorType = Random.Value < 0.5 ? 1 : 2;
            }
            else Random.Shift();

            // Inheritance time
            if (useHardBackSpikes)
            {
                // HardBackSpikes (requires 56 random values max)

                // Calculate pattern and generate corresponding attributes
                pattern = Random.Value < 0.6f ? ScavBodyScalePattern.SpineRidge : ScavBodyScalePattern.DoubleSpineRidge;
                if (Random.Value < 0.1f) pattern = ScavBodyScalePattern.RandomBackBlotch;
                GeneratePattern(Random, ScavBackType.HardBackSpikes, pattern, out top, out bottom, out numScales);

                // Advance pointer
                if (Random.Value < 0.5f && Random.Value < 0.85f) Random.Shift();
                Random.Shift(2);

                // General size
                generalSize = Custom.LerpMap(numScales, 5f, 35f, 1f, 0.2f);
                generalSize = Mathf.Lerp(generalSize, p.dom, Random.Value); // uses dominance
                generalSize = Mathf.Lerp(generalSize, Mathf.Pow(Random.Value, 0.75f), Random.Value);

                // Extra pointer offsetting (for future reference purposes)
                // if (colored > 0 && Random.Value < 0.25f + 0.5f * colored) Random.Shift();
            }
            else
            {
                // WobblyBackTufts (requires 59 random values max)

                // Calculate pattern, tick pointer, and generate corresponding attributes
                pattern = ScavBodyScalePattern.RandomBackBlotch;
                if (Random.Value < 0.25f && Random.Value < 0.05f) // scruffy is never 0f because it is hardcoded to 1f
                {
                    pattern = (Random.Value < 0.5f) ? ScavBodyScalePattern.DoubleSpineRidge : ScavBodyScalePattern.SpineRidge;
                }

                if (Random.Value >= 0.2f)
                {
                    if (Random.Value < 0.5f) Random.Shift(2);
                    else Random.Shift();
                }

                GeneratePattern(Random, ScavBackType.WobblyBackTufts, pattern, out top, out bottom, out numScales);

                // Pointer offsetting
                if (pattern == ScavBodyScalePattern.RandomBackBlotch) Random.Shift();
                Random.Shift();

                // General size
                generalSize = Mathf.Lerp(Random.Value, p.dom, Random.Value);
                generalSize = Mathf.Lerp(generalSize, Random.Value, Random.Value);
                generalSize = Mathf.Pow(generalSize, Mathf.Lerp(2f, 0.65f, p.dom));

                // More pointer offsetting (for future reference purposes)
                // j += 9;
                // j += numScales * (2 + (pattern == ScavBodyScalePattern.RandomBackBlotch ? 1 : 0) + (Random.Value != 1f ? 2 : 0));
                // if (colored > 0 && Random.Value < 0.25f + 0.5f * colored) Random.Shift();
            }

            float r = 0f;
            if (stInp.enabled) r += (useHardBackSpikes ? 0 : 1) ^ stInp.Value;
            if (ctInp.enabled) r += Distance(colorType, ctInp.Value);
            if (csInp.enabled) r += Distance(colored, csInp.Value);
            if (spInp.enabled) r += Distance((int)pattern, spInp.Value);
            if (rsInp.enabled) r += Distance(top, rsInp.Value);
            if (reInp.enabled) r += Distance(bottom, rsInp.Value);
            if (nsInp.enabled) r += Distance(numScales, nsInp.Value);
            if (gsInp.enabled) r += Distance(generalSize, gsInp.Value);

            return r;
        }
    }
}
