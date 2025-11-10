#if UNITY_EDITOR
using System;
using System.Reflection;

namespace VladislavTsurikov.AutoDefines.Editor
{
    public abstract class TypeDefineRule : ConditionalDefineRule
    {
        public abstract string GetTypeFullName();

        protected override bool IsInstalled()
        {
            if (Type.GetType(GetTypeFullName(), false) != null)
            {
                return true;
            }

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly t in assemblies)
            {
                if (t.GetType(GetTypeFullName(), false) != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
#endif
