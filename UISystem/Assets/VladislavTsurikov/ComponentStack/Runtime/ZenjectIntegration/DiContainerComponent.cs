#if COMPONENT_STACK_ZENJECT
using VladislavTsurikov.ComponentStack.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.ComponentStack.Runtime
{
    public abstract class DiContainerComponent : Component
    {
        protected DiContainer DiContainer;

        protected override void SetupComponent(object[] setupData = null)
        {
            if (setupData == null)
            {
                return;
            }

            DiContainer = (DiContainer)setupData[0];
            InjectSelf();
            SetupDiContainerComponent(setupData);
        }

        protected virtual void SetupDiContainerComponent(object[] setupData = null)
        {
        }

        protected void InjectSelf() => DiContainer.Inject(this);
    }
}
#endif
