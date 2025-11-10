using UnityEditor;
using UnityEngine;
using VladislavTsurikov.UnityUtility.Editor;

namespace VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem
{
    public enum HandleSettingsMode
    {
        Custom,
        Standard
    }

    public class Area : MonoBehaviour
    {
        public Bounds AreaBounds;
        public Vector3 PastThisPosition = Vector3.zero;
        public Vector3 PastScale = Vector3.one;

        public Color ColorCube = Color.HSVToRGB(0.0f, 0.75f, 1.0f);
        public float PixelWidth = 4.0f;
        public bool Dotted;
        public HandleSettingsMode HandleSettingsMode = HandleSettingsMode.Standard;
        public bool DrawHandleIfNotSelected;

        public void SetAreaBounds()
        {
            AreaBounds = new Bounds();
            AreaBounds.size = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            AreaBounds.center = transform.position;
        }

        public void SetBoundsIfNecessary(bool setAllParameters = false)
        {
            var hasChangedPosition = PastThisPosition != transform.position;
            var hasChangedSize = transform.localScale != PastScale;

            if (setAllParameters == false)
            {
                if (!hasChangedPosition && !hasChangedSize)
                {
                    return;
                }
            }

            SetAreaBounds();

            PastScale = transform.localScale;

            PastThisPosition = transform.position;
        }
    }

#if UNITY_EDITOR
    public class AreaGizmoDrawer
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NonSelected | GizmoType.NotInSelectionHierarchy |
                   GizmoType.Selected)]
        private static void DrawGizmoForArea(Area area, GizmoType gizmoType)
        {
            var isFaded = (int)gizmoType == (int)GizmoType.NonSelected ||
                          (int)gizmoType == (int)GizmoType.NotInSelectionHierarchy || (int)gizmoType ==
                          (int)GizmoType.NonSelected + (int)GizmoType.NotInSelectionHierarchy;

            if (area.DrawHandleIfNotSelected == false)
            {
                if (isFaded)
                {
                    return;
                }
            }

            var opacity = isFaded ? 0.5f : 1.0f;

            DrawBox(area, opacity);
        }

        private static void DrawBox(Area area, float alpha)
        {
            Transform newTransform = area.transform;
            newTransform.rotation = Quaternion.identity;
            newTransform.transform.localScale = new Vector3(Mathf.Max(1f, newTransform.transform.localScale.z),
                Mathf.Max(1f, newTransform.transform.localScale.y), Mathf.Max(1f, newTransform.transform.localScale.z));

            if (area.HandleSettingsMode == HandleSettingsMode.Custom)
            {
                Color color = area.ColorCube;
                color.a *= alpha;
                DrawHandles.DrawCube(newTransform.localToWorldMatrix, color, area.PixelWidth, area.Dotted);
            }
            else
            {
                var thickness = 4.0f;
                Color color = Color.yellow;
                color.a *= alpha;
                DrawHandles.DrawCube(newTransform.localToWorldMatrix, color, thickness);
            }
        }
    }
#endif
}
