#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.PrototypeSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.UnityUtility.Runtime;
using GameObjectUtility = VladislavTsurikov.MegaWorld.Runtime.Core.Utility.GameObjectUtility;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool
{
    public static class PlaceObjectUtility
    {
        public static PlacedObjectData DragPlace(Group group, PlacedObjectPrototype proto, RayHit rayHit,
            MouseMove mouseMove)
        {
            var settings =
                (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool),
                    typeof(PrecisePlaceToolSettings));

            PlacedObjectData placedObjectData = TryToPlace(group, proto, rayHit);

            if (placedObjectData == null)
            {
                return null;
            }

            if (settings.AlongStroke)
            {
                ObjectActions.RotateAlongStroke(placedObjectData.GameObject, mouseMove);
            }

            return placedObjectData;
        }

        public static PlacedObjectData TryToPlace(Group group, PlacedObjectPrototype proto, RayHit rayHit)
        {
            if (proto == null)
            {
                return null;
            }

            if (rayHit == null)
            {
                return null;
            }

            var settings =
                (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool),
                    typeof(PrecisePlaceToolSettings));

            Instance instance = GetInstancedDataForSpawn(proto, rayHit, settings.RememberPastTransform);

            if (!CanPlace(proto, instance))
            {
                return null;
            }

            return Place(group, proto, rayHit, instance);
        }

        private static PlacedObjectData Place(Group group, PlacedObjectPrototype proto, RayHit rayHit,
            Instance instance)
        {
            var settings =
                (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool),
                    typeof(PrecisePlaceToolSettings));

            GameObject gameObject =
                GameObjectUtility.Instantiate(proto.Prefab, instance.Position, instance.Scale, instance.Rotation);

            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(gameObject);
            }

            if (settings.Align)
            {
                ObjectActions.AlignObject(gameObject, rayHit.Normal, settings.WeightToNormal);
            }

            var precisePlaceSettings =
                (PrecisePlaceSettings)proto.GetElement(typeof(PrecisePlaceTool), typeof(PrecisePlaceSettings));

            ObjectActions.ChangePositionOffset(gameObject, precisePlaceSettings.PositionOffset);

            return new PlacedObjectData(proto, gameObject);
        }

        public static bool CanPlace(PlacedObjectPrototype proto, GameObject go)
        {
            var instance = new Instance(go);
            return CanPlace(proto, instance);
        }

        private static bool CanPlace(PlacedObjectPrototype proto, Instance instance)
        {
            var settings =
                (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool),
                    typeof(PrecisePlaceToolSettings));

            if (settings.OverlapCheck)
            {
                var overlapCheckSettings = (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

                if (proto is PrototypeGameObject)
                {
                    if (!OverlapCheckSettings.RunOverlapCheck(proto.GetType(), overlapCheckSettings, proto.Extents,
                            instance))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!OverlapCheckSettings.RunOverlapCheck(proto.GetType(), overlapCheckSettings, proto.Extents,
                            instance))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static Instance GetInstancedDataForSpawn(PlacedObjectPrototype proto, RayHit rayHit,
            bool rememberPastTransform)
        {
            var settings =
                (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool),
                    typeof(PrecisePlaceToolSettings));

            Vector3 scaleFactor = Vector3.one;
            Quaternion rotation = Quaternion.identity;

            if (rememberPastTransform && proto.PastTransform != null)
            {
                scaleFactor = proto.PastTransform.Scale;
                rotation = proto.PastTransform.Rotation;
            }

            var instance = new Instance(rayHit.Point, scaleFactor, rotation);

            if (settings.UseTransformComponents)
            {
                var transformComponentSettings =
                    (TransformComponentSettings)proto.GetElement(typeof(TransformComponentSettings));
                transformComponentSettings.TransformComponentStack.ManipulateTransform(ref instance, 1, rayHit.Normal);
            }

            return instance;
        }
    }
}
#endif
