using System;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.TextureStamperTool
{
    [Name("Stamper Tool Controller")]
    public class StamperControllerSettings : Node
    {
        public bool AutoRespawn;
        public float DelayAutoRespawn;

        [NonSerialized]
        public StamperTool StamperTool;

        public bool Visualisation = true;
    }
}
