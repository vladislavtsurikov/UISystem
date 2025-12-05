#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.MouseActions
{
    public abstract class MouseAction : Component
    {
        protected GameObject GameObject;
        protected Vector3 Normal;

        public virtual void CheckShortcutCombos(GameObject gameObject, Vector3 normal)
        {
        }

        public virtual void OnMouseMove()
        {
        }

        public virtual void OnRepaint()
        {
        }

        protected bool Begin(GameObject gameObject, Vector3 normal)
        {
            if (_active || gameObject == null)
            {
                return false;
            }

            GameObject = gameObject;
            _active = true;
            Normal = normal;
            Normal.Normalize();

            return true;
        }

        public void End() => _active = false;

        public bool IsFit()
        {
            if (_active && GameObject != null)
            {
                return true;
            }

            return false;
        }

        private void GetAxisPoints(Vector3 pos, Vector3 dir, Vector3[] points)
        {
            float dist2;
            var dist1 = dist2 = float.PositiveInfinity;
            points[0] = points[1] = pos;
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(SceneView.currentDrawingSceneView.camera);
            foreach (Plane plane in planes)
            {
                var ray = new Ray(pos, dir);
                if (plane.Raycast(ray, out var enter) && plane.GetSide(pos) && enter < dist1)
                {
                    points[0] = ray.GetPoint(enter);
                    dist1 = enter;
                }

                ray.direction = -dir;
                if (plane.Raycast(ray, out enter) && plane.GetSide(pos) && enter < dist2)
                {
                    points[1] = ray.GetPoint(enter);
                    dist2 = enter;
                }
            }
        }

        protected void DrawAxisLine(Vector3 pos, Vector3 dir)
        {
            var pointArray = new Vector3[2];

            GetAxisPoints(pos, dir, pointArray);
            Handles.DrawAAPolyLine(3, pointArray);
        }
    }
}
#endif
