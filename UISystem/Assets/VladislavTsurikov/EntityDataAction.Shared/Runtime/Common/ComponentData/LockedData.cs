using OdinSerializer;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Common
{
    [Name("UI/Gunship/LockedData")]
    public sealed class LockedData : ComponentData
    {
        [OdinSerialize]
        private bool _isLocked;

        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                if (_isLocked == value)
                {
                    return;
                }

                _isLocked = value;
                MarkDirty();
            }
        }
    }
}
