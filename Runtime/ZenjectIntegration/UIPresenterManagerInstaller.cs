#if UI_SYSTEM_ZENJECT
using VladislavTsurikov.UISystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime
{
    public class UIPresenterManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            UIPresenterManager manager = new();
            Container.Bind<UIPresenterManager>().FromInstance(manager).AsSingle();
        }
    }
}

#endif
