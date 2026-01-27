#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Actions;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;

namespace VladislavTsurikov.EntityDataAction.Editor.Core
{
    public class ActionReorderableListComponentEditor : ReorderableListComponentEditor
    {
        public Entity Entity => GetEntityFromTarget();

        public override void OnGUI(Rect rect, int index)
        {
            Action action = Target as Action;
            Entity entity = GetEntityFromTarget();
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
            Entity entity = GetEntityFromTarget();
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

        protected Entity GetEntityFromTarget()
        {
            if (Target is EntityAction entityAction)
            {
                return entityAction.Entity;
            }

            return null;
        }
    }
}
#endif
