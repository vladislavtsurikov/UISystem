using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.ZenjectIntegration
{
    public class AddressableLoaderInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var manager = new ResourceLoaderManager(Container);

            Container.Bind<ResourceLoaderManager>().FromInstance(manager).AsSingle();
        }
    }
}
