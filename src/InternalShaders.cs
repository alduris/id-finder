using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable disable
namespace FinderMod
{
    internal static class InternalShaders
    {
        public static ComputeShader personalityShader;
        public static void LoadShaders()
        {
            var bundle = AssetBundle.LoadFromFile(AssetManager.ResolveFilePath("shaders/aldurisentities", false));
            personalityShader = bundle.LoadAsset<ComputeShader>("Assets/IDFinder/Personality.compute");
        }
    }
}
#nullable enable
