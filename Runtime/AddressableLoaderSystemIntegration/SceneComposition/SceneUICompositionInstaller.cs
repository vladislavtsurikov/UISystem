#if UI_SYSTEM_ZENJECT
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration
{
    public class SceneUICompositionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SceneUICompositionService>().AsSingle().IfNotBound();
        }
    }
}

#endif
