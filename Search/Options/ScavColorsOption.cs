using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class ScavColorsOption : Option
    {
        private readonly ColorHSLInput body, head, deco, eye;

        public ScavColorsOption() : base("Scavenger Colors")
        {
            elements = [
                body = new ColorHSLInput("Body color"),
                head = new ColorHSLInput("Head color"),
                deco = new ColorHSLInput("Decoration color") { descripion = "Controls eartler tip colors if enabled. Can sometimes control eye or belly color." },
                eye = new ColorHSLInput("Eye color", true, false, true), // eye saturation is locked to 1
            ];
        }

        public override float Execute(XORShift128 Random)
        {
            Personality p = new Personality(Random);

            // Pre-generate some attributes we will need later
            float generalMelanin = Custom.PushFromHalf(Random.Value, 2f);
            float headSize = ClampedRandomVariation(0.5f, 0.5f, 0.1f, Random);
            Random.Shift(); // tick state for eartler width
            float eyeSize = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(Random.Value, Mathf.Pow(headSize, 0.5f), Random.Value * 0.4f)), Mathf.Lerp(0.95f, 0.55f, p.sym));
            float narrowEyes = (Random.Value < Mathf.Lerp(0.3f, 0.7f, p.sym)) ? 0f : Mathf.Pow(Random.Value, Mathf.Lerp(0.5f, 1.5f, p.sym));
            float pupilSize = 0f;
            bool deepPupils = false;
            bool hasColoredPupils = false;

            // Calculate how far we have to advance the pointer
            Random.Shift(9); // next 9 rolls (eyes angle, fatness * 3, narrow waist * 3, neck thickness * 2)

            if (Random.Value < 0.65f && eyeSize > 0.4f && narrowEyes < 0.3f) // check to add pupils
            {
                if (Random.Value < Mathf.Pow(p.sym, 1.5f) * 0.8f) // determine if sympathetic enough to have colored pupils
                {
                    // Colored pupils
                    pupilSize = Mathf.Lerp(0.4f, 0.8f, Mathf.Pow(Random.Value, 0.5f));
                    if (Random.Value < 0.6666667f)
                    {
                        hasColoredPupils = true;
                        Random.Shift();
                    }
                }
                else
                {
                    // Deep pupils
                    pupilSize = 0.7f;
                    deepPupils = true;
                }
            }

            Random.Shift(8); // tick 8 more rolls (hands head color * 3, legs size, arm thickness * 2, colored eartler tips, wide teeth)
            if (Random.Value >= 0.5f) Random.Shift(); // tail segments
            if (Random.Value < 0.25f) Random.Shift(); // unused scruffy calculation that's still done for some reason

            // OK NOW WE GET TO THE COLOR CRAP
            HSLColor bodyColor, headColor, decoColor, eyeColor, bellyColor;
            float bodyColorBlack, headColorBlack, bellyColorBlack;

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

            // Body color calculation
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

            // More magic number calculation
            float accentHue2;
            if (Random.Value < Custom.LerpMap(bodyColorBlack, 0.5f, 0.8f, 0.9f, 0.3f))
            {
                accentHue2 = accentHue1 + Mathf.Lerp(-1f, 1f, Random.Value) * 0.1f * Mathf.Pow(Random.Value, 1.5f);
                accentHue2 = Mathf.Lerp(accentHue2, 0.15f, Random.Value);
                if (accentHue2 > 1f)
                {
                    accentHue2 -= 1f;
                }
                else if (accentHue2 < 0f)
                {
                    accentHue2 += 1f;
                }
            }
            else
            {
                accentHue2 = ((Random.Value < 0.5f) ? Custom.Decimal(bodyHue + 0.5f) : Custom.Decimal(accentHue1 + 0.5f)) + Mathf.Lerp(-1f, 1f, Random.Value) * 0.25f * Mathf.Pow(Random.Value, 2f);
                if (Random.Value < Mathf.Lerp(0.8f, 0.2f, p.nrg)) // energy
                {
                    accentHue2 = Mathf.Lerp(accentHue2, 0.15f, Random.Value);
                }
                if (accentHue2 > 1f)
                {
                    accentHue2 -= 1f;
                }
                else if (accentHue2 < 0f)
                {
                    accentHue2 += 1f;
                }
            }

            // Head color time
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
            if (headColor.saturation < bodyColor.saturation * 0.75f)
            {
                if (Random.Value < 0.5f)
                {
                    headColor.hue = bodyColor.hue;
                }
                else
                {
                    headColor.lightness *= 0.25f;
                }
                headColor.saturation = bodyColor.saturation * 0.75f;
            }

            // Decoration colors (eartler tips)
            decoColor = new HSLColor((Random.Value < 0.65f) ? bodyHue : ((Random.Value < 0.5f) ? accentHue1 : accentHue2), Random.Value, 0.5f + 0.5f * Mathf.Pow(Random.Value, 0.5f));
            decoColor.lightness *= Mathf.Lerp(generalMelanin, Random.Value, 0.5f);

            // Eye colors
            eyeColor = new HSLColor(accentHue2, 1f, (Random.Value < 0.2f) ? (0.5f + Random.Value * 0.5f) : 0.5f);
            // this.eyeColor = new HSLColor(this.scavenger.Elite ? 0f : num3, 1f, (Random.Value < 0.2f) ? (0.5f + Random.Value * 0.5f) : 0.5f);
            if (hasColoredPupils)
            {
                eyeColor.lightness = Mathf.Lerp(eyeColor.lightness, 1f, 0.3f);
            }
            if (headColor.lightness * (1f - headColorBlack) > eyeColor.lightness / 2f && (pupilSize == 0f || deepPupils))
            {
                eyeColor.lightness *= 0.2f;
            }

            // Belly color
            /*float value = Random.Value;
            float value2 = Random.Value;
            bellyColor = new HSLColor(
                Mathf.Lerp(bodyColor.hue, decoColor.hue, value * 0.7f),
                bodyColor.saturation * Mathf.Lerp(1f, 0.5f, value),
                bodyColor.lightness + 0.05f + 0.3f * value2
            );
            bellyColorBlack = Mathf.Lerp(bodyColorBlack, 1f, 0.3f * Mathf.Pow(value2, 1.4f));
            if (Random.Value < 0.033333335f)
            {
                headColor.lightness = Mathf.Lerp(0.2f, 0.35f, Random.Value);
                headColorBlack *= Mathf.Lerp(1f, 0.8f, Random.Value);
                bellyColor.hue = Mathf.Lerp(bellyColor.hue, headColor.hue, Mathf.Pow(Random.Value, 0.5f));
            }*/

            float r = 0f;

            if (body.HueInput.enabled) r += Distance(bodyColor.hue, body.HueInput);
            if (body.SatInput.enabled) r += Distance(bodyColor.saturation, body.SatInput);
            if (body.LightInput.enabled) r += Distance(bodyColor.lightness, body.LightInput);

            if (head.HueInput.enabled) r += Distance(headColor.hue, head.HueInput);
            if (head.SatInput.enabled) r += Distance(headColor.saturation, head.SatInput);
            if (head.LightInput.enabled) r += Distance(headColor.lightness, head.LightInput);

            if (deco.HueInput.enabled) r += Distance(decoColor.hue, deco.HueInput);
            if (deco.SatInput.enabled) r += Distance(decoColor.saturation, deco.SatInput);
            if (deco.LightInput.enabled) r += Distance(decoColor.lightness, deco.LightInput);

            if (eye.HueInput.enabled) r += Distance(eyeColor.hue, eye.HueInput);
            if (eye.LightInput.enabled) r += Distance(eyeColor.lightness, eye.LightInput);

            return r;
        }
    }
}
