using System;
using System.Linq;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class BehaviorAttribute : FilterAttribute
    {
        public Type BehaviorType { get; }
        public string[] Contexts { get; }

        public BehaviorAttribute(Type behaviorType, params string[] contexts)
        {
            BehaviorType = behaviorType;
            Contexts = contexts ?? Array.Empty<string>();
        }

        public bool Matches(string context) =>
            Contexts != null && Contexts.Contains(context, StringComparer.OrdinalIgnoreCase);
    }
}
