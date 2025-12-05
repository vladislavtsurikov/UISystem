#if ACTIONFLOW_ZENJECT
using UnityEngine;
using Zenject;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.ZenjectIntegration
{
    public abstract class DiContainerMonoBehaviour : MonoBehaviour
    {
        protected DiContainer DiContainer;

        public void SetContainer(DiContainer container)
        {
            DiContainer = container;
            ApplySetup();
        }

        protected abstract void ApplySetup();
    }
}
#endif
