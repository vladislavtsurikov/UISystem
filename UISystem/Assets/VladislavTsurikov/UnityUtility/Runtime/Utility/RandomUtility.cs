using UnityEngine;

namespace VladislavTsurikov.UnityUtility.Runtime
{
    public static class RandomUtility
    {
        public static void ChangeRandomSeed()
        {
            var randomSeed = Random.Range(0, int.MaxValue);

            Random.InitState(randomSeed);
        }
    }
}
