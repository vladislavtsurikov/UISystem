#if UI_SYSTEM_ZENJECT
using Zenject;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    public class TestSceneSwitcherIMGUIInstaller : MonoInstaller
    {
        public override void InstallBindings() =>
            Container.Bind<TestSceneSwitcherIMGUI>().FromComponentInHierarchy().AsSingle();
    }
}

#endif
