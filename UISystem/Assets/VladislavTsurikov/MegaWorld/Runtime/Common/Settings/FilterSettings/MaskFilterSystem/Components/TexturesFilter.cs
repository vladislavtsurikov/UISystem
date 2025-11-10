#if UNITY_EDITOR
#endif
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.TerrainTools;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

#if UNITY_2021_2_OR_NEWER
#else
using UnityEngine.Experimental.TerrainAPI;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    [Serializable]
    [Name("Textures")]
    public class TexturesFilter : MaskFilter
    {
        public BlendMode BlendMode = BlendMode.Multiply;
        public List<TerrainTexture> TerrainTextureList = new();

        [OnDeserializing]
        public void Init()
        {
            if (TerrainTextureList == null)
            {
                TerrainTextureList = new List<TerrainTexture>();
            }
        }

        public override void Eval(MaskFilterContext maskFilterContext, int index)
        {
            if (Terrain.activeTerrain == null)
            {
                return;
            }

            Terrain terrain = maskFilterContext.BoxArea.TerrainUnder;

            Vector2 currUV = UnityTerrainUtility.WorldPointToUV(maskFilterContext.BoxArea.Center, terrain);

            BrushTransform brushTransform =
                TerrainPaintUtility.CalculateBrushTransform(terrain, currUV, maskFilterContext.BoxArea.BoxSize, 0.0f);
            Rect brushRect = brushTransform.GetBrushXYBounds();

            var addTexturesToRenderTextureList = new List<TerrainTexture>();

            if (IsSyncTerrain(Terrain.activeTerrain) == false)
            {
                UpdateCheckTerrainTextures(Terrain.activeTerrain);
            }

            for (var i = 0; i < TerrainTextureList.Count; i++)
            {
                if (TerrainTextureList[i].Selected)
                {
                    addTexturesToRenderTextureList.Add(TerrainTextureList[i]);
                }
            }

            Material blendMat = MaskFilterUtility.blendModesMaterial;

            var output = RenderTexture.GetTemporary(maskFilterContext.SourceRenderTexture.width,
                maskFilterContext.SourceRenderTexture.height, maskFilterContext.SourceRenderTexture.depth,
                GraphicsFormat.R16_SFloat);
            output.enableRandomWrite = true;

            foreach (TerrainTexture terrainTexture in addTexturesToRenderTextureList)
            {
                var localSourceRender = RenderTexture.GetTemporary(maskFilterContext.SourceRenderTexture.width,
                    maskFilterContext.SourceRenderTexture.height, 1, RenderTextureFormat.ARGB32);
                localSourceRender.enableRandomWrite = true;

                PaintContext localTextureContext = TerrainPaintUtility.BeginPaintTexture(terrain, brushRect,
                    Terrain.activeTerrain.terrainData.terrainLayers[terrainTexture.TerrainProtoId]);

                blendMat.SetInt("_BlendMode", (int)BlendMode.Add);
                blendMat.SetTexture("_MainTex", output);
                blendMat.SetTexture("_BlendTex", localTextureContext.sourceRenderTexture);

                Graphics.Blit(output, localSourceRender, blendMat, 0);
                Graphics.Blit(localSourceRender, output);

                TerrainPaintUtility.ReleaseContextResources(localTextureContext);

                RenderTexture.ReleaseTemporary(localSourceRender);
            }

            var sourceRender = RenderTexture.GetTemporary(maskFilterContext.SourceRenderTexture.width,
                maskFilterContext.SourceRenderTexture.height, 1, RenderTextureFormat.ARGB32);
            sourceRender.enableRandomWrite = true;

            if (index == 0)
            {
                blendMat.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                blendMat.SetInt("_BlendMode", (int)BlendMode);
            }

            blendMat.SetTexture("_MainTex", output);
            blendMat.SetTexture("_BlendTex", maskFilterContext.SourceRenderTexture);

            Graphics.Blit(output, maskFilterContext.DestinationRenderTexture, blendMat, 0);

            RenderTexture.ReleaseTemporary(output);
            RenderTexture.ReleaseTemporary(sourceRender);
        }

        public bool IsSyncTerrain(Terrain terrain)
        {
            foreach (TerrainLayer layer in terrain.terrainData.terrainLayers)
            {
                if (layer == null)
                {
                    continue;
                }

                var find = false;

                for (var i = 0; i < TerrainTextureList.Count; i++)
                {
                    if (TextureUtility.IsSameTexture(layer.diffuseTexture, TerrainTextureList[i].Texture))
                    {
                        find = true;
                        break;
                    }
                }

                if (find == false)
                {
                    return false;
                }
            }

            return true;
        }

        public void UpdateCheckTerrainTextures(Terrain activeTerrain)
        {
            if (activeTerrain == null)
            {
                Debug.LogWarning(
                    "Can not update prototypes from the terrain as there is no terrain currently active in this scene.");
                return;
            }

            int idx;

            TerrainTextureList.Clear();

            TerrainLayer[] terrainLayers = activeTerrain.terrainData.terrainLayers;

            for (idx = 0; idx < terrainLayers.Length; idx++)
            {
                if (terrainLayers[idx] == null)
                {
                    continue;
                }

                var checkTerrainTextures = new TerrainTexture
                {
                    Texture = terrainLayers[idx].diffuseTexture, TerrainProtoId = idx
                };

                TerrainTextureList.Add(checkTerrainTextures);
            }
        }
    }
}
