#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.MouseActions
{
    [Serializable]
    [CreateNodes(new[] { typeof(MoveAlongDirection), typeof(Scale), typeof(Rotation) })]
    public class MouseActionStack : NodeStackOnlyDifferentTypes<MouseAction>
    {
        public bool IsAnyMouseActionActive
        {
            get
            {
                foreach (MouseAction data in _elementList)
                {
                    if (data.Active)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        protected void OnRemoveInvalidComponents() => RemoveElementsWithSameType();

        public void OnMouseMove()
        {
            foreach (MouseAction data in _elementList)
            {
                if (data.IsFit())
                {
                    data.OnMouseMove();
                    break;
                }
            }
        }

        public void CheckShortcutCombos(GameObject gameObject, RayHit hit)
        {
            if (hit == null)
            {
                return;
            }

            foreach (MouseAction data in _elementList)
            {
                data.CheckShortcutCombos(gameObject, hit.Normal);
            }
        }

        public void End()
        {
            foreach (MouseAction data in _elementList)
            {
                data.End();
            }
        }

        public void OnRepaint()
        {
            foreach (MouseAction data in _elementList)
            {
                if (data.IsFit())
                {
                    data.OnRepaint();
                    break;
                }
            }
        }
    }
}
#endif
