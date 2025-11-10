namespace VladislavTsurikov.CPUNoise.Runtime
{
    public static class NoiseUtility
    {
        public static void NormalizeArray(float[,] arr, int width, int height, ref float rangeMin, ref float rangeMax)
        {
            var min = float.PositiveInfinity;
            var max = float.NegativeInfinity;

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var v = arr[x, y];
                if (v < min)
                {
                    min = v;
                }

                if (v > max)
                {
                    max = v;
                }
            }

            rangeMin = min;
            rangeMax = max;

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var v = arr[x, y];
                arr[x, y] = (v - min) / (max - min);
            }
        }
    }
}
