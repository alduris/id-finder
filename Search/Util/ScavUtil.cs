using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search.Util
{
    internal class ScavUtil
    {
        public enum ScavBackType
        {
            HardBackSpikes = 0,
            WobblyBackTufts = 1
        }

        public enum ScavBodyScalePattern
        {
            SpineRidge = 0,
            DoubleSpineRidge = 1,
            RandomBackBlotch = 2
        }

        // Max values needed: 46
        public static void GeneratePattern(XORShift128 Random, ScavBackType type, ScavBodyScalePattern pattern, out float top, out float bottom, out int numScales)
        {
            switch (pattern)
            {
                case ScavBodyScalePattern.SpineRidge:
                    {
                        top = Mathf.Lerp(0.07f, 0.3f, Random.Value);
                        bottom = Mathf.Lerp(0.6f, 1f, Random.Value);
                        float num = Mathf.Lerp(2.5f, 12f, Random.Value);
                        numScales = (int)((bottom - top) * 100f / num);
                        break;
                    }
                case ScavBodyScalePattern.DoubleSpineRidge:
                    {
                        top = Mathf.Lerp(0.07f, 0.3f, Random.Value);
                        bottom = Mathf.Lerp(0.6f, 1f, Random.Value);
                        if (type == ScavBackType.WobblyBackTufts)
                        {
                            bottom = Mathf.Lerp(bottom, 0.5f, Random.Value);
                        }
                        float num3 = Mathf.Lerp(4.5f, 12f, Random.Value);
                        numScales = (int)((bottom - top) * 100f / num3) * 2;
                        break;
                    }
                case ScavBodyScalePattern.RandomBackBlotch:
                    {
                        //                                              v  scruffy is hardcoded to 1f :(
                        numScales = (int)Mathf.Lerp(Mathf.Lerp(20f, 4f, 1f), 40f, Mathf.Lerp(Random.Value, Random.Value, 0.5f * Random.Value));
                        Random.Shift(numScales);
                        top = Mathf.Lerp(0.02f, 0.2f, Random.Value);
                        bottom = Mathf.Lerp(0.4f, 0.9f, Mathf.Pow(Random.Value, 1.5f));
                        break;
                    }
                default: // this should never be reached unless you pass in null for pattern
                    {
                        throw new NotImplementedException();
                    }
            }
        }
    }
}
