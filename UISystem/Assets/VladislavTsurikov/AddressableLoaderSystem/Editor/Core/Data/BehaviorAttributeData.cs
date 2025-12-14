using System;
using System.Collections.Generic;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    [Serializable]
    public class BehaviorAttributeData
    {
        public Type BehaviorType;
        public List<string> Contexts = new();
    }
}
