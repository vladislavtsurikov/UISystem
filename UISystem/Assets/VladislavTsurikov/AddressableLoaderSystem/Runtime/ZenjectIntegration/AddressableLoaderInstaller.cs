#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
#if ADDRESSABLE_LOADER_SYSTEM_ZENJECT
using Zenject;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.ZenjectIntegration
{
    public class AddressableLoaderInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ZenjectAddressableLoaderSystemInitializer>().AsSingle();
        }
    }
}
#endif
#endif
