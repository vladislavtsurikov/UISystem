#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.PrototypeSettings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool
{
    public static class ObjectActions
    {
        public static void UpdateTransform(RayHit rayHit)
        {
            var settings =
                (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool),
                    typeof(PrecisePlaceToolSettings));

            if (ActiveObjectController.PlacedObjectData != null)
            {
                if (!settings.MouseActionStack.IsAnyMouseActionActive)
                {
                    if (settings.EnableSnapMove)
                    {
                        ActiveObjectController.PlacedObjectData.GameObject.transform.position =
                            Snapping.Snap(rayHit.Point, settings.SnapMove);
                    }
                    else
                    {
                        ActiveObjectController.PlacedObjectData.GameObject.transform.position = rayHit.Point;
                    }

                    var precisePlaceSettings =
                        (PrecisePlaceSettings)ActiveObjectController.PlacedObjectData.Proto.GetElement(
                            typeof(PrecisePlaceTool), typeof(PrecisePlaceSettings));

                    ChangePositionOffset(ActiveObjectController.PlacedObjectData.GameObject,
                        precisePlaceSettings.PositionOffset);

                    if (settings.Align)
                    {
                        AlignObject(ActiveObjectController.PlacedObjectData.GameObject, rayHit.Normal,
                            settings.WeightToNormal);
                    }
                }
            }
        }

        public static void ChangePositionOffset(GameObject gameObject, float offset) =>
            gameObject.transform.position += new Vector3(0, offset, 0);

        public static void AlignObject(GameObject gameObject, Vector3 normal, float weightToNormal)
        {
            Transform objectTransform = gameObject.transform;

            var normalRotation = Quaternion.FromToRotation(Vector3.up, normal);
            objectTransform.rotation = Quaternion.Lerp(Quaternion.identity, normalRotation, weightToNormal);
        }

        public static void RotateAlongStroke(GameObject gameObject, MouseMove mouseMove)
        {
            var settings =
                (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool),
                    typeof(PrecisePlaceToolSettings));

            Vector3Ex.GetOrientation(mouseMove.Raycast.Normal,
                settings.Align ? FromDirection.SurfaceNormal : FromDirection.Y, settings.WeightToNormal,
                out Vector3 upwards, out _, out Vector3 forward);

            var strokeForward = Vector3.Cross(mouseMove.StrokeDirection, upwards);

            if (strokeForward.magnitude > 0.001f)
            {
                forward = strokeForward;
            }

            gameObject.transform.rotation = Quaternion.LookRotation(forward, upwards);
        }
    }
}
#endif
