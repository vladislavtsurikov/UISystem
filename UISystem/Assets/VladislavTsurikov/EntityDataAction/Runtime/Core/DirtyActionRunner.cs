using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using Action = VladislavTsurikov.ActionFlow.Runtime.Actions.Action;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    public sealed class DirtyActionRunner
    {
        private readonly NodeStackOnlyDifferentTypes<ComponentData> _data;
        private readonly EntityActionCollection _actions;

        private readonly HashSet<Type> _pendingDirtyTypes = new HashSet<Type>();
        private bool _isProcessingDirty;
        private Entity _entity;

        public DirtyActionRunner(Entity entity, NodeStackOnlyDifferentTypes<ComponentData> data, EntityActionCollection actions)
        {
            _entity = entity;
            _data = data;
            _actions = actions;
        }

        public void Setup()
        {
            _data.ElementDirtied += HandleDataDirtied;
        }

        public void OnDisable()
        {
            _data.ElementDirtied -= HandleDataDirtied;
            _pendingDirtyTypes.Clear();
            _isProcessingDirty = false;
        }

        public void Trigger(Type dirtiedType)
        {
            if (!_entity.Active)
            {
                return;
            }

            if (dirtiedType == null)
            {
                return;
            }

            _pendingDirtyTypes.Add(dirtiedType);

            if (_isProcessingDirty)
            {
                return;
            }

            ProcessDirtyLoop().Forget();
        }

        public void TriggerAll()
        {
            if (!_entity.Active)
            {
                return;
            }

            _pendingDirtyTypes.Add(typeof(ComponentData));

            if (_isProcessingDirty)
            {
                return;
            }

            ProcessDirtyLoop().Forget();
        }

        private void HandleDataDirtied(Node dirtiedComponent)
        {
            if (!_entity.Active)
            {
                return;
            }

            if (dirtiedComponent == null)
            {
                return;
            }

            Trigger(dirtiedComponent.GetType());
        }

        private async UniTask ProcessDirtyLoop()
        {
            if (!_entity.Active)
            {
                _pendingDirtyTypes.Clear();
                return;
            }

            _isProcessingDirty = true;

            try
            {
                while (_entity.Active && _pendingDirtyTypes.Count > 0)
                {
                    Type[] dirtySnapshot = new Type[_pendingDirtyTypes.Count];
                    _pendingDirtyTypes.CopyTo(dirtySnapshot);
                    _pendingDirtyTypes.Clear();

                    await RunDependentActions(dirtySnapshot);
                }
            }
            finally
            {
                _isProcessingDirty = false;

                if (!_entity.Active)
                {
                    _pendingDirtyTypes.Clear();
                }
            }
        }

        private async UniTask RunDependentActions(Type[] dirtiedTypes)
        {
            for (int i = 0; i < _actions.ElementList.Count; i++)
            {
                if (!_entity.Active)
                {
                    return;
                }

                Action action = _actions.ElementList[i];

                if (!action.Active)
                {
                    continue;
                }

                Type actionType = action.GetType();

                if (!ShouldRunForAnyDirtyType(actionType, dirtiedTypes))
                {
                    continue;
                }

                await action.RunAction(default);
            }
        }

        private static bool ShouldRunForAnyDirtyType(Type actionType, Type[] dirtiedTypes)
        {
            if (dirtiedTypes.Length == 1 && dirtiedTypes[0] == typeof(ComponentData))
            {
                return true;
            }

            for (int i = 0; i < dirtiedTypes.Length; i++)
            {
                if (RunOnDirtyDataUtility.MatchesDirtyType(actionType, dirtiedTypes[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
