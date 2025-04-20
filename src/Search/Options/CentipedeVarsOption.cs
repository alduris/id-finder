using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public class CentipedeVarsOption : Option
    {
        public enum CentipedeType
        {
            Normal,
            Red,
            Centiwing,
            Aquapede
        }

        private readonly CentipedeType type;
        
        private readonly HueInput HueInput = null!;
        private readonly ColorHSLInput ColorInput = null!;
        private readonly FloatInput SizeInput = null!;

        public CentipedeVarsOption(CentipedeType type)
        {
            this.type = type;
            switch (type)
            {
                default:
                    HueInput = new HueInput("Hue", 0.04f, 0.1f);
                    SizeInput = new FloatInput("Default size", 0f, 1f);
                    break;
                case CentipedeType.Red:
                    ColorInput = new ColorHSLInput("Color", true, -0.02f, 0.01f, true, 0.9f, 1f, false, 0.5f, 0.5f);
                    break;
                case CentipedeType.Centiwing:
                    HueInput = new HueInput("Hue", 0.28f, 0.38f);
                    SizeInput = new FloatInput("Default size", 0.5f, 0.65f);
                    break;
                case CentipedeType.Aquapede:
                    ColorInput = new ColorHSLInput("Color", true, 0.5f, 0.6f, true, 0.8f, 1f, false, 0.5f, 0.5f);
                    SizeInput = new FloatInput("Default size", 0.9f, 1f);
                    break;
            }
            elements = [];
            if (HueInput != null) elements.Add(HueInput);
            if (ColorInput != null) elements.Add(ColorInput);
            if (SizeInput != null) elements.Add(SizeInput);
        }

        private (float hue, float saturation, float size) GetResults(XORShift128 Random)
        {
            // Centipede.GenerateSize()
            float size = Mathf.Lerp(0f, 1f, Mathf.Pow(Random.Value, 1.5f));
            var (x, y, z, w) = (Random.x, Random.y, Random.z, Random.w);
            if (type == CentipedeType.Centiwing)
            {
                size = Mathf.Lerp(0.5f, 0.65f, Random.Value);
            }
            else if (type == CentipedeType.Red)
            {
                size = 1f;
            }
            else if (type == CentipedeType.Aquapede)
            {
                size = Mathf.Min(1f, Mathf.Lerp(0.9f, 1.8f, Random.Value));
            }

            // CentipedeGraphics..ctor()
            Random.InitState(x, y, z, w);
            float hue, saturation;
            if (type == CentipedeType.Centiwing)
            {
                hue = Mathf.Lerp(0.28f, 0.38f, Random.Value);
                saturation = 0.5f;
            }
            else if (type == CentipedeType.Aquapede)
            {
                hue = Mathf.Lerp(0.5f, 0.6f, Random.Value);
                saturation = Mathf.Lerp(0.8f, 1f, Random.Value);
            }
            else if (type == CentipedeType.Red)
            {
                hue = Mathf.Lerp(-0.02f, 0.1f, Random.Value);
                saturation = Mathf.Lerp(0.9f, 1f, Random.Value);
            }
            else
            {
                hue = Mathf.Lerp(0.04f, 0.1f, Random.Value);
                saturation = 0.9f;
            }
            
            return (hue, saturation, size);
        }

        public override float Execute(XORShift128 Random)
        {
            var (hue, saturation, size) = GetResults(Random);
            return DistanceIf(hue, HueInput)
                + DistanceIf(new HSLColor(hue, saturation, 0.5f), ColorInput)
                + DistanceIf(size, SizeInput);
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var (hue, saturation, size) = GetResults(Random);
            yield return $"Hue: {hue}";
            yield return $"Saturation: {saturation}";
            yield return $"Size: {size}";
        }
    }
}
