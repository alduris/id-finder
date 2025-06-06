﻿using System.Runtime.CompilerServices;

namespace FinderMod.Search
{
    public class XORShift128
    {
        public uint x = 0, y = 0, z = 0, w = 0;
        const uint MT19937 = 0x6c078965u;
        public void InitState(int seed)
        {
            x = (uint)seed;
            y = (uint)(MT19937 * x + 1);
            z = (uint)(MT19937 * y + 1);
            w = (uint)(MT19937 * z + 1);
        }

        public void InitState(uint seed)
        {
            x = seed;
            y = MT19937 * x + 1;
            z = MT19937 * y + 1;
            w = MT19937 * z + 1;
        }

        public void InitState(uint x, uint y, uint z, uint w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint NextU32()
        {
            uint t = x ^ (x << 11);
            x = y; y = z; z = w;
            return w = w ^ (w >> 19) ^ t ^ (t >> 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Shift() => NextU32();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Shift(int n)
        {
            for (int i = 0; i < n; i++) NextU32();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Range(int min, int max)
        {
            if (max - min == 0) return min;

            unchecked
            {
                uint minLong = (uint)min;
                uint maxLong = (uint)max;
                uint r = NextU32();

                if (max < min)
                    return (int)(minLong - r % (maxLong - minLong));
                else
                    return (int)(minLong + r % (maxLong - minLong));
            }
        }

        public float Value => (NextU32() & 0x7FFFFF) * 1.192093E-07f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Range(float min, float max)
        {
            // This is accurate to how the original code does it, though is often off by an insignificant amount (usually 1 or 2 bits on the very low end).
            // Why? I can't exactly determine but it appears to be out of my control so ¯\_(ツ)_/¯
            float f = (NextU32() & 0x7FFFFFu) * 1.192093E-07f;
            return (((1.0f - f) * max) + (f * min));
        }
    }
}
