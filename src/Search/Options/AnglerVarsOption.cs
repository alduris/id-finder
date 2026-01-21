using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class AnglerVarsOption : Option
    {
        private readonly IntInput NumOfPropellersInput;

        private readonly IntInput BottomTeethInput;
        private readonly IntInput TopTeethInput;

        private readonly FloatInput EyeSizeInput;
        private readonly HueInput EyeHueInput;

        private readonly FloatInput RudderLengthInput;
        private readonly FloatInput ConeRadInput;

        private readonly FloatInput ArmLowLengthInput;
        private readonly FloatInput ArmHighLengthInput;
        private readonly FloatInput ArmLowExtraInput;

        public AnglerVarsOption()
        {
            elements = [
                NumOfPropellersInput = new IntInput("Number of propellers", 3, 6),
                new Whitespace(),
                BottomTeethInput = new IntInput("Number of bottom teeth", 4, 13),
                TopTeethInput = new IntInput("Number of top teeth", 4, 13),
                new Whitespace(),
                EyeSizeInput = new FloatInput("Eye size", 0.5f, 1.15f),
                EyeHueInput = new HueInput("Eye hue", 0.027f, 0.027f + 0.153f),
                new Whitespace(),
                RudderLengthInput = new FloatInput("Rudder length"),
                ConeRadInput = new FloatInput("Cone radius", 15f, 24f),
                new Whitespace(),
                ArmLowLengthInput = new FloatInput("Arm lower length", 25f, 60f),
                ArmHighLengthInput = new FloatInput("Arm higher length", 25f, 70f),
                ArmLowExtraInput = new FloatInput("Arm lower extra length", 5f, 25f)
                ];
        }

        public override float Execute(XORShift128 Random)
        {
            var vars = new AnglerVars(Random);
            return DistanceIf(vars.numProps, NumOfPropellersInput)
                + DistanceIf(vars.bottomTeeth, BottomTeethInput)
                + DistanceIf(vars.topTeeth, TopTeethInput)
                + DistanceIf(vars.eyeSize, EyeSizeInput)
                + DistanceIf(vars.eyeHue, EyeHueInput)
                + DistanceIf(vars.rudderLength, RudderLengthInput)
                + DistanceIf(vars.coneRad, ConeRadInput)
                + DistanceIf(vars.armLowLength, ArmLowLengthInput)
                + DistanceIf(vars.armHighLength, ArmHighLengthInput)
                + DistanceIf(vars.armLowExtra, ArmLowExtraInput);
        }

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            var vars = new AnglerVars(Random);
            yield return $"Number of propellers: {vars.numProps}";
            yield return null!;
            yield return $"Number of bottom teeth: {vars.bottomTeeth}";
            yield return $"Number of top teeth: {vars.topTeeth}";
            yield return null!;
            yield return $"Eye size: {vars.eyeSize}";
            yield return $"Eye hue: {vars.eyeHue}";
            yield return null!;
            yield return $"Rudder length: {vars.rudderLength}";
            yield return $"Cone radius: {vars.coneRad}";
            yield return null!;
            yield return $"Arm lower length: {vars.armLowLength}";
            yield return $"Arm higher length: {vars.armHighLength}";
            yield return $"Arm lower extra length: {vars.armLowExtra}";
        }

        private struct AnglerVars
        {
            public int numProps;
            public int bottomTeeth;
            public int topTeeth;
            public float eyeSize;
            public float eyeHue;
            public float rudderLength;
            public float coneRad;
            public float armLowLength;
            public float armHighLength;
            public float armLowExtra;

            public AnglerVars(XORShift128 Random)
            {
                numProps = Random.Range(3, 7);

                int baseTeethCount = Random.Range(7, 14);
                bottomTeeth = baseTeethCount - Random.Range(0, 4);
                topTeeth = baseTeethCount - Random.Range(0, 4);

                Random.Shift(4 * (bottomTeeth + topTeeth));

                eyeSize = 0.5f + Random.Value * 0.65f;
                eyeHue = 0.027f + 0.153f * Random.Value;
                rudderLength = Random.Value;
                coneRad = Random.Range(15f, 24f);
                armLowLength = Random.Range(25f, 60f);
                armHighLength = Random.Range(25f, 70f);
                armLowExtra = Random.Range(5f, 25f);

                // sonarSlowLoopLength = Random.Range(100, 140);
                // sonarFastLoopLength = Random.Range(16, 24);
                // sonarPitchVariation = Random.Range(0.9f, 1.1f);
            }
        }
    }
}
