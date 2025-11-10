using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    [Name("Random Seed Settings")]
    public class RandomSeedSettings : Component
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
