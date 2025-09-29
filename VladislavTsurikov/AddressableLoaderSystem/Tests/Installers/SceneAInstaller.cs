using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    public class SceneAInstaller : MonoInstaller
    {
        public override void InstallBindings() => Container.Bind<SceneAInjectionValidator>()
            .FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }
}
