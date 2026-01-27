using System;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using UnityEngine;
using Action = VladislavTsurikov.ActionFlow.Runtime.Actions.Action;

namespace VladislavTsurikov.ActionFlow.Runtime
{
    public class ActionCollection : NodeStackSupportSameType<Action>
    {
        public async UniTask<bool> Run(CancellationToken token = default)
        {
            foreach (Action action in ElementList)
            {
                token.ThrowIfCancellationRequested();
                bool isActionCompleted;
                try
                {
                    isActionCompleted = await action.RunAction(token);
                }
                catch (Exception ex)
                {
                    string actionName = action?.GetType().Name ?? "<null>";
                    string entityName = TryGetEntityName(action);
                    string entityInfo = string.IsNullOrEmpty(entityName) ? "Entity: <unknown>" : $"Entity: {entityName}";
                    Debug.LogError($"Action exception in {actionName}. {entityInfo}\n{ex}");
                    return false;
                }

                if (!isActionCompleted)
                {
                    return false;
                }
            }

            return true;
        }

        private static string TryGetEntityName(Action action)
        {
            if (action == null)
            {
                return null;
            }

            PropertyInfo entityProperty = action.GetType().GetProperty("Entity", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (entityProperty == null)
            {
                return null;
            }

            object entityObject = entityProperty.GetValue(action);
            if (entityObject == null)
            {
                return null;
            }

            if (entityObject is Component component)
            {
                return component.gameObject != null ? component.gameObject.name : component.name;
            }

            if (entityObject is UnityEngine.Object unityObject)
            {
                return unityObject.name;
            }

            return null;
        }
    }
}
