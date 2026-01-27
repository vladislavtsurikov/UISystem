using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Common
{
    [RunOnDirtyData(typeof(SelectedData))]
    [RequiresData(typeof(SelectedData))]
    [Name("UI/Common/Selected/AddSelectedStyleAction")]
    public sealed class AddSelectedStyleAction : AddStyleAction
    {
        protected override string Style => "Selected";

        protected override bool GetState()
        {
            return Get<SelectedData>().IsSelected;
        }
    }
}
