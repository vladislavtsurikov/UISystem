using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    public class TestSceneSwitcherIMGUIInstaller : MonoInstaller
    {
        public override void InstallBindings() =>
            Container.Bind<TestSceneSwitcherIMGUI>().FromComponentInHierarchy().AsSingle();
    }
}
