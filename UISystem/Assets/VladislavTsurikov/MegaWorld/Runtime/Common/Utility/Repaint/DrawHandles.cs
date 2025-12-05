#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Repaint
{
    public static class DrawHandles
    {
        public static void DrawXYZCross(RayHit hit, Vector3 upwards, Vector3 right, Vector3 forward)
        {
            var handleSize = HandleUtility.GetHandleSize(hit.Point) * 0.5f;

            Handles.color = Color.green;
            Handles.DrawAAPolyLine(3, hit.Point + upwards * handleSize, hit.Point + upwards * -handleSize * 0.2f);
            Handles.color = Color.red;
            Handles.DrawAAPolyLine(3, hit.Point + right * handleSize, hit.Point + right * -handleSize * 0.2f);
            Handles.color = Color.blue;
            Handles.DrawAAPolyLine(3, hit.Point + forward * handleSize, hit.Point + forward * -handleSize * 0.2f);
        }

        public static void CircleCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current != null &&
                (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint))
            {
                Handles.CircleHandleCap(controlID, position, rotation, size, Event.current.type);
            }
        }

        public static void DotCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (Event.current != null &&
                (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint))
            {
                Handles.DotHandleCap(controlID, position, rotation, size, Event.current.type);
            }
        }
    }
}
#endif
