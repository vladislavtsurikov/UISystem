#if UNITY_EDITOR
using UnityEditor.PackageManager.Requests;
using VladislavTsurikov.AutoDefines.Editor.Core;

namespace VladislavTsurikov.AutoDefines.Editor
{
    public abstract class PackageDefineRule : ConditionalDefineRule
    {
        private AddRequest _addRequest;

        protected abstract string GetPackageId();

        protected virtual bool ShouldAutoInstall() => false;

        protected override bool IsInstalled() => UpmListCache.IsInstalled(GetPackageId());

        protected override void InstallIfMissing()
        {
            if (!ShouldAutoInstall())
            {
                return;
            }

            UpmInstaller.Install(GetPackageId());
        }
    }
}
#endif
