using System;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class BehaviorAttribute : Attribute
    {
        public Type BehaviorType { get; }
        public string[] Contexts { get; }

        public BehaviorAttribute(Type behaviorType, params string[] contexts)
        {
            BehaviorType = behaviorType;
            Contexts = contexts;
        }
    }
}
