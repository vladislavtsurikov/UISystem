using VladislavTsurikov.UISystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime
{
    public class UIHandlerManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var manager = new ZenjectUIHandlerManager(Container);
            Container.Bind<UIHandlerManager>().FromInstance(manager).AsSingle();
        }
    }
}
