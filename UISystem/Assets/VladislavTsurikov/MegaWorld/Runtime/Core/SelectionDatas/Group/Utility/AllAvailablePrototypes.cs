using System.Collections.Generic;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Utility
{
    public static class AllAvailablePrototypes
    {
        private static readonly Dictionary<int, Prototype> _prototypes = new();

        public static void AddPrototype(Prototype prototype) => _prototypes.TryAdd(prototype.ID, prototype);

        public static void RemovePrototype(Prototype prototype) => _prototypes.Remove(prototype.ID);

        public static Prototype GetPrototype(int id) =>
            _prototypes.TryGetValue(id, out Prototype prototype) ? prototype : null;
    }
}
