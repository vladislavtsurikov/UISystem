using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Style;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Common
{
    public abstract class AddStyleAction : EntityAction
    {
        [OdinSerialize]
        private List<StyleEntity> _styleEntities = new List<StyleEntity>();

        protected abstract string Style { get; }

        protected abstract bool GetState();

        protected override UniTask<bool> Run(CancellationToken token)
        {
            bool state = GetState();

            for (int i = 0; i < _styleEntities.Count; i++)
            {
                StyleEntity styleEntity = _styleEntities[i];
                if (styleEntity == null)
                {
                    continue;
                }

                var styleStateData = (StyleStateData)styleEntity.Data.GetElement(typeof(StyleStateData));
                if (styleStateData == null)
                {
                    continue;
                }

                if (state)
                {
                    styleStateData.AddStyle(Style);
                }
                else
                {
                    styleStateData.RemoveStyle(Style);
                }
            }

            return UniTask.FromResult(true);
        }
    }
}
