#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.GameObjectCollider.Editor;
using VladislavTsurikov.MegaWorld.Editor.BrushEraseTool.PrototypeElements;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.Undo.Editor.GameObject;
using VladislavTsurikov.Undo.Editor.TerrainObjectRenderer;
using VladislavTsurikov.UnityUtility.Runtime;
using GameObjectUtility = VladislavTsurikov.UnityUtility.Runtime.GameObjectUtility;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using ToolsComponentStack = VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.ToolsComponentStack;

namespace VladislavTsurikov.MegaWorld.Editor.BrushEraseTool
{
    [Name("Brush Erase")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new[]
    {
        typeof(PrototypeTerrainObject), typeof(PrototypeGameObject), typeof(PrototypeTerrainDetail)
    })]
    [AddGlobalCommonComponents(new[] { typeof(TransformSpaceSettings), typeof(LayerSettings) })]
    [AddToolComponents(new[] { typeof(BrushEraseToolSettings), typeof(BrushSettings) })]
    [AddPrototypeComponents(
        new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject), typeof(PrototypeTerrainDetail) },
        new[] { typeof(AdditionalEraseSetting) })]
    [AddGroupComponents(new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject) },
        new[] { typeof(FilterSettings) })]
    [AddGroupComponents(new[] { typeof(PrototypeTerrainDetail) }, new[] { typeof(MaskFilterComponentSettings) })]
    public class BrushEraseTool : ToolWindow
    {
        private BrushSettings _brushSettings;
        private SpacingMouseMove _mouseMove = new();

        protected override void OnEnable()
        {
            _brushSettings =
                (BrushSettings)ToolsComponentStack.GetElement(typeof(BrushEraseTool), typeof(BrushSettings));

            _mouseMove = new SpacingMouseMove();
            _mouseMove.OnMouseDown += OnMouseDown;
            _mouseMove.OnMouseDrag += OnMouseDrag;
            _mouseMove.OnRepaint += OnRepaint;
        }

        protected override void DoTool()
        {
            _mouseMove.Spacing = _brushSettings.Spacing;
            _mouseMove.LookAtSize = _brushSettings.BrushSize;

            _mouseMove.Run();
        }

        protected override void HandleKeyboardEvents()
        {
            var brushSettings =
                (BrushSettings)ToolsComponentStack.GetElement(typeof(BrushEraseTool), typeof(BrushSettings));

            brushSettings.ScrollBrushRadiusEvent();
        }

        private void OnMouseDown()
        {
            foreach (Group group in WindowData.Instance.SelectedData.SelectedGroupList)
            {
                BoxArea area = _brushSettings.BrushJitterSettings.GetAreaVariables(_brushSettings,
                    _mouseMove.Raycast.Point,
                    group);

                if (area.RayHit != null)
                {
                    EraseGroup(group, area);
                }
            }
        }

        private void OnMouseDrag(Vector3 dragPoint)
        {
            foreach (Group group in WindowData.Instance.SelectedData.SelectedGroupList)
            {
                RayHit originalRaycastInfo = RaycastUtility.Raycast(RayUtility.GetRayDown(dragPoint),
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));

                if (originalRaycastInfo != null)
                {
                    BoxArea area = _brushSettings.GetAreaVariables(originalRaycastInfo);

                    EraseGroup(group, area);
                }
            }
        }

        private void OnRepaint()
        {
            BoxArea area = _brushSettings.GetAreaVariables(_mouseMove.Raycast);
            BrushEraseToolVisualisation.Draw(area, WindowData.Instance.SelectionData,
                GlobalCommonComponentSingleton<LayerSettings>.Instance);
        }

        private void EraseGroup(Group group, BoxArea area)
        {
            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                EraseGameObject(group, area);
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainDetail))
            {
                EraseTerrainDetails(group, area);
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainObject))
            {
                EraseTerrainObject(group, area);
            }
        }

        private void EraseTerrainObject(Group group, BoxArea boxArea)
        {
#if RENDERER_STACK
            var brushEraseToolSettings =
                (BrushEraseToolSettings)ToolsComponentStack.GetElement(typeof(BrushEraseTool),
                    typeof(BrushEraseToolSettings));

            var filterSettings =
                (FilterSettings)group.GetElement(typeof(BrushEraseTool), typeof(FilterSettings));

            if (filterSettings.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterSettings.MaskFilterComponentSettings, boxArea);
            }

            PrototypeTerrainObjectOverlap.OverlapBox(boxArea.Bounds, null, false, true, (proto, instance) =>
            {
                if (proto.Active == false || proto.Selected == false)
                {
                    return true;
                }

                var additionalEraseSetting =
                    (AdditionalEraseSetting)proto.GetElement(typeof(BrushEraseTool), typeof(AdditionalEraseSetting));

                float fitness = 1;

                if (filterSettings.FilterType == FilterType.SimpleFilter)
                {
                    if (Physics.Raycast(RayUtility.GetRayFromCameraPosition(instance.Position), out RaycastHit hit,
                            PreferenceElementSingleton<RaycastPreferenceSettings>.Instance.MaxRayDistance,
                            GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(
                                group.PrototypeType)))
                    {
                        fitness = filterSettings.SimpleFilter.GetFitness(hit.point, hit.normal);
                    }
                }
                else
                {
                    if (filterSettings.MaskFilterComponentSettings.MaskFilterStack.ElementList.Count != 0)
                    {
                        fitness =
                            TextureUtility.GetFromWorldPosition(boxArea.Bounds, instance.Position,
                                filterSettings.MaskFilterComponentSettings.FilterMaskTexture2D);
                    }
                }

                var maskFitness =
                    TextureUtility.GetFromWorldPosition(boxArea.Bounds, instance.Position, boxArea.Mask);

                fitness *= maskFitness;

                fitness *= brushEraseToolSettings.EraseStrength;

                var successOfErase = Random.Range(0.0f, 1.0f);

                if (successOfErase < fitness)
                {
                    var randomSuccessForErase = Random.Range(0.0f, 1.0f);

                    if (randomSuccessForErase < additionalEraseSetting.Success / 100)
                    {
                        Undo.Editor.Undo.RegisterUndoAfterMouseUp(new DestroyedTerrainObject(instance));
                        instance.Destroy();
                    }
                }

                return true;
            });
#endif
        }

        private void EraseGameObject(Group group, BoxArea boxArea)
        {
            var brushEraseToolSettings =
                (BrushEraseToolSettings)ToolsComponentStack.GetElement(typeof(BrushEraseTool),
                    typeof(BrushEraseToolSettings));

            var filterSettings = (FilterSettings)group.GetElement(typeof(BrushEraseTool), typeof(FilterSettings));

            if (filterSettings.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterSettings.MaskFilterComponentSettings, boxArea);
            }

            PrototypeGameObjectOverlap.OverlapBox(boxArea.Bounds, (proto, go) =>
            {
                if (proto == null || proto.Active == false || proto.Selected == false)
                {
                    return true;
                }

                var additionalEraseSetting =
                    (AdditionalEraseSetting)proto.GetElement(typeof(BrushEraseTool), typeof(AdditionalEraseSetting));

                GameObject prefabRoot = GameObjectUtility.GetPrefabRoot(go);

                if (prefabRoot == null)
                {
                    return true;
                }

                if (boxArea.Bounds.Contains(prefabRoot.transform.position))
                {
                    float fitness = 1;

                    if (filterSettings.FilterType == FilterType.SimpleFilter)
                    {
                        if (Physics.Raycast(RayUtility.GetRayFromCameraPosition(prefabRoot.transform.position),
                                out RaycastHit hit,
                                PreferenceElementSingleton<RaycastPreferenceSettings>.Instance.MaxRayDistance,
                                GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(
                                    group.PrototypeType)))
                        {
                            fitness = filterSettings.SimpleFilter.GetFitness(hit.point, hit.normal);
                        }
                    }
                    else
                    {
                        if (filterSettings.MaskFilterComponentSettings.MaskFilterStack.ElementList.Count != 0)
                        {
                            fitness = TextureUtility.GetFromWorldPosition(boxArea.Bounds, prefabRoot.transform.position,
                                filterSettings.MaskFilterComponentSettings.FilterMaskTexture2D);
                        }
                    }

                    var maskFitness = TextureUtility.GetFromWorldPosition(boxArea.Bounds, prefabRoot.transform.position,
                        boxArea.Mask);

                    fitness *= maskFitness;

                    fitness *= brushEraseToolSettings.EraseStrength;

                    var successOfErase = Random.Range(0.0f, 1.0f);

                    if (successOfErase < fitness)
                    {
                        var randomSuccessForErase = Random.Range(0.0f, 1.0f);

                        if (randomSuccessForErase < additionalEraseSetting.Success / 100)
                        {
                            Undo.Editor.Undo.RegisterUndoAfterMouseUp(new DestroyedGameObject(go));
                            Object.DestroyImmediate(prefabRoot);
                        }
                    }
                }

                return true;
            });

            GameObjectColliderUtility.RemoveNullObjectNodesForAllScenes();
        }

        private void EraseTerrainDetails(Group group, BoxArea area)
        {
            var brushEraseToolSettings =
                (BrushEraseToolSettings)ToolsComponentStack.GetElement(typeof(BrushEraseTool),
                    typeof(BrushEraseToolSettings));

            if (TerrainResourcesController.IsSyncError(group, Terrain.activeTerrain))
            {
                return;
            }

            var maskFilterComponentSettings =
                (MaskFilterComponentSettings)group.GetElement(typeof(BrushEraseTool),
                    typeof(MaskFilterComponentSettings));

            FilterMaskOperation.UpdateMaskTexture(maskFilterComponentSettings, area);

            var eraseSize = new Vector2Int(
                UnityTerrainUtility.WorldToDetail(area.BoxSize, area.TerrainUnder.terrainData.size.x,
                    area.TerrainUnder.terrainData),
                UnityTerrainUtility.WorldToDetail(area.BoxSize, area.TerrainUnder.terrainData.size.z,
                    area.TerrainUnder.terrainData));

            Vector2Int halfBrushSize = eraseSize / 2;

            var center = new Vector2Int(
                UnityTerrainUtility.WorldToDetail(area.RayHit.Point.x - area.TerrainUnder.transform.position.x,
                    area.TerrainUnder.terrainData),
                UnityTerrainUtility.WorldToDetail(area.RayHit.Point.z - area.TerrainUnder.transform.position.z,
                    area.TerrainUnder.terrainData.size.z,
                    area.TerrainUnder.terrainData));

            Vector2Int position = center - halfBrushSize;
            var startPosition = Vector2Int.Max(position, Vector2Int.zero);

            Vector2Int offset = startPosition - position;

            Vector2Int current;
            float fitness = 1;
            float detailmapResolution = area.TerrainUnder.terrainData.detailResolution;
            int x, y;

            foreach (Prototype prototype in group.PrototypeList)
            {
                var prototypeTerrainDetail = (PrototypeTerrainDetail)prototype;

                if (prototypeTerrainDetail.Active == false || prototypeTerrainDetail.Selected == false)
                {
                    continue;
                }

                var additionalEraseSetting =
                    (AdditionalEraseSetting)prototypeTerrainDetail.GetElement(typeof(BrushEraseTool),
                        typeof(AdditionalEraseSetting));

                var localData = area.TerrainUnder.terrainData.GetDetailLayer(
                    startPosition.x, startPosition.y,
                    Mathf.Max(0, Mathf.Min(position.x + eraseSize.x, (int)detailmapResolution) - startPosition.x),
                    Mathf.Max(0, Mathf.Min(position.y + eraseSize.y, (int)detailmapResolution) - startPosition.y),
                    prototypeTerrainDetail.TerrainProtoId);

                float widthY = localData.GetLength(1);
                float heightX = localData.GetLength(0);

                if (maskFilterComponentSettings.MaskFilterStack.ElementList.Count > 0)
                {
                    for (y = 0; y < widthY; y++)
                    for (x = 0; x < heightX; x++)
                    {
                        current = new Vector2Int(y, x);

                        var randomSuccess = Random.Range(0.0f, 1.0f);

                        if (randomSuccess < additionalEraseSetting.Success / 100)
                        {
                            Vector2 normal = Vector2.zero;
                            normal.y = Mathf.InverseLerp(0, eraseSize.y, current.y);
                            normal.x = Mathf.InverseLerp(0, eraseSize.x, current.x);

                            fitness = TextureUtility.Get(normal, maskFilterComponentSettings.FilterMaskTexture2D);

                            var maskFitness = area.GetAlpha(current + offset, eraseSize);

                            fitness *= maskFitness;

                            fitness *= brushEraseToolSettings.EraseStrength;

                            var targetStrength = Mathf.Max(0,
                                localData[x, y] - Mathf.RoundToInt(Mathf.Lerp(0, 10, fitness)));

                            localData[x, y] = targetStrength;
                        }
                    }

                    area.TerrainUnder.terrainData.SetDetailLayer(startPosition.x, startPosition.y,
                        prototypeTerrainDetail.TerrainProtoId, localData);
                }
                else
                {
                    for (y = 0; y < widthY; y++)
                    for (x = 0; x < heightX; x++)
                    {
                        current = new Vector2Int(y, x);

                        var randomSuccess = Random.Range(0.0f, 1.0f);

                        if (randomSuccess < additionalEraseSetting.Success / 100)
                        {
                            var maskFitness = area.GetAlpha(current + offset, eraseSize);

                            maskFitness *= brushEraseToolSettings.EraseStrength;

                            var targetStrength = Mathf.Max(0,
                                localData[x, y] - Mathf.RoundToInt(Mathf.Lerp(0, 10, maskFitness)));

                            localData[x, y] = targetStrength;
                        }
                    }

                    area.TerrainUnder.terrainData.SetDetailLayer(startPosition.x, startPosition.y,
                        prototypeTerrainDetail.TerrainProtoId, localData);
                }
            }
        }
    }
}
#endif
