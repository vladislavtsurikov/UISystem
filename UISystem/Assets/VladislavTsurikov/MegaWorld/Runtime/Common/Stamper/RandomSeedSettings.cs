using UnityEngine;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    [Name("Random Seed Settings")]
    public class RandomSeedSettings : Node
    {
        public bool GenerateRandomSeed;
        public int RandomSeed;

        public void GenerateRandomSeedIfNecessary()
        {
            if (GenerateRandomSeed)
            {
                ChangeRandomSeed();
            }
            else
            {
                Random.InitState(RandomSeed);
            }
        }

        private void ChangeRandomSeed()
        {
            RandomSeed = Random.Range(0, int.MaxValue);

            Random.InitState(RandomSeed);
        }
    }
}
