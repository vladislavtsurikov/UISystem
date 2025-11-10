#if UNITY_EDITOR

namespace VladislavTsurikov.AutoDefines.Editor.Core
{
    public abstract class AutoDefineRule
    {
        protected abstract string GetDefineToApplySymbol();
        public abstract void Run();
    }
}
#endif
