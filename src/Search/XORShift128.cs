using System.Runtime.CompilerServices;
using UnityEngine;

namespace FinderMod.Search
{
    /// <summary>
    /// Version of <see cref="UnityEngine.Random"/> able to be created individually, which is useful for multithreading purposes.
    /// </summary>
    public class XORShift128
    {
        /// <summary>State variable</summary>
        public uint x = 0, y = 0, z = 0, w = 0;
        const uint MT19937 = 0x6c078965u;
        const uint MANTISSA = 0x7FFFFFu;
        const float INV_MANTISSA = 1.192093E-07f;
        const float TWO_PI = 6.2831855f;
        const float ONE_THIRD = 1f / 3f;

        /// <summary>Initializes with a seed.</summary>
        /// <param name="seed">Seed to initialize with</param>
        public void InitState(int seed)
        {
            x = (uint)seed;
            y = (uint)(MT19937 * x + 1);
            z = (uint)(MT19937 * y + 1);
            w = (uint)(MT19937 * z + 1);
        }

        /// <summary>Initializes with a seed.</summary>
        /// <param name="seed">Seed to initialize with</param>
        public void InitState(uint seed)
        {
            x = seed;
            y = MT19937 * x + 1;
            z = MT19937 * y + 1;
            w = MT19937 * z + 1;
        }

        /// <summary>Initializes with a state</summary>
        /// <param name="x"><see cref="x"/></param>
        /// <param name="y"><see cref="y"/></param>
        /// <param name="z"><see cref="z"/></param>
        /// <param name="w"><see cref="w"/></param>
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

        /// <summary>Advances the random state without returning a number.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Shift() => NextU32();

        /// <summary>Advances the random state n times without returning a number.</summary>
        /// <param name="n">The amount of times to shift</param>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Shift(int n)
        {
            for (int i = 0; i < n; i++) NextU32();
        }

        /// <summary>Equivalent to <see cref="Random.Range(int, int)"/></summary>
        /// <param name="min">Minimum, inclusive.</param>
        /// <param name="max">Maximum, exclusive.</param>
        /// <returns>Random value between min (inclusive) and max (exclusive).</returns>
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

        /// <summary>Equivalent to <see cref="Random.value"/>. Returns a random value between 0 and 1, inclusive.</summary>
        public float Value => (NextU32() & MANTISSA) * INV_MANTISSA;

        /// <summary>Equivalent to <see cref="Random.Range(float, float)"/></summary>
        /// <param name="min">Minimum, inclusive.</param>
        /// <param name="max">Maximum, inclusive.</param>
        /// <returns>Random value between min and max, inclusive.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Range(float min, float max)
        {
            // This is accurate to how the original code does it, though is often off by an insignificant amount (usually 1 or 2 bits on the very low end).
            // Why? I can't exactly determine but it appears to be out of my control so ¯\_(ツ)_/¯
            float f = (NextU32() & MANTISSA) * INV_MANTISSA;
            return ((1.0f - f) * max) + (f * min);
        }

        /// <summary>Equivalent to <see cref="Random.rotation"/></summary>
        /// <remarks>This cannot be substituted with calls of <see cref="Shift()"/>.</remarks>
        /// <returns>A random <see cref="Quaternion"/>. This is not uniform.</returns>
        public Quaternion Rotation()
        {
            uint u3 = w;
            uint u4 = x ^ (x << 11);
            u4 = ((u3 >> 11) ^ u4) >> 8 ^ u3 ^ u4;

            uint u1 = y ^ (y << 11);
            u1 = ((u4 >> 11) ^ u1) >> 8 ^ u4 ^ u1;
            float qx = 1f - (u4 & MANTISSA) * INV_MANTISSA * 2f;

            uint u2 = z ^ (z << 11);
            u2 = ((u1 >> 11) ^ u2) >> 8 ^ u1 ^ u2;
            float qy = 1f - (u1 & MANTISSA) * INV_MANTISSA * 2f;

            u3 ^= (u3 << 11);
            x = u4;
            y = u1;
            z = u2;
            float qz = 1f - (u2 & MANTISSA) * INV_MANTISSA * 2f;

            u3 = ((u2 >> 11) ^ u3) >> 8 ^ u2 ^ u3;
            w = u3;
            float qw = 1f - (u3 & MANTISSA) * INV_MANTISSA * 2f;

            float magnitude = Mathf.Sqrt(qx * qx + qy * qy + qz * qz + qw * qw);
            if (magnitude > 1e-05f)
            {
                qx /= magnitude;
                qy /= magnitude;
                qz /= magnitude;
                qw /= magnitude;
            }
            else
            {
                qx = 0f;
                qw = 1f;
                qz = 0f;
                qy = 0f;
            }

            if (qw < 0f)
            {
                qx = -qx;
                qy = -qy;
                qz = -qz;
                qw = -qw;
            }

            return new Quaternion(qx, qy, qz, qw);
        }

        /// <summary>Equivalent to <see cref="Random.rotationUniform"/></summary>
        /// <remarks>This cannot be substituted with calls of <see cref="Shift()"/>.</remarks>
        /// <returns>A random <see cref="Quaternion"/>, uniformly.</returns>
        public Quaternion RotationUniform()
        {
            uint u1 = w;
            uint u2 = x ^ (x << 11);
            x = u1;
            u2 = ((u1 >> 11) ^ u2) >> 8 ^ u1 ^ u2;
            u1 = y ^ (y << 11);
            y = u2;
            u1 = ((u2 >> 11) ^ u1) >> 8 ^ u2 ^ u1;

            float magnitude = 1f - (u2 & MANTISSA) * INV_MANTISSA;

            u2 = z ^ (z << 11);
            z = u1;
            u2 = ((u1 >> 11) ^ u2) >> 8 ^ u1 ^ u2;
            w = u2;

            float f1 = (1f - (u1 & MANTISSA) * INV_MANTISSA) * TWO_PI;
            float f2 = (1f - (u2 & MANTISSA) * INV_MANTISSA) * TWO_PI;

            float d1 = Mathf.Sqrt(1f - magnitude);
            float d2 = Mathf.Sqrt(magnitude);

            float qx = Mathf.Sin(f1) * d1;
            float qy = Mathf.Cos(f1) * d1;
            float qz = Mathf.Sin(f2) * d2;
            float qw = Mathf.Cos(f2) * d2;

            if (qw < 0)
            {
                qx = -qx;
                qy = -qy;
                qz = -qz;
                qw = -qw;
            }

            return new Quaternion(qx, qy, qz, qw);
        }

        /// <summary>Equivalent to <see cref="Random.onUnitSphere"/></summary>
        /// <remarks>This can be substituted with 2 calls of <see cref="Shift()"/>.</remarks>
        /// <returns>A random <see cref="Vector3"/> with a magnitude of 1</returns>
        public Vector3 OnUnitSphere()
        {
            uint u1 = NextU32();
            uint u2 = NextU32();

            float cz = 1f - (u1 & MANTISSA) * INV_MANTISSA * 2f;
            float radius = Mathf.Sqrt(1f - cz * cz);

            float angle = (1f - (u2 & MANTISSA) * INV_MANTISSA) * TWO_PI;

            float cx = Mathf.Cos(angle) * radius;
            float cy = Mathf.Sin(angle) * radius;

            return new Vector3(cx, cy, cz);
        }

        /// <summary>Equivalent to <see cref="Random.insideUnitSphere"/></summary>
        /// <remarks>This can be substituted with 3 calls of <see cref="Shift()"/>.</remarks>
        /// <returns>A random <see cref="Vector3"/> with a magnitude no greater than 1</returns>
        public Vector3 InsideUnitSphere()
        {
            Vector3 sphere = OnUnitSphere();
            float radius = Mathf.Pow((NextU32() & MANTISSA) * INV_MANTISSA, ONE_THIRD); // cbrt
            return sphere * radius;
        }

        /// <summary>Equivalent to <see cref="Random.insideUnitCircle"/></summary>
        /// <remarks>This can be substituted with 2 calls of <see cref="Shift()"/>.</remarks>
        /// <returns>A random <see cref="Vector2"/> with a magnitude no greater than 1</returns>
        public Vector2 InsideUnitCircle()
        {
            uint u1 = NextU32();
            uint u2 = NextU32();

            float angle = (1f - (u1 & MANTISSA) * INV_MANTISSA) * TWO_PI;
            float radius = Mathf.Sqrt(1f - (u2 & MANTISSA) * INV_MANTISSA);

            return new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        }
    }
}
