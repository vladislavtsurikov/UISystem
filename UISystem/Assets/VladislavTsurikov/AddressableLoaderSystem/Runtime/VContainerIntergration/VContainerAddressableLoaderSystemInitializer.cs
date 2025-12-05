#if ADDRESSABLE_LOADER_SYSTEM_VCONTAINER
using System;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    public class VContainerAddressableLoaderSystemInitializer : AddressableLoaderSystemInitializer, IStartable
    {
        private readonly IContainerBuilder _builder;

        public VContainerAddressableLoaderSystemInitializer(IContainerBuilder builder)
        {
            _builder = builder;
        }

        protected override void Bind(Type type, object instance)
        {
            try
            {
                _builder.RegisterInstance(instance).As(type);
            }
            catch (Exception e)
            {
                Debug.LogError($"[VContainerAddressableLoaderSystemInitializer] Failed to bind {type.Name}: {e}");
            }
        }
    }
}
#endif
