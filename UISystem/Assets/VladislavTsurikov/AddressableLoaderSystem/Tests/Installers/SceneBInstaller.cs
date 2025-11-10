#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
#if ADDRESSABLE_LOADER_SYSTEM_ZENJECT
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    public class SceneBInstaller : MonoInstaller
    {
        public override void InstallBindings() => Container.Bind<SceneBInjectionValidator>()
            .FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }
}
#endif
#endif
