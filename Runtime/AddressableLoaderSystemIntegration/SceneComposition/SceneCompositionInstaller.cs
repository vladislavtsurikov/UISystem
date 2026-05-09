#if UI_SYSTEM_ZENJECT
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration
{
    public class SceneCompositionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SceneUICompositionService>().AsSingle().IfNotBound();
            Container.Bind<SceneCompositionService>().AsSingle().IfNotBound();
        }
    }
}

#endif
