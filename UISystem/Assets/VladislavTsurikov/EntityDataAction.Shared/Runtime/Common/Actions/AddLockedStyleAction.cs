using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Common
{
    [RunOnDirtyData(typeof(LockedData))]
    [RequiresData(typeof(LockedData))]
    [Name("UI/Common/Locked/AddLockedStyleAction")]
    public sealed class AddLockedStyleAction : AddStyleAction
    {
        protected override string Style => "Locked";

        protected override bool GetState()
        {
            return Get<LockedData>().IsLocked;
        }
    }
}
