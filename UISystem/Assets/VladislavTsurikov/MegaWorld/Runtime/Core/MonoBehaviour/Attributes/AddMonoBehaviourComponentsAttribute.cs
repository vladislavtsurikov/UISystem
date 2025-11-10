using System;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AddMonoBehaviourComponentsAttribute : Attribute
    {
        public readonly Type[] Types;

        public AddMonoBehaviourComponentsAttribute(Type[] types) => Types = types;
    }
}
