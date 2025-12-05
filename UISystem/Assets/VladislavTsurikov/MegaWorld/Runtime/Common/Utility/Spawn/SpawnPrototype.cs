#if UNITY_2021_2_OR_NEWER
using UnityEngine.TerrainTools;
#else
using UnityEngine.Experimental.TerrainAPI;
#endif
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.Undo.Editor.TerrainObjectRenderer;
using VladislavTsurikov.UnityUtility.Runtime;
using GameObjectUtility = VladislavTsurikov.MegaWorld.Runtime.Core.Utility.GameObjectUtility;
using Instance = VladislavTsurikov.UnityUtility.Runtime.Instance;
using PrototypeTerrainObject =
    VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject.
    PrototypeTerrainObject;
#if UNITY_EDITOR
using VladislavTsurikov.Undo.Editor.GameObject;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Spawn
{
    public static class SpawnPrototype
    {
        public static void SpawnTerrainObject(PrototypeTerrainObject proto, RayHit rayHit, float fitness,
            bool supportUndo = false)
        {
#if RENDERER_STACK
            var overlapCheckSettings =
                (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

            var instance =
                new Instance(rayHit.Point, proto.Prefab.transform.lossyScale, proto.Prefab.transform.rotation);

            var transformComponentSettings =
                (TransformComponentSettings)proto.GetElement(typeof(TransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref instance, fitness,
                rayHit.Normal);

            if (OverlapCheckSettings.RunOverlapCheck(proto.GetType(), overlapCheckSettings, proto.Extents, instance))
            {
                TerrainObjectInstance terrainObjectInstance =
                    TerrainObjectRendererAPI.AddInstance(proto.RendererPrototype, instance.Position, instance.Scale,
                        instance.Rotation);

#if UNITY_EDITOR
                if (supportUndo)
                {
                    Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedTerrainObject(terrainObjectInstance));
                }
#endif
            }
#endif
        }

        public static void SpawnGameObject(Group group, PrototypeGameObject proto, RayHit rayHit, float fitness,
            bool supportUndo = false)
        {
            var overlapCheckSettings = (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

            var instance = new Instance(rayHit.Point, proto.Prefab.transform.lossyScale,
                proto.Prefab.transform.rotation);

            var transformComponentSettings =
                (TransformComponentSettings)proto.GetElement(typeof(TransformComponentSettings));
            transformComponentSettings.TransformComponentStack.ManipulateTransform(ref instance, fitness,
                rayHit.Normal);

            if (OverlapCheckSettings.RunOverlapCheck(proto.GetType(), overlapCheckSettings, proto.Extents, instance))
            {
                GameObject gameObject = GameObjectUtility.Instantiate(proto.Prefab, instance.Position, instance.Scale,
                    instance.Rotation);
                group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(gameObject);

#if UNITY_EDITOR
                GameObjectCollider.Editor.GameObjectCollider.RegisterGameObjectToCurrentScene(gameObject);

                if (supportUndo)
                {
                    Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(gameObject));
                }
#endif
                gameObject.transform.hasChanged = false;
            }
        }

        public static void SpawnTerrainDetails(PrototypeTerrainDetail proto, BoxArea boxArea, Terrain terrain)
        {
            TerrainData terrainData = terrain.terrainData;
            var spawnSize = new Vector2Int(
                UnityTerrainUtility.WorldToDetail(boxArea.Size.x, terrainData.size.x, terrainData),
                UnityTerrainUtility.WorldToDetail(boxArea.Size.z, terrainData.size.z, terrainData));

            Vector2Int halfBrushSize = spawnSize / 2;

            var center = new Vector2Int(
                UnityTerrainUtility.WorldToDetail(boxArea.Center.x - terrain.transform.position.x, terrain.terrainData),
                UnityTerrainUtility.WorldToDetail(boxArea.Center.z - terrain.transform.position.z,
                    terrain.terrainData.size.z, terrain.terrainData));

            Vector2Int position = center - halfBrushSize;
            var startPosition = Vector2Int.Max(position, Vector2Int.zero);

            Vector2Int offset = startPosition - position;

            float fitness = 1;
            float detailmapResolution = terrain.terrainData.detailResolution;

            var localData = terrain.terrainData.GetDetailLayer(
                startPosition.x, startPosition.y,
                Mathf.Max(0, Mathf.Min(position.x + spawnSize.x, (int)detailmapResolution) - startPosition.x),
                Mathf.Max(0, Mathf.Min(position.y + spawnSize.y, (int)detailmapResolution) - startPosition.y),
                proto.TerrainProtoId);

            float widthY = localData.GetLength(1);
            float heightX = localData.GetLength(0);

            var maskFilterComponentSettings =
                (MaskFilterComponentSettings)proto.GetElement(typeof(MaskFilterComponentSettings));
            var spawnDetailSettings = (SpawnDetailSettings)proto.GetElement(typeof(SpawnDetailSettings));

            for (var y = 0; y < widthY; y++)
            for (var x = 0; x < heightX; x++)
            {
                var current = new Vector2Int(y, x);

                Vector2 normal = current + startPosition;
                normal /= detailmapResolution;

                Vector2 worldPosition = UnityTerrainUtility.GetTerrainWorldPositionFromRange(normal, terrain);

                if (maskFilterComponentSettings.MaskFilterStack.ElementList.Count > 0)
                {
                    fitness = TextureUtility.GetFromWorldPosition(boxArea.Bounds,
                        new Vector3(worldPosition.x, 0, worldPosition.y),
                        maskFilterComponentSettings.FilterMaskTexture2D);
                }

                var maskFitness = boxArea.GetAlpha(current + offset, spawnSize);

                int targetStrength;

                if (spawnDetailSettings.UseRandomOpacity)
                {
                    var randomFitness = fitness;
                    randomFitness *= Random.value;

                    targetStrength = Mathf.RoundToInt(Mathf.Lerp(0, spawnDetailSettings.Density, randomFitness));
                }
                else
                {
                    targetStrength = Mathf.RoundToInt(Mathf.Lerp(0, spawnDetailSettings.Density, fitness));
                }

                targetStrength = Mathf.RoundToInt(Mathf.Lerp(localData[x, y], targetStrength, maskFitness));

                if (Random.Range(0f, 1f) < 1 - fitness || Random.Range(0f, 1f) < spawnDetailSettings.FailureRate / 100)
                {
                    targetStrength = 0;
                }

                if (Random.Range(0f, 1f) < 1 - maskFitness)
                {
                    targetStrength = localData[x, y];
                }

                localData[x, y] = targetStrength;
            }

            terrain.terrainData.SetDetailLayer(startPosition.x, startPosition.y, proto.TerrainProtoId, localData);
        }

        public static void SpawnTexture(PrototypeTerrainTexture proto,
            TerrainPainterRenderHelper terrainPainterRenderHelper, float textureTargetStrength)
        {
            var maskFilterComponentSettings =
                (MaskFilterComponentSettings)proto.GetElement(typeof(MaskFilterComponentSettings));

            PaintContext paintContext = terrainPainterRenderHelper.AcquireTexture(proto.TerrainLayer);

            if (paintContext != null)
            {
                FilterMaskOperation.UpdateFilterContext(ref maskFilterComponentSettings.FilterContext,
                    maskFilterComponentSettings.MaskFilterStack, terrainPainterRenderHelper.BoxArea);

                RenderTexture filterMaskRT = maskFilterComponentSettings.FilterContext.Output;

                Material mat = MaskFilterUtility.GetPaintMaterial();

                // apply brush
                var targetAlpha = textureTargetStrength;
                float s = 1;
                var brushParams = new Vector4(s, targetAlpha, 0.0f, 0.0f);
                mat.SetTexture("_BrushTex", terrainPainterRenderHelper.BoxArea.Mask);
                mat.SetVector("_BrushParams", brushParams);
                mat.SetTexture("_FilterTex", filterMaskRT);
                mat.SetTexture("_SourceAlphamapRenderTextureTex", paintContext.sourceRenderTexture);

                terrainPainterRenderHelper.SetupTerrainToolMaterialProperties(paintContext, mat);

                terrainPainterRenderHelper.RenderBrush(paintContext, mat, 0);

                TerrainPaintUtility.EndPaintTexture(paintContext, "Terrain Paint - Texture");

                if (maskFilterComponentSettings.FilterContext != null)
                {
                    maskFilterComponentSettings.FilterContext.Dispose();
                }

                TerrainPaintUtility.ReleaseContextResources(paintContext);
            }
        }
    }
}
