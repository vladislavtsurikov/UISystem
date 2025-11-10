#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool
{
    public static class SelectionPrototypeUtility
    {
        public static void ScrollWheelAction(MouseMove mouseMove)
        {
            Group group = WindowData.Instance.SelectedData.SelectedGroup;

            if (group == null)
            {
                return;
            }

            if (WindowData.Instance.SelectedData.HasOneSelectedPrototype())
            {
                return;
            }

            Event e = Event.current;

            if (e.type == EventType.ScrollWheel)
            {
                if (e.delta.y < 0)
                {
                    SetPrecisionUnit(mouseMove, group, GetPrecisionUnit(group) + 1);
                }
                else if (e.delta.y > 0)
                {
                    SetPrecisionUnit(mouseMove, group, GetPrecisionUnit(group) - 1);
                }

                e.Use();
            }
        }

        public static PlacedObjectPrototype GetSelectedProto(Group group)
        {
            var settings =
                (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool),
                    typeof(PrecisePlaceToolSettings));

            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                PrototypeGameObject proto;
                if (settings.RandomSelectPrototype)
                {
                    proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(
                        group.GetAllSelectedPrototypes());
                }
                else
                {
                    proto = (PrototypeGameObject)group.PrototypeList[GetPrecisionUnit(group)];
                    if (!proto.Selected)
                    {
                        proto = (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(
                            group.GetAllSelectedPrototypes());
                    }
                }

                return proto;
            }
            else
            {
                PrototypeTerrainObject proto;
                if (settings.RandomSelectPrototype)
                {
                    proto = (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(
                        group.GetAllSelectedPrototypes());
                }
                else
                {
                    proto = (PrototypeTerrainObject)group.PrototypeList[GetPrecisionUnit(group)];
                    if (!proto.Selected)
                    {
                        proto = (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(
                            group.GetAllSelectedPrototypes());
                    }
                }

                return proto;
            }
        }

        private static void SetPrecisionUnit(MouseMove mouseMove, Group group, int index)
        {
            var precisionUnit = GetPrecisionUnit(group);
            if (index == precisionUnit)
            {
                return;
            }

            if (index > group.PrototypeList.Count - 1)
            {
                precisionUnit = 0;
            }
            else if (index < 0)
            {
                precisionUnit = group.PrototypeList.Count - 1;
            }
            else
            {
                precisionUnit = index;
            }

            ActiveObjectController.DestroyObject();

            ActiveObjectController.PlacedObjectData = PlaceObjectUtility.TryToPlace(group,
                (PlacedObjectPrototype)group.PrototypeList[precisionUnit], mouseMove.Raycast);
        }

        private static int GetPrecisionUnit(Group group)
        {
            if (ActiveObjectController.PlacedObjectData == null)
            {
                return 0;
            }

            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                for (var i = 0; i < group.PrototypeList.Count; i++)
                {
                    var proto = (PrototypeGameObject)group.PrototypeList[i];
                    if (GameObjectUtility.IsSameGameObject(proto.Prefab,
                            ActiveObjectController.PlacedObjectData.GameObject))
                    {
                        return i;
                    }
                }
            }
            else
            {
                for (var i = 0; i < group.PrototypeList.Count; i++)
                {
                    var proto = (PrototypeTerrainObject)group.PrototypeList[i];
                    if (GameObjectUtility.IsSameGameObject(proto.Prefab,
                            ActiveObjectController.PlacedObjectData.GameObject))
                    {
                        return i;
                    }
                }
            }

            return 0;
        }
    }
}
#endif
