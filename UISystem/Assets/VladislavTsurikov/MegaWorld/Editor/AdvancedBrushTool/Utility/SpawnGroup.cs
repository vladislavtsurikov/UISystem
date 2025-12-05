#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Spawn;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.AdvancedBrushTool
{
    public static class SpawnGroup
    {
        public static async UniTask SpawnGroupGameObject(Group group, BoxArea area, bool dragMouse)
        {
            var advancedBrushToolSettings =
                (AdvancedBrushToolSettings)ToolsComponentStack.GetElement(typeof(AdvancedBrushTool),
                    typeof(AdvancedBrushToolSettings));

            var filterSettings = (FilterSettings)group.GetElement(typeof(FilterSettings));

            if (filterSettings.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterSettings.MaskFilterComponentSettings, area);
            }

            var scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            scatterComponentSettings.ScatterStack.SetWaitingNextFrame(null);

            await scatterComponentSettings.ScatterStack.Samples(area, sample =>
            {
                if (dragMouse)
                {
                    if (Random.Range(0f, 100f) < advancedBrushToolSettings.FailureRateOnMouseDrag)
                    {
                        return;
                    }
                }

                RayHit rayHit = RaycastUtility.Raycast(
                    RayUtility.GetRayDown(new Vector3(sample.x, area.RayHit.Point.y, sample.y)),
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));
                if (rayHit != null)
                {
                    var proto =
                        (PrototypeGameObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                    if (proto == null || proto.Active == false)
                    {
                        return;
                    }

                    var fitness = GetFitness.Get(group, area.Bounds, rayHit);

                    var maskFitness = TextureUtility.GetFromWorldPosition(area.Bounds, rayHit.Point, area.Mask);

                    fitness *= maskFitness;

                    if (fitness != 0)
                    {
                        if (Random.Range(0f, 1f) < 1 - fitness)
                        {
                            return;
                        }

                        SpawnPrototype.SpawnGameObject(group, proto, rayHit, fitness, true);
                    }
                }
            });
        }


        public static async UniTask SpawnGroupTerrainObject(Group group, BoxArea area, bool dragMouse = false)
        {
#if RENDERER_STACK
            var advancedBrushToolSettings =
                (AdvancedBrushToolSettings)ToolsComponentStack.GetElement(typeof(AdvancedBrushTool),
                    typeof(AdvancedBrushToolSettings));

            var filterSettings = (FilterSettings)group.GetElement(typeof(FilterSettings));

            if (filterSettings.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterSettings.MaskFilterComponentSettings, area);
            }

            var scatterComponentSettings =
                (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));
            scatterComponentSettings.ScatterStack.SetWaitingNextFrame(null);

            await scatterComponentSettings.ScatterStack.Samples(area, sample =>
            {
                if (dragMouse)
                {
                    if (Random.Range(0f, 100f) < advancedBrushToolSettings.FailureRateOnMouseDrag)
                    {
                        return;
                    }
                }

                RayHit rayHit =
                    RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(sample.x, area.RayHit.Point.y, sample.y)),
                        GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(
                            group.PrototypeType));
                if (rayHit != null)
                {
                    var proto =
                        (PrototypeTerrainObject)GetRandomPrototype.GetMaxSuccessProto(group.GetAllSelectedPrototypes());

                    if (proto == null || proto.Active == false)
                    {
                        return;
                    }

                    var fitness = GetFitness.Get(group, area.Bounds, rayHit);

                    var maskFitness = TextureUtility.GetFromWorldPosition(area.Bounds, rayHit.Point, area.Mask);

                    fitness *= maskFitness;

                    if (fitness != 0)
                    {
                        if (Random.Range(0f, 1f) < 1 - fitness)
                        {
                            return;
                        }

                        SpawnPrototype.SpawnTerrainObject(proto, rayHit, fitness, true);
                    }
                }
            });
#endif
        }
    }
}
#endif
