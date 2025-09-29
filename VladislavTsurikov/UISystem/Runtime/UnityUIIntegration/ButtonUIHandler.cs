using System;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public abstract class ButtonUIHandler : ChildSpawningUIHandler
    {
        protected ButtonUIHandler(DiContainer container)
            : base(container)
        {
        }

        public override T GetUIComponent<T>(string bindingId, int index = 0)
        {
            Type handlerType = Parent?.GetType() ?? GetType();

            return ResolveWithId<T>(bindingId, handlerType, index);
        }
    }
}
