﻿using System.Collections.Generic;
using FinderMod.Inputs;
using RWCustom;
using Unity.Burst;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class EliteScavColorsOption : Option
    {
        private readonly ColorHSLInput body, head, deco;

        public EliteScavColorsOption() : base()
        {
            elements = [
                body = new ColorHSLInput("Body color"),
                head = new ColorHSLInput("Head color"),
                deco = new ColorHSLInput("Decoration color") { description = "Controls eartler tip colors if enabled. Can sometimes control eye or belly color." }
            ];
        }

        [BurstCompile]
        private (HSLColor, HSLColor, HSLColor) GetColors(XORShift128 Random)
        {
            Personality p = new(Random);

            // Pre-generate some attributes we will need later
            float generalMelanin = Custom.PushFromHalf(Random.Value, 2f);
            float headSize = ClampedRandomVariation(0.5f, 0.5f, 0.1f, Random);
            Random.Shift();
            float eyeSize = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(Random.Value, Mathf.Pow(headSize, 0.5f), Random.Value * 0.4f)), Mathf.Lerp(0.95f, 0.55f, p.sym));
            if (Random.Value >= Mathf.Lerp(0.3f, 0.7f, p.sym)) Random.Shift(); // narrow eyes gets overridden in the next line but the calculation still occurs
            float narrowEyes = 1f;

            // Calculate how far we have to advance the pointer
            Random.Shift(9); // next 9 rolls (eyes angle, fatness * 3, narrow waist * 3, neck thickness * 2)

            if (Random.Value < 0.65f && eyeSize > 0.4f && narrowEyes < 0.3f) // check to add pupils
            {
                if (Random.Value < Mathf.Pow(p.sym, 1.5f) * 0.8f) // determine if sympathetic enough to have colored pupils
                {
                    // Colored pupils
                    Random.Shift();
                    if (Random.Value < 0.6666667f)
                    {
                        // hasColoredPupils = true;
                        Random.Shift();
                    }
                }
                // else deep pupils
            }

            Random.Shift(8); // tick 8 more rolls (colored pupils, hands head color * 3, legs size, arm thickness * 2, wide teeth)
            if (Random.Value >= 0.5f) Random.Shift(); // tail segments
            if (Random.Value < 0.25f) Random.Shift(); // unused scruffy calculation that's still done for some reason

            // OK NOW WE GET TO THE COLOR CRAP
            HSLColor bodyColor, headColor, decoColor;
            float bodyColorBlack, headColorBlack;

            Random.Shift();
            if (Random.Value < 0.025f) Random.Shift();
            float bodyHue = Mathf.Pow(Random.Value, 5f);
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

            var bodyColorRGB = bodyColor.rgb;
            bodyColorBlack = Custom.LerpMap((bodyColorRGB.r + bodyColorRGB.g + bodyColorRGB.b) / 3f, 0.04f, 0.8f, 0.3f, 0.95f, 0.5f);
            bodyColorBlack = Mathf.Lerp(bodyColorBlack, Mathf.Lerp(0.5f, 1f, Random.Value), Random.Value * Random.Value * Random.Value);
            bodyColorBlack *= generalMelanin;
            Vector2 vector = new(bodyColor.saturation, Mathf.Lerp(-1f, 1f, bodyColor.lightness * (1f - bodyColorBlack)));
            if (vector.magnitude < 0.5f)
            {
                vector = Vector2.Lerp(vector, vector.normalized, Mathf.InverseLerp(0.5f, 0.3f, vector.magnitude));
                bodyColor = new HSLColor(bodyColor.hue, Mathf.InverseLerp(-1f, 1f, vector.x), Mathf.InverseLerp(-1f, 1f, vector.y));

                bodyColorRGB = bodyColor.rgb;
                bodyColorBlack = Custom.LerpMap((bodyColorRGB.r + bodyColorRGB.g + bodyColorRGB.b) / 3f, 0.04f, 0.8f, 0.3f, 0.95f, 0.5f);
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

            return (headColor, bodyColor, decoColor);
        }

        public override float Execute(XORShift128 Random)
        {
            var (headColor, bodyColor, decoColor) = GetColors(Random);

            float r = 0f;

            r += WrapDistanceIf(bodyColor.hue, body.HueInput);
            r += DistanceIf(bodyColor.saturation, body.SatInput);
            r += DistanceIf(bodyColor.lightness, body.LightInput);

            r += WrapDistanceIf(headColor.hue, head.HueInput);
            r += DistanceIf(headColor.saturation, head.SatInput);
            r += DistanceIf(headColor.lightness, head.LightInput);

            r += WrapDistanceIf(decoColor.hue, deco.HueInput);
            r += DistanceIf(decoColor.saturation, deco.SatInput);
            r += DistanceIf(decoColor.lightness, deco.LightInput);

            return r;
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var (headColor, bodyColor, decoColor) = GetColors(Random);

            yield return $"Head color: hsl({headColor.hue}, {headColor.saturation}, {headColor.lightness})";
            yield return $"Body color: hsl({bodyColor.hue}, {bodyColor.saturation}, {bodyColor.lightness})";
            yield return $"Deco color: hsl({decoColor.hue}, {decoColor.saturation}, {decoColor.lightness})";
            yield break;
        }
    }
}
