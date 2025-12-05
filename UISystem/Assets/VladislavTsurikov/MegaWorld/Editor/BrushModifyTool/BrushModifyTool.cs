#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.GameObjectCollider.Editor;
using VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.Undo.Editor.GameObject;
using VladislavTsurikov.Undo.Editor.TerrainObjectRenderer;
using VladislavTsurikov.UnityUtility.Runtime;
using GameObjectUtility = VladislavTsurikov.UnityUtility.Runtime.GameObjectUtility;
using Instance = VladislavTsurikov.UnityUtility.Runtime.Instance;
using ToolsComponentStack = VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.ToolsComponentStack;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool
{
    [Name("Happy Artist/Brush Modify")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject) })]
    [AddToolComponents(new[] { typeof(BrushSettings), typeof(ModifyTransformSettings) })]
    [AddGroupComponents(new[] { typeof(PrototypeGameObject), typeof(PrototypeTerrainObject) },
        new[] { typeof(FilterSettings) })]
    public class BrushModifyTool : ToolWindow
    {
        private BrushSettings _brushSettings;
        private Dictionary<GameObject, ModifyInfo> _modifiedGameObjects = new();
#if RENDERER_STACK
        private Dictionary<TerrainObjectInstance, ModifyInfo> _modifiedTerrainObjects = new();
#endif

        private MouseMove _mouseMove = new();
        private int _updateTicks;

        protected override void OnEnable()
        {
            _modifiedGameObjects = new Dictionary<GameObject, ModifyInfo>();
#if RENDERER_STACK
            _modifiedTerrainObjects = new Dictionary<TerrainObjectInstance, ModifyInfo>();
#endif

            _brushSettings =
                (BrushSettings)ToolsComponentStack.GetElement(typeof(BrushModifyTool), typeof(BrushSettings));

            _mouseMove = new MouseMove();
            _mouseMove.OnMouseDown += OnMouseDown;
            _mouseMove.OnMouseUp += OnMouseUp;
            _mouseMove.OnMouseDrag += OnMouseDrag;
            _mouseMove.OnRepaint += OnRepaint;
        }

        protected override void DoTool()
        {
            _mouseMove.LookAtSize = _brushSettings.BrushSize;

            _mouseMove.Run();
        }

        protected override void HandleKeyboardEvents()
        {
            var brushSettings =
                (BrushSettings)ToolsComponentStack.GetElement(typeof(BrushModifyTool), typeof(BrushSettings));

            brushSettings.ScrollBrushRadiusEvent();
        }

        private void OnMouseDown() => _updateTicks = 0;

        private void OnMouseUp()
        {
            _modifiedGameObjects.Clear();
#if RENDERER_STACK
            _modifiedTerrainObjects.Clear();
#endif
        }

        private void OnMouseDrag(Vector3 dragPoint)
        {
            _updateTicks++;

            foreach (Group group in WindowData.Instance.SelectedData.SelectedGroupList)
            {
                BoxArea area = _brushSettings.GetAreaVariables(_mouseMove.Raycast);

                ModifyGroup(group, area);
            }
        }

        private void OnRepaint()
        {
            BoxArea area = _brushSettings.GetAreaVariables(_mouseMove.Raycast);
            BrushModifyToolVisualisation.Draw(area);
        }

        private void ModifyGroup(Group group, BoxArea area)
        {
            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                ModifyGameObject(group, area);
            }
#if RENDERER_STACK
            else if (group.PrototypeType == typeof(PrototypeTerrainObject))
            {
                ModifyTerrainObject(group, area);
            }
#endif
        }

#if RENDERER_STACK
        private void ModifyTerrainObject(Group group, BoxArea boxArea)
        {
            var filterSettings =
                (FilterSettings)group.GetElement(typeof(BrushModifyTool), typeof(FilterSettings));

            if (filterSettings.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterSettings.MaskFilterComponentSettings, boxArea);
            }

            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;
            var modifyTransformSettings =
                (ModifyTransformSettings)ToolsComponentStack.GetElement(typeof(BrushModifyTool),
                    typeof(ModifyTransformSettings));

            PrototypeTerrainObjectOverlap.OverlapBox(boxArea.Bounds, null, false, true,
                (proto, terrainObjectInstance) =>
                {
                    if (proto.Active == false || proto.Selected == false)
                    {
                        return true;
                    }

                    if (!_modifiedTerrainObjects.TryGetValue(terrainObjectInstance, out ModifyInfo modifyInfo))
                    {
                        Vector3 randomVector = Random.insideUnitSphere;

                        modifyInfo.RandomScale = 1f - Random.value;
                        modifyInfo.RandomPositionY = 1f - Random.value;
                        modifyInfo.RandomRotation = new Vector3(randomVector.x, randomVector.y, randomVector.z);

                        Undo.Editor.Undo.RegisterUndoAfterMouseUp(new TerrainObjectTransform(terrainObjectInstance));
                    }

                    float fitness = 1;
                    Vector3 checkPoint = terrainObjectInstance.Position;

                    switch (filterSettings.FilterType)
                    {
                        case FilterType.SimpleFilter:
                        {
                            if (Physics.Raycast(RayUtility.GetRayDown(checkPoint), out RaycastHit hit,
                                    PreferenceElementSingleton<RaycastPreferenceSettings>.Instance.MaxRayDistance,
                                    layerSettings.GetCurrentPaintLayers(group.PrototypeType)))
                            {
                                fitness = filterSettings.SimpleFilter.GetFitness(hit.point, hit.normal);
                            }

                            break;
                        }
                        case FilterType.MaskFilter:
                        {
                            if (filterSettings.MaskFilterComponentSettings.MaskFilterStack.ElementList.Count != 0)
                            {
                                fitness =
                                    TextureUtility.GetFromWorldPosition(boxArea.Bounds, checkPoint,
                                        filterSettings.MaskFilterComponentSettings.FilterMaskTexture2D);
                            }

                            break;
                        }
                    }

                    var maskFitness = TextureUtility.GetFromWorldPosition(boxArea.Bounds, checkPoint, boxArea.Mask);

                    fitness *= maskFitness;

                    if (modifyInfo.LastUpdate != _updateTicks)
                    {
                        modifyInfo.LastUpdate = _updateTicks;

                        var instance =
                            new Instance(terrainObjectInstance.Position, terrainObjectInstance.Scale,
                                terrainObjectInstance.Rotation);

                        var moveLenght = Event.current.delta.magnitude;

                        modifyTransformSettings.ModifyTransform(ref instance, ref modifyInfo, moveLenght,
                            _mouseMove.StrokeDirection, fitness, Vector3.up);

                        terrainObjectInstance.Position = instance.Position;
                        terrainObjectInstance.Rotation = instance.Rotation.normalized;
                        terrainObjectInstance.Scale = instance.Scale;
                    }

                    _modifiedTerrainObjects[terrainObjectInstance] = modifyInfo;

                    return true;
                });
        }
#endif

        private void ModifyGameObject(Group group, BoxArea boxArea)
        {
            var filterSettings = (FilterSettings)group.GetElement(typeof(BrushModifyTool), typeof(FilterSettings));

            if (filterSettings.FilterType == FilterType.MaskFilter)
            {
                FilterMaskOperation.UpdateMaskTexture(filterSettings.MaskFilterComponentSettings, boxArea);
            }

            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;
            var modifyTransformSettings =
                (ModifyTransformSettings)ToolsComponentStack.GetElement(typeof(BrushModifyTool),
                    typeof(ModifyTransformSettings));

            var modifyTransform = false;

            PrototypeGameObjectOverlap.OverlapBox(boxArea.Bounds, (proto, go) =>
            {
                if (proto == null || proto.Active == false || proto.Selected == false)
                {
                    return true;
                }

                GameObject prefabRoot = GameObjectUtility.GetPrefabRoot(go);
                if (prefabRoot == null)
                {
                    return true;
                }

                if (boxArea.Bounds.Contains(prefabRoot.transform.position) == false)
                {
                    return true;
                }

                if (!_modifiedGameObjects.TryGetValue(prefabRoot, out ModifyInfo modifyInfo))
                {
                    Vector3 randomVector = Random.insideUnitSphere;

                    modifyInfo.RandomScale = 1f - Random.value;
                    modifyInfo.RandomPositionY = 1f - Random.value;

                    modifyInfo.RandomRotation = new Vector3(randomVector.x, randomVector.y, randomVector.z);

                    Undo.Editor.Undo.RegisterUndoAfterMouseUp(new GameObjectTransform(go));
                }

                float fitness = 1;
                Vector3 checkPoint = prefabRoot.transform.position;

                switch (filterSettings.FilterType)
                {
                    case FilterType.SimpleFilter:
                    {
                        if (Physics.Raycast(RayUtility.GetRayDown(checkPoint), out RaycastHit hit,
                                PreferenceElementSingleton<RaycastPreferenceSettings>.Instance.MaxRayDistance,
                                layerSettings.GetCurrentPaintLayers(group.PrototypeType)))
                        {
                            fitness = filterSettings.SimpleFilter.GetFitness(hit.point, hit.normal);
                        }

                        break;
                    }
                    case FilterType.MaskFilter:
                    {
                        if (filterSettings.MaskFilterComponentSettings.MaskFilterStack.ElementList.Count != 0)
                        {
                            fitness = TextureUtility.GetFromWorldPosition(boxArea.Bounds, prefabRoot.transform.position,
                                filterSettings.MaskFilterComponentSettings.FilterMaskTexture2D);
                        }

                        break;
                    }
                }

                var maskFitness = TextureUtility.GetFromWorldPosition(boxArea.Bounds, checkPoint, boxArea.Mask);

                fitness *= maskFitness;

                if (modifyInfo.LastUpdate != _updateTicks)
                {
                    modifyInfo.LastUpdate = _updateTicks;

                    var instance = new Instance(prefabRoot.transform.position, prefabRoot.transform.localScale,
                        prefabRoot.transform.rotation);

                    var moveLenght = Event.current.delta.magnitude;

                    modifyTransformSettings.ModifyTransform(ref instance, ref modifyInfo, moveLenght,
                        _mouseMove.StrokeDirection, fitness, Vector3.up);

                    prefabRoot.transform.position = instance.Position;
                    prefabRoot.transform.rotation = instance.Rotation;
                    prefabRoot.transform.localScale = instance.Scale;

                    modifyTransform = true;
                }

                _modifiedGameObjects[prefabRoot] = modifyInfo;

                return true;
            });

            if (modifyTransform)
            {
                GameObjectColliderUtility.HandleTransformChangesForAllScenes();
            }
        }
    }
}
#endif
