using System;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    internal readonly struct BoundComponentRecord
    {
        public Type Type { get; }
        public string Id { get; }

        public BoundComponentRecord(Type type, string id)
        {
            Type = type;
            Id = id;
        }
    }
}
