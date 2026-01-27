using System;
using VladislavTsurikov.EntityDataAction.Runtime;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Style
{
    public class StyleEntity : FilteredEntity
    {
        protected override Type[] ComponentDataTypesToCreate()
        {
            return new[] { typeof(StyleStateData) };
        }

        public override string[] GetAllowedActionNamePrefixes()
        {
            return new[] { "UI/Common/StyleGroup" };
        }
    }
}
