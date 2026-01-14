using System;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.PrototypeSettings
{
    [Serializable]
    [Name("Precise Place Settings")]
    public class PrecisePlaceSettings : Node
    {
        public float PositionOffset;
    }
}
