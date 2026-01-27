using OdinSerializer;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Refresh
{
    [Name("UI/Common/Refresh/RefreshData")]
    public sealed class RefreshData : ComponentData
    {
        [OdinSerialize]
        private bool _refresh;

        public bool Refresh
        {
            get => _refresh;
            set
            {
                _refresh = value;
                MarkDirty();
            }
        }
    }
}
