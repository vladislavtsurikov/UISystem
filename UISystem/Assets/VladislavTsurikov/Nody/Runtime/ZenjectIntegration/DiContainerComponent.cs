#if COMPONENT_STACK_ZENJECT
using VladislavTsurikov.Nody.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.Nody.Runtime
{
    public abstract class DiContainerComponent : Node
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
