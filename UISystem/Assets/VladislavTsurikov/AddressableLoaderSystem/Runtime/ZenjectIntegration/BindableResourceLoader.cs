#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
#if ADDRESSABLE_LOADER_SYSTEM_ZENJECT
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using Zenject;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.ZenjectIntegration
{
    public abstract class BindableResourceLoader : ResourceLoader
    {
        private readonly List<(Type type, object identifier)> _boundTypes = new();
        private readonly DiContainer _container;

        public BindableResourceLoader(DiContainer container) => _container = container;

        protected sealed override async UniTask UnloadResourceLoader(CancellationToken token)
        {
            await OnResourceUnload();

            foreach ((Type type, var id) in _boundTypes)
            {
                if (id != null)
                {
                    _container.UnbindId(type, id);
                }
                else
                {
                    _container.Unbind(type);
                }
            }

            _boundTypes.Clear();
        }

        protected virtual async UniTask OnResourceUnload() => await UniTask.CompletedTask;

        protected void BindToContainer<T>(T instance, object id = null)
        {
            if (instance == null)
            {
                return;
            }

            if (id != null)
            {
                _container.Bind<T>().WithId(id).FromInstance(instance).AsSingle();
                _boundTypes.Add((typeof(T), id));
            }
            else
            {
                _container.Bind<T>().FromInstance(instance).AsSingle();
                _boundTypes.Add((typeof(T), null));
            }
        }

        protected UniTask<T> LoadAndBind<T>(CancellationToken token, string key, object id = null) where T : Object =>
            LoadAndTrack<T>(key, token)
                .ContinueWith(obj =>
                {
                    if (obj == null)
                    {
                        return null;
                    }

                    if (obj is Object uObj && uObj == null)
                    {
                        return null;
                    }

                    BindToContainer(obj, id);
                    return obj;
                });
    }
}
#endif
#endif
