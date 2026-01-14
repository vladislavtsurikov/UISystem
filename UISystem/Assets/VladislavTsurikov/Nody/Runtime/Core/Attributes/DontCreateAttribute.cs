using System;

namespace VladislavTsurikov.Nody.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DontCreateAttribute : Attribute
    {
    }
}
