using System;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Refresh
{
    [ExecuteInEditMode]
    public class RefreshEntity : FilteredEntity
    {
        protected override Type[] ComponentDataTypesToCreate()
        {
            return new[]
            {
                typeof(RefreshData)
            };
        }

        public override string[] GetAllowedActionNamePrefixes()
        {
            return new[] { "UI/Common/Refresh/" };
        }
    }
}
