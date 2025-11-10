using System;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class FilterAttribute : Attribute
    {
    }
}
