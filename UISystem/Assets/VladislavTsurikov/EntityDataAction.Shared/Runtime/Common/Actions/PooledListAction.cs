using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Style;
using VladislavTsurikov.ObjectPool.Runtime;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Common
{
    public abstract class PooledListAction<TItem> : EntityAction where TItem : Entity
    {
        private List<TItem> _activeInstances;
        private MonoBehaviourPool<TItem> _pool;

        protected abstract TItem Prefab { get; }
        protected abstract Transform Container { get; }
        protected abstract int PoolMaxSize { get; }

        protected virtual bool SkipDisableWhenNotPlaying => false;

        protected sealed override void OnFirstSetupComponent(object[] setupData = null)
        {
            if (_pool == null && Prefab != null && Container != null)
            {
                _pool = new MonoBehaviourPool<TItem>(Prefab, Container, PoolMaxSize);
            }

            _activeInstances ??= new List<TItem>();

            OnFirstSetupComponentPooledList(setupData);
        }

        protected virtual void OnFirstSetupComponentPooledList(object[] setupData = null)
        {
        }

        protected sealed override void OnDisableElement()
        {
            if (SkipDisableWhenNotPlaying && !Application.isPlaying)
            {
                return;
            }

            ReleaseAllInstances();
        }

        protected sealed override async UniTask<bool> Run(CancellationToken token)
        {
            if (_pool == null || Prefab == null || Container == null)
            {
                Debug.LogWarning($"{GetType().Name}.{nameof(Run)}: Pool or prefab or container is null");
                return true;
            }

            int requiredCount = GetRequiredCount();
            if (requiredCount < 0)
            {
                Debug.LogWarning($"{GetType().Name}.{nameof(Run)}: GetRequiredCount returned negative value");
                return true;
            }

            await UpdateInstances(requiredCount, token);
            return true;
        }

        protected abstract int GetRequiredCount();
        protected abstract void SetupInstance(TItem instance, int index);

        protected virtual void OnInstanceCreated(TItem instance)
        {
        }

        protected virtual void OnInstanceReleased(TItem instance)
        {
        }

        private async UniTask UpdateInstances(int requiredCount, CancellationToken token)
        {
            int currentCount = _activeInstances.Count;

            for (int i = currentCount - 1; i >= requiredCount; i--)
            {
                TItem instance = _activeInstances[i];
                OnInstanceReleased(instance);
                _activeInstances.RemoveAt(i);
                _pool.Release(instance);
            }

            for (int i = 0; i < requiredCount; i++)
            {
                TItem instance;
                bool isNewInstance = false;

                if (i < _activeInstances.Count)
                {
                    instance = _activeInstances[i];
                }
                else
                {
                    instance = _pool.Get();
                    isNewInstance = true;
                    _activeInstances.Add(instance);
                    OnInstanceCreated(instance);
                }

                ClearStyles(instance);

                SetupInstance(instance, i);
                instance.transform.SetSiblingIndex(i);

                if (isNewInstance)
                {
                    Entity[] entities = instance.GetComponentsInChildren<Entity>(true);
                    for (int entityIndex = 0; entityIndex < entities.Length; entityIndex++)
                    {
                        entities[entityIndex].LocalActive = true;
                    }
                }
            }
        }

        private static void ClearStyles(TItem instance)
        {
            StyleEntity[] styleEntities = instance.GetComponentsInChildren<StyleEntity>(true);
            for (int styleIndex = 0; styleIndex < styleEntities.Length; styleIndex++)
            {
                StyleStateData styleStateData = styleEntities[styleIndex].GetData<StyleStateData>();
                if (styleStateData != null)
                {
                    styleStateData.ClearStyles();
                    styleStateData.Reset();
                }
            }
        }

        private void ReleaseAllInstances()
        {
            if (_pool == null || _activeInstances == null)
            {
                return;
            }

            for (int i = _activeInstances.Count - 1; i >= 0; i--)
            {
                TItem instance = _activeInstances[i];
                if (instance != null)
                {
                    OnInstanceReleased(instance);
                    _pool.Release(instance);
                }
            }

            _activeInstances.Clear();
        }
    }
}
