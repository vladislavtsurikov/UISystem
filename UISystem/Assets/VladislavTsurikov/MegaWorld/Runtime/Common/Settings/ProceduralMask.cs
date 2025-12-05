using System;
using UnityEngine;
using VladislavTsurikov.CPUNoise.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings
{
    public enum Shape
    {
        Circle,
        Square
    }

    [Serializable]
    public class ProceduralMask
    {
        public Shape Shape = Shape.Circle;

        [Range(0f, 100f)]
        public float Falloff = 10;

        [Range(0f, 100f)]
        public float Strength = 100;

        private Texture2D _mask;

        public Texture2D Mask
        {
            get
            {
                if (_mask == null)
                {
                    CreateProceduralTexture();
                }

                return _mask;
            }
        }

        public void CreateProceduralTexture()
        {
            var width = 256;
            var height = 256;

            _mask = new Texture2D(width, height);

            var arr = new float[width, height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    switch (Shape)
                    {
                        case Shape.Circle:
                        {
                            var distance = Vector2.Distance(new Vector2(y, x), new Vector2(width / 2, height / 2));

                            float size = width / 2;
                            size = Mathf.Lerp(0, size, 0.98f);

                            var maxAddViability = Strength / 100;
                            var falloff = Falloff / 100;

                            if (falloff != 0)
                            {
                                if (distance <= size)
                                {
                                    var newMinRadius = Mathf.Lerp(size, 1, Falloff / 100);
                                    var grayScale = Mathf.InverseLerp(size, newMinRadius, distance);

                                    grayScale = Mathf.Lerp(0, grayScale, maxAddViability);

                                    if (FractalNoise)
                                    {
                                        var fractalNoiseValue =
                                            Mathf.InverseLerp(RangeMin, RangeMax, Fractal.Sample2D(x, y));

                                        AdditionalChanges(ref fractalNoiseValue);

                                        grayScale *= fractalNoiseValue;
                                    }

                                    arr[x, y] = grayScale;
                                }
                                else
                                {
                                    arr[x, y] = 0;
                                }
                            }
                            else
                            {
                                if (distance <= size)
                                {
                                    var grayScale = Mathf.Lerp(0, 1, maxAddViability);

                                    if (FractalNoise)
                                    {
                                        var fractalNoiseValue =
                                            Mathf.InverseLerp(RangeMin, RangeMax, Fractal.Sample2D(x, y));

                                        AdditionalChanges(ref fractalNoiseValue);

                                        grayScale *= fractalNoiseValue;
                                    }

                                    arr[x, y] = grayScale;
                                }
                                else
                                {
                                    arr[x, y] = 0;
                                }
                            }

                            break;
                        }
                        case Shape.Square:
                        {
                            var checkPoint = new Vector2(y, x);
                            var center = new Vector2(width / 2, height / 2);

                            float size = width;
                            var newMinSize = Mathf.Lerp(size, 0, Falloff / 100);
                            var maxAddViability = Strength / 100;

                            var originBoundsSize = new Bounds(center, new Vector3(newMinSize, newMinSize, newMinSize));
                            var offsetBoundsSize = new Bounds(center, new Vector3(size, size, size));

                            var maxDifference = offsetBoundsSize.extents.x - originBoundsSize.extents.x;
                            var minDifference = Vector3.Distance(checkPoint, originBoundsSize.ClosestPoint(checkPoint));

                            float grayScale = 0;

                            if (minDifference == 0)
                            {
                                grayScale = 1;
                            }
                            else
                            {
                                grayScale = Mathf.Lerp(1f, 0, minDifference / maxDifference);
                            }

                            grayScale = Mathf.Lerp(0, grayScale, maxAddViability);

                            if (FractalNoise)
                            {
                                var fractalNoiseValue = Mathf.InverseLerp(RangeMin, RangeMax, Fractal.Sample2D(x, y));

                                AdditionalChanges(ref fractalNoiseValue);

                                grayScale *= fractalNoiseValue;
                            }

                            arr[x, y] = grayScale;

                            break;
                        }
                    }
                }
            }

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var fractalValue = arr[x, y];

                    Mask.SetPixel(x, y, new Color(fractalValue, fractalValue, fractalValue, 1));
                }
            }

            Mask.Apply();
        }

        public void AdditionalChanges(ref float fractalNoiseValue)
        {
            if (Invert)
            {
                fractalNoiseValue = 1 - fractalNoiseValue;
            }

            if (fractalNoiseValue < RemapMin)
            {
                fractalNoiseValue = 0;
            }
            else if (fractalNoiseValue > RemapMax)
            {
                fractalNoiseValue = 1;
            }
            else
            {
                fractalNoiseValue = Mathf.InverseLerp(RemapMin, RemapMax, fractalNoiseValue);
            }
        }

        public void FindNoiseRangeMinMaxForProceduralNoise(int width, int height)
        {
            var fractal = new FractalNoiseCPU(GetNoiseForProceduralBrush(), Octaves, Frequency / 7, Lacunarity,
                Persistence);

            var arr = new float[width, height];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    arr[x, y] = fractal.Sample2D(x, y);
                }
            }

            NoiseUtility.NormalizeArray(arr, width, height, ref RangeMin, ref RangeMax);
        }

        public INoiseCPU GetNoiseForProceduralBrush()
        {
            switch (NoiseType)
            {
                case NoiseType.Perlin:
                    return new PerlinNoiseCPU(Seed, 20);
                case NoiseType.Value:
                    return new ValueNoiseCPU(Seed, 20);
                case NoiseType.Simplex:
                    return new SimplexNoiseCPU(Seed, 20);
                case NoiseType.Voronoi:
                    return new VoronoiNoiseCPU(Seed, 20);
                case NoiseType.Worley:
                    return new WorleyNoiseCPU(Seed, 20, 1.0f);
                default:
                    return new PerlinNoiseCPU(Seed, 20);
            }
        }

        #region Fractal Noise Settings

        public NoiseType NoiseType = NoiseType.Perlin;
        public FractalNoiseCPU Fractal = new();
        public float RangeMin = -0.5f;
        public float RangeMax = 0.5f;
        public int Seed;
        public int Octaves = 4;
        public float Frequency = 1f;
        public float Lacunarity = 2f;
        public float Persistence = 0.5f;
        public bool FractalNoise;

        [Range(0, 1)]
        public float RemapMin;

        [Range(0, 1)]
        public float RemapMax = 1f;

        public bool Invert;

        #endregion
    }
}
