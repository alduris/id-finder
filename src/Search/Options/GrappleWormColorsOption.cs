using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class GrappleWormColorsOption : Option
    {
        private readonly ColorHSLInput ColorInput = new("Color", 0.52f, 0.68f, 0.4f, 0.9f, 0.15f, 0.3f);

        public GrappleWormColorsOption()
        {
            elements = [ColorInput];
        }

        private HSLColor Color(XORShift128 Random) => new(Mathf.Lerp(0.52f, 0.68f, Random.Value), Mathf.Lerp(0.4f, 0.9f, Random.Value), Mathf.Lerp(0.15f, 0.3f, Random.Value));

        public override float Execute(XORShift128 Random)
        {
            return DistanceIf(Color(Random), ColorInput);
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var color = Color(Random);
            yield return $"Color: hsl({color.hue}, {color.saturation}, {color.lightness})";
        }
    }
}
