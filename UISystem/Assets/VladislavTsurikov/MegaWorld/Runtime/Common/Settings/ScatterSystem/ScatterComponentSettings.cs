using System;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    [Serializable]
    [Name("Scatter Settings")]
    public class ScatterComponentSettings : Component
    {
        public ScatterStack ScatterStack = new();

        protected override void SetupComponent(object[] setupData = null) => ScatterStack.Setup();

        protected override void OnCreate() => ScatterStack.CreateIfMissingType(typeof(RandomGrid));
    }
}
