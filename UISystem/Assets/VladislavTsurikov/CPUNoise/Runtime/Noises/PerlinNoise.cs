using UnityEngine;

namespace VladislavTsurikov.CPUNoise.Runtime
{
    public class PerlinNoiseCPU : NoiseCPU
    {
        public PerlinNoiseCPU(int seed, float frequency, float amplitude = 1.0f)
        {
            Frequency = frequency;
            Amplitude = amplitude;
            Offset = Vector3.zero;

            Perm = new PermutationTable(1024, 255, seed);
        }

        private PermutationTable Perm { get; }

        public override void UpdateSeed(int seed) => Perm.Build(seed);

        /// <summary>
        ///     Sample the noise in 1 dimension.
        /// </summary>
        public override float Sample1D(float x)
        {
            x = (x + Offset.x) * Frequency;

            var ix0 = (int)Mathf.Floor(x); // Integer part of x
            var fx0 = x - ix0; // Fractional part of x
            var fx1 = fx0 - 1.0f;

            var s = Fade(fx0);

            var n0 = Grad(Perm[ix0], fx0);
            var n1 = Grad(Perm[ix0 + 1], fx1);

            return 0.25f * Lerp(s, n0, n1) * Amplitude;
        }

        /// <summary>
        ///     Sample the noise in 2 dimensions.
        /// </summary>
        public override float Sample2D(float x, float y)
        {
            x = (x + Offset.x) * Frequency;
            y = (y + Offset.y) * Frequency;

            var ix0 = (int)Mathf.Floor(x); // Integer part of x
            var iy0 = (int)Mathf.Floor(y); // Integer part of y

            var fx0 = x - ix0; // Fractional part of x
            var fy0 = y - iy0; // Fractional part of y
            var fx1 = fx0 - 1.0f;
            var fy1 = fy0 - 1.0f;

            var t = Fade(fy0);
            var s = Fade(fx0);

            var nx0 = Grad(Perm[ix0, iy0], fx0, fy0);
            var nx1 = Grad(Perm[ix0, iy0 + 1], fx0, fy1);

            var n0 = Lerp(t, nx0, nx1);

            nx0 = Grad(Perm[ix0 + 1, iy0], fx1, fy0);
            nx1 = Grad(Perm[ix0 + 1, iy0 + 1], fx1, fy1);

            var n1 = Lerp(t, nx0, nx1);

            return 0.66666f * Lerp(s, n0, n1) * Amplitude;
        }

        /// <summary>
        ///     Sample the noise in 3 dimensions.
        /// </summary>
        public override float Sample3D(float x, float y, float z)
        {
            x = (x + Offset.x) * Frequency;
            y = (y + Offset.y) * Frequency;
            z = (z + Offset.z) * Frequency;

            var ix0 = (int)Mathf.Floor(x); // Integer part of x
            var iy0 = (int)Mathf.Floor(y); // Integer part of y
            var iz0 = (int)Mathf.Floor(z); // Integer part of z
            var fx0 = x - ix0; // Fractional part of x
            var fy0 = y - iy0; // Fractional part of y
            var fz0 = z - iz0; // Fractional part of z
            var fx1 = fx0 - 1.0f;
            var fy1 = fy0 - 1.0f;
            var fz1 = fz0 - 1.0f;

            var r = Fade(fz0);
            var t = Fade(fy0);
            var s = Fade(fx0);

            var nxy0 = Grad(Perm[ix0, iy0, iz0], fx0, fy0, fz0);
            var nxy1 = Grad(Perm[ix0, iy0, iz0 + 1], fx0, fy0, fz1);
            var nx0 = Lerp(r, nxy0, nxy1);

            nxy0 = Grad(Perm[ix0, iy0 + 1, iz0], fx0, fy1, fz0);
            nxy1 = Grad(Perm[ix0, iy0 + 1, iz0 + 1], fx0, fy1, fz1);
            var nx1 = Lerp(r, nxy0, nxy1);

            var n0 = Lerp(t, nx0, nx1);

            nxy0 = Grad(Perm[ix0 + 1, iy0, iz0], fx1, fy0, fz0);
            nxy1 = Grad(Perm[ix0 + 1, iy0, iz0 + 1], fx1, fy0, fz1);
            nx0 = Lerp(r, nxy0, nxy1);

            nxy0 = Grad(Perm[ix0 + 1, iy0 + 1, iz0], fx1, fy1, fz0);
            nxy1 = Grad(Perm[ix0 + 1, iy0 + 1, iz0 + 1], fx1, fy1, fz1);
            nx1 = Lerp(r, nxy0, nxy1);

            var n1 = Lerp(t, nx0, nx1);

            return 1.1111f * Lerp(s, n0, n1) * Amplitude;
        }

        private float Fade(float t) => t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f);

        private float Lerp(float t, float a, float b) => a + t * (b - a);

        private float Grad(int hash, float x)
        {
            var h = hash & 15;
            var grad = 1.0f + (h & 7); // Gradient value 1.0, 2.0, ..., 8.0
            if ((h & 8) != 0)
            {
                grad = -grad; // Set a random sign for the gradient
            }

            return grad * x; // Multiply the gradient with the distance
        }

        private float Grad(int hash, float x, float y)
        {
            var h = hash & 7; // Convert low 3 bits of hash code
            var u = h < 4 ? x : y; // into 8 simple gradient directions,
            var v = h < 4 ? y : x; // and compute the dot product with (x,y).
            return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -2.0f * v : 2.0f * v);
        }

        private float Grad(int hash, float x, float y, float z)
        {
            var h = hash & 15; // Convert low 4 bits of hash code into 12 simple
            var u = h < 8 ? x : y; // gradient directions, and compute dot product.
            var v = h < 4 ? y : h == 12 || h == 14 ? x : z; // Fix repeats at h = 12 to 15
            return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -v : v);
        }

        private float Grad(int hash, float x, float y, float z, float t)
        {
            var h = hash & 31; // Convert low 5 bits of hash code into 32 simple
            var u = h < 24 ? x : y; // gradient directions, and compute dot product.
            var v = h < 16 ? y : z;
            var w = h < 8 ? z : t;
            return ((h & 1) != 0 ? -u : u) + ((h & 2) != 0 ? -v : v) + ((h & 4) != 0 ? -w : w);
        }
    }
}
