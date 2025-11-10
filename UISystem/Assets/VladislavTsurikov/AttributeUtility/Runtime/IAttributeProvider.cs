using System;

namespace VladislavTsurikov.AttributeUtility.Runtime
{
    public interface IAttributeProvider
    {
        Attribute[] GetCustomAttributes(bool inherit);
    }
}
