using FinderMod.Inputs;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class NootAdultVarsOption : Option
    {
        private readonly FloatInput wsInp, lsInp, fatInp, slInp;
        private readonly ColorRGBInput bodyInp, eyeInp;

        public NootAdultVarsOption() : base("Noodlefly Adult Variations")
        {
            elements = [
                wsInp = new FloatInput("Wing size", 0.8f, 1.2f),
                lsInp = new FloatInput("Leg size", 0.6f, 1.4f),
                fatInp = new FloatInput("Fatness"),
                slInp = new FloatInput("Snout length"),
                bodyInp = new ColorRGBInput("Body color"),
                eyeInp = new ColorRGBInput("Eye color") { description = "Tied to body color, may not give expected results" }
            ];
        }

        public override float Execute(XORShift128 Random)
        {
            float wingsSize, legsFac, fatness, snoutLength, hue, hueDiv, lightness;

            wingsSize = Mathf.Lerp(0.8f, 1.2f, ClampedRandomVariation(0.5f, 0.5f, 0.4f, Random));
            legsFac = Mathf.Lerp(0.6f, 1.4f, Random.Value);
            fatness = ClampedRandomVariation(0.5f, 0.5f, 0.4f, Random);
            snoutLength = Mathf.Lerp(0.5f, 1.5f, Random.Value);
            hue = WrappedRandomVariation(0.5f, 0.08f, 0.2f, Random);
            Random.Shift(2);
            bool cosBools2 = (Random.Value < 0.5f);
            Random.Shift();
            lightness = 0.4f;
            if (Random.Value < 0.33333334f)
            {
                lightness = 0.4f * Mathf.Pow(Random.Value, 5f);
            }
            else if (Random.Value < 0.05882353f)
            {
                lightness = 1f - 0.6f * Mathf.Pow(Random.Value, 5f);
            }
            hueDiv = Mathf.Lerp(-1f, 1f, Random.Value) * Custom.LerpMap(Mathf.Abs(0.5f - hue), 0f, 0.08f, 0.06f, 0.3f);
            if (lightness < 0.4f && Random.Value > lightness && Random.Value < 0.1f)
            {
                hue = Random.Value;
            }

            // Calculate true colors
            Color bodyColor, eyeColor; // there is also a highlight color and detail color I think affects legs and wings
            hue += 0.478f;
            bodyColor = Custom.HSL2RGB(hue, Custom.LerpMap(lightness, 0.5f, 1f, 0.9f, 0.5f), Mathf.Lerp(0.1f, 0.8f, Mathf.Pow(lightness, 2f)));

            hue += Mathf.InverseLerp(0.5f, 0.6f, lightness) * 0.5f;
            if (cosBools2)
            {
                eyeColor = Custom.HSL2RGB(hue + 0.5f - hueDiv, 1f, Mathf.Lerp(0.7f, 0.3f, Mathf.Pow(lightness, 1.5f)));
            }
            else
            {
                eyeColor = Custom.HSL2RGB(hue + 0.5f, 1f, Mathf.Lerp(0.7f, 0.3f, Mathf.Pow(lightness, 1.5f)));
            }

            // Return results
            float r = 0f;
            r += DistanceIf(wingsSize, wsInp);
            r += DistanceIf(legsFac, lsInp);
            r += DistanceIf(fatness, fatInp);
            r += DistanceIf(snoutLength, slInp);
            r += DistanceIf(bodyColor, bodyInp);
            r += DistanceIf(eyeColor, eyeInp);

            return r;
        }
    }
}
