using System.Collections.Generic;
using FinderMod.Inputs;
using UnityEngine;

namespace FinderMod.Search.Options
{
    internal class CoalescipedeOption : Option, ICanGPU
    {
        private readonly FloatInput SizeInput = new("Size");
        public CoalescipedeOption()
        {
            RepresentedCreature = CreatureTemplate.Type.Spider;
            elements = [SizeInput];
        }

        public ComputeShader Shader => InternalShaders.coalescipedeSizeShader;

        public override float Execute(XORShift128 Random)
        {
            return DistanceIf(Random.Value, SizeInput);
        }

        public ICanGPU.GPUInput[] GetGPUInputs() => [SizeInput.AsGPUInput()];

        protected override IEnumerable<string> GetValues(XORShift128 Random)
        {
            yield return $"Size: {Random.Value}";
        }
    }
}
