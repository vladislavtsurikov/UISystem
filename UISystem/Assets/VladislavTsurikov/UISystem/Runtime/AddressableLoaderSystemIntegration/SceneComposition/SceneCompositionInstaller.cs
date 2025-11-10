#if UI_SYSTEM_ZENJECT
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration
{
    public class SceneCompositionInstaller : MonoInstaller
    {
        public override void InstallBindings() => Container.Bind<SceneCompositionService>().AsSingle();
    }
}

#endif
