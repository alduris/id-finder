using System.Runtime.CompilerServices;

namespace FinderMod.Search
{
    public class XORShift128
    {
        public uint x = 0, y = 0, z = 0, w = 0;
        const uint MT19937 = 1812433253;
        public void InitSeed(int seed)
        {
            x = (uint)seed;
            y = (uint)(MT19937 * x + 1);
            z = (uint)(MT19937 * y + 1);
            w = (uint)(MT19937 * z + 1);
        }

        public void InitState(uint x, uint y, uint z, uint w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint _XORShift()
        {
            uint t = x ^ (x << 11);
            x = y; y = z; z = w;
            return w = w ^ (w >> 19) ^ t ^ (t >> 8);
        }
        public uint XORShift() => _XORShift();

        public int Range(int min, int max)
        {
            if (max - min == 0) return min;

            long minLong = (long)min;
            long maxLong = (long)max;
            long r = _XORShift();

            if (max < min)
                return (int)(minLong - r % (maxLong - minLong));
            else
                return (int)(minLong + r % (maxLong - minLong));
        }

        public float Value => 1.0f - Range(0.0f, 1.0f);

        public float Range(float min, float max)
        {
            return (min - max) * ((float)(_XORShift() << 9) / 0xFFFFFFFF) + max;
        }
    }
}
