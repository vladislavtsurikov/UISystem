using OdinSerializer;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.ProgressBar
{
    [Name("UI/Common/ProgressBarData")]
    public sealed class ProgressBarData : ComponentData
    {
        [OdinSerialize]
        private float _progress01;

        public float Progress01
        {
            get => _progress01;
            set
            {
                if (System.Math.Abs(_progress01 - value) < 0.001f)
                {
                    return;
                }

                _progress01 = value;
                MarkDirty();
            }
        }
    }
}
