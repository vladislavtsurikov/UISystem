using VladislavTsurikov.AutoDefines.Editor.Core;

namespace VladislavTsurikov.AutoDefines.Editor
{
    public abstract class ConditionalDefineRule : AutoDefineRule
    {
        protected abstract bool IsInstalled();

        protected virtual void InstallIfMissing()
        {
        }

        public override void Run()
        {
            if (ApplyDefineIsInstalled())
            {
                return;
            }

            InstallIfMissing();
        }

        private bool ApplyDefineIsInstalled()
        {
            if (IsInstalled())
            {
                DefineSymbolsBatcher.ApplyDefine(GetDefineToApplySymbol(), true);
                return true;
            }

            return false;
        }
    }
}
