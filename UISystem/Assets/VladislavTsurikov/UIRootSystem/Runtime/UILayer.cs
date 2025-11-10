#if UI_SYSTEM_ZENJECT
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;
using VladislavTsurikov.UISystem.Runtime.UnityUIIntegration;
using Zenject;

namespace VladislavTsurikov.UIRootSystem.Runtime
{
    public abstract class UILayer : UnityUIHandler
    {
        protected UILayer(DiContainer container, PrefabResourceLoader loader)
            : base(container, loader)
        {
        }

        public abstract int GetLayerIndex();
    }
}

#endif
