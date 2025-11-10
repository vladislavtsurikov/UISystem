using System;
using UnityEngine;

namespace VladislavTsurikov.ColorUtility.Runtime
{
    [Serializable]
    public struct HSL
    {
        /// <summary> Construct a new HSL struct </summary>
        /// <param name="h"> Hue - h ∊ [0, 1] </param>
        /// <param name="s"> Saturation - s ∊ [0, 1] </param>
        /// <param name="l"> Lightness - l ∊ [0, 1] </param>
        public HSL(float h, float s, float l)
        {
            this.h = h;
            this.s = s;
            this.l = l;
        }

        /// <summary> Hue - h ∊ [0, 1] </summary>
        public float h;

        /// <summary> Saturation - s ∊ [0, 1] </summary>
        public float s;

        /// <summary> Lightness - l ∊ [0, 1] </summary>
        public float l;

        public HSL Copy() => new(h, s, l);

        public Color ToColor(float alpha = 1) => ColorUtils.HSLtoRGB(this).Validate().ToColor();

        public RGB ToRGB() => ColorUtils.HSLtoRGB(this);

        public HSL Validate()
        {
            h = ValidateColor(h, H.MIN, H.MAX);
            s = ValidateColor(s, S.MIN, S.MAX);
            l = ValidateColor(l, L.MIN, L.MAX);
            return this;
        }

        private float ValidateColor(float value, float min, float max) => Mathf.Max(min, Mathf.Min(max, value));

        public Vector3 Factorize() =>
            new()
            {
                x = FactorizeColor(h, H.MIN, H.MAX, H.F),
                y = FactorizeColor(s, S.MIN, S.MAX, S.F),
                z = FactorizeColor(l, L.MIN, L.MAX, L.F)
            };

        private int FactorizeColor(float value, float min, float max, float f) =>
            (int)Mathf.Max(min * f, Mathf.Min(max * f, Mathf.Round(value * f)));

        public string ToString(bool factorize = false) =>
            factorize
                ? $"hsl({Factorize().x}, {Factorize().y}%, {Factorize().z}%)"
                : $"hsl({h}, {s}%, {l}%)";

        /// <summary> Hue </summary>
        public struct H
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 360;
        }

        /// <summary> Saturation </summary>
        public struct S
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 100;
        }

        /// <summary> Lightness </summary>
        public struct L
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 100;
        }
    }
}
