#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
#if ADDRESSABLE_LOADER_SYSTEM_ZENJECT
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    public class TestSceneSwitcherIMGUIInstaller : MonoInstaller
    {
        public override void InstallBindings() =>
            Container.Bind<TestSceneSwitcherIMGUI>().FromComponentInHierarchy().AsSingle();
    }
}
#endif
#endif
