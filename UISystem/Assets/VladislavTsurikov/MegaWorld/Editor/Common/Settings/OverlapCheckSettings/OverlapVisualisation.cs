#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.OverlapCheckSettings
{
    public static class OverlapVisualisation
    {
        public static void Draw(Bounds bounds, SelectionData data, bool visualizeSelectedProto = true)
        {
            if (data.SelectedData.GetSelectedPrototypes(typeof(PrototypeGameObject)).Count != 0)
            {
                VisualizeOverlapForGameObject(bounds, visualizeSelectedProto);
            }

            if (data.SelectedData.GetSelectedPrototypes(typeof(PrototypeTerrainObject)).Count != 0)
            {
                VisualizeOverlapForTerrainObject(bounds, visualizeSelectedProto);
            }
        }

        private static void VisualizeOverlapForTerrainObject(Bounds bounds, bool visualizeSelectedProto = true)
        {
#if RENDERER_STACK
            PrototypeTerrainObjectOverlap.OverlapBox(bounds, null, false, true, (proto, terrainObjectInstance) =>
            {
                if (visualizeSelectedProto)
                {
                    if (!proto.Selected)
                    {
                        return true;
                    }
                }

                var overlapCheckSettings =
                    (Runtime.Common.Settings.OverlapCheckSettings.OverlapCheckSettings)proto.GetElement(
                        typeof(Runtime.Common.Settings.OverlapCheckSettings.OverlapCheckSettings));

                overlapCheckSettings.CurrentOverlapShape?.DrawOverlapVisualisation(terrainObjectInstance.Position,
                    terrainObjectInstance.Scale, terrainObjectInstance.Rotation, proto.Extents);

                return true;
            });
#endif
        }

        private static void VisualizeOverlapForGameObject(Bounds bounds, bool visualizeSelectedProto = true) =>
            PrototypeGameObjectOverlap.OverlapBox(bounds, (proto, go) =>
            {
                if (visualizeSelectedProto)
                {
                    if (!proto.Selected)
                    {
                        return true;
                    }
                }

                var overlapCheckSettings =
                    (Runtime.Common.Settings.OverlapCheckSettings.OverlapCheckSettings)proto.GetElement(
                        typeof(Runtime.Common.Settings.OverlapCheckSettings.OverlapCheckSettings));

                overlapCheckSettings.CurrentOverlapShape?.DrawOverlapVisualisation(go.transform.position,
                    go.transform.lossyScale, go.transform.rotation, proto.Extents);

                return true;
            });
    }
}
#endif
