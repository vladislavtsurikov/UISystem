#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Actions;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;

namespace VladislavTsurikov.EntityDataActionFramework.Editor
{
    public sealed class ActionReorderableListComponentEditor : ReorderableListComponentEditor
    {
        public override void OnGUI(Rect rect, int index)
        {
            Action action = Target as Action;
            Entity entity = TryGetEntity(action);
            if (entity == null)
            {
                base.OnGUI(rect, index);
                return;
            }

            bool requirementsMet = RequiresDataUtility.IsRequirementsMet(entity.Data, action.GetType());
            if (!requirementsMet)
            {
                return;
            }

            base.OnGUI(rect, index);
        }

        public override float GetElementHeight(int index)
        {
            Action action = Target as Action;
            Entity entity = TryGetEntity(action);
            if (entity == null)
            {
                return base.GetElementHeight(index);
            }

            bool requirementsMet = RequiresDataUtility.IsRequirementsMet(entity.Data, action.GetType());
            if (!requirementsMet)
            {
                return 0f;
            }

            return base.GetElementHeight(index);
        }

        private static Entity TryGetEntity(Action action)
        {
            if (action is EntityAction entityAction)
            {
                return entityAction.Entity;
            }

            return null;
        }
    }
}
#endif
