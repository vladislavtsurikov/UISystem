using System;
using System.Collections.Generic;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas
{
    public sealed class SelectedPrototypes
    {
        public readonly Type PrototypeType;
        public readonly List<Prototype> SelectedPrototypeList = new();

        public SelectedPrototypes(Type prototypeType) => PrototypeType = prototypeType;
    }
}
