using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWCustom;
using UnityEngine;

namespace FinderMod.Search
{
    internal class ScavUtil
    {
        public enum ScavBackType
        {
            HardBackSpikes,
            WobblyBackTufts
        }

        public enum ScavBodyScalePattern
        {
            SpineRidge = 1,
            DoubleSpineRidge = 2,
            RandomBackBlotch = 3
        }

        // Max values needed: 46
        public static void GeneratePattern(float[] vals, ref int picker, ScavBackType type, ScavBodyScalePattern pattern, out float top, out float bottom, out int numScales)
        {
            switch(pattern)
            {
                case ScavBodyScalePattern.SpineRidge:
                    {
                        top = Mathf.Lerp(0.07f, 0.3f, vals[picker++]);
                        bottom = Mathf.Lerp(0.6f, 1f, vals[picker++]);
                        float num = Mathf.Lerp(2.5f, 12f, vals[picker++]);
                        numScales = (int)((bottom - top) * 100f / num);
                        break;
                    }
                case ScavBodyScalePattern.DoubleSpineRidge :
                    {
                        top = Mathf.Lerp(0.07f, 0.3f, vals[picker++]);
                        bottom = Mathf.Lerp(0.6f, 1f, vals[picker++]);
                        if (type == ScavBackType.WobblyBackTufts)
                        {
                            bottom = Mathf.Lerp(bottom, 0.5f, vals[picker++]);
                        }
                        float num3 = Mathf.Lerp(4.5f, 12f, vals[picker++]);
                        numScales = (int)((bottom - top) * 100f / num3) * 2;
                        break;
                    }
                case ScavBodyScalePattern.RandomBackBlotch: 
                    {
                        float value = vals[picker++];
                        numScales = (int)Mathf.Lerp(Mathf.Lerp(20f, 4f, 1f), 40f, Mathf.Lerp(value, vals[picker++], 0.5f * vals[picker++]));
                        //                                              ^ scruffy is hardcoded to 1f :(
                        picker += numScales;
                        top = Mathf.Lerp(0.02f, 0.2f, vals[picker++]);
                        bottom = Mathf.Lerp(0.4f, 0.9f, Mathf.Pow(vals[picker++], 1.5f));
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
