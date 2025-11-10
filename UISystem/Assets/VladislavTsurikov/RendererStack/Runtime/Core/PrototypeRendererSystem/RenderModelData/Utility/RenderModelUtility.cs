using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.RendererStack.Runtime.Core.Preferences;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.Console;
using VladislavTsurikov.RendererStack.Runtime.Core.RenderManager.GPUInstancedIndirect;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData.Utility
{
    public static class RenderModelUtility
    {
        public static RenderModel GetRenderModel(Prototype prototype, GameObject prefab)
        {
            prototype.PrototypeConsole.Clear();

            if (prefab == null)
            {
                PrototypeConsole.Log(prototype, new PrototypeLog("Prefab is null", "The prefab is not set."));
                return null;
            }

            if (prefab.transform.position != Vector3.zero)
            {
                PrototypeConsole.Log(prototype,
                    new PrototypeLog("Prefab root position is not at (0,0,0)",
                        "If it is not at 0,0,0 then make sure that you deliberately set the offset.", false));
            }

            var renderModel = new RenderModel(prefab);

            LODGroup lodGroup = renderModel.Prefab.GetComponent<LODGroup>();

            if (lodGroup != null)
            {
                renderModel.MultiplySize = renderModel.Prefab.transform.localScale;

                UnityEngine.LOD[] loDs = lodGroup.GetLODs();

                foreach (UnityEngine.LOD lod in loDs)
                {
                    if (lod.renderers == null || lod.renderers.Length == 0)
                    {
                        PrototypeConsole.Log(prototype,
                            new PrototypeLog("LOD does not have any renderers",
                                "One or more LODs in the prefab's LOD Group do have contain any mesh renderers. Either assign a mesh renderer to the LOD, or remove the lOD."));
                        return null;
                    }

                    if (lod.renderers.Length > 1)
                    {
                        PrototypeConsole.Log(prototype,
                            new PrototypeLog("LOD has more than one Renderer",
                                "No support for multiple Renderer in LOD, please make sure there is one."));

                        return null;
                    }

                    Renderer renderer = lod.renderers[0];

                    if (renderer == null)
                    {
                        PrototypeConsole.Log(prototype,
                            new PrototypeLog("Renderer in LOD is not assigned",
                                "One or more LODs in the prefab's LOD Group do have contain any mesh renderers. Either assign a mesh renderer to the LOD, or remove the lOD."));

                        return null;
                    }

                    AddLod(prototype, renderModel, renderer);
                }
            }
            else
            {
                MeshRenderer meshRenderer = renderModel.Prefab.GetComponentInChildren<MeshRenderer>();

                renderModel.MultiplySize = meshRenderer.gameObject.transform.localScale;

                AddLod(prototype, renderModel, meshRenderer);
            }

            renderModel.BoundingSphereRadius =
                GameObjectUtility.CalculateBoundsInstantiate(renderModel.Prefab).extents.magnitude;

            return renderModel;
        }

        private static void AddLod(Prototype prototype, RenderModel renderModel, Renderer renderer)
        {
            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();

            if (meshFilter == null)
            {
                PrototypeConsole.Log(prototype,
                    new PrototypeLog("Mesh Renderer does not have a Mesh Filter component",
                        "A Mesh Renderer component in the prefab does not have a Mesh Filter component on the same object. The Mesh Filter object contains the mesh that needs to be rendered and is required."));
                return;
            }

            Mesh mesh = renderer.GetComponent<MeshFilter>().sharedMesh;

            Material[] materials = renderer.sharedMaterials;

            if (mesh == null)
            {
                PrototypeConsole.Log(prototype,
                    new PrototypeLog("Mesh is null",
                        "A Mesh Renderer component in the prefab does not have a mesh assigned."));
                return;
            }

            if (materials == null || materials.Length == 0)
            {
                PrototypeConsole.Log(prototype,
                    new PrototypeLog("Renderer does not have any materials",
                        "A Mesh Renderer component in the prefab does not have any materials assigned."));
                return;
            }

            if (materials.Length < mesh.subMeshCount)
            {
                PrototypeConsole.Log(prototype,
                    new PrototypeLog("Renderer does not have a material for each sub mesh",
                        "The number of materials in the mesh is less than the number of submeshes. Each submesh in a mesh requires its own material."));

                return;
            }

            if (materials.Length > mesh.subMeshCount)
            {
                PrototypeConsole.Log(prototype,
                    new PrototypeLog("Renderer has more materials than sub meshes",
                        "The number of materials in the mesh is greater than the number of submeshes. Each submesh in a mesh requires its own material."));

                return;
            }

            foreach (Material mat in materials)
            {
                if (mat == null)
                {
                    PrototypeConsole.Log(prototype,
                        new PrototypeLog("Material for renderer is not assigned",
                            "A Mesh Renderer component in the prefab has a material that is not assigned."));

                    return;
                }

                if (mat.shader == null)
                {
                    PrototypeConsole.Log(prototype,
                        new PrototypeLog("Material does not have a shader",
                            "A Mesh Renderer component in the prefab has a material that does not have a valid shader."));

                    return;
                }
            }

            List<Material> instancedMaterials = CreateMaterialsWithGPUInstancedIndirect(renderer.sharedMaterials);

            var mpb = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(mpb);
            var shadowMpb = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(shadowMpb);

            var lod = new LOD
            {
                Mesh = mesh,
                Materials = instancedMaterials,
                MaterialPropertyBlock = mpb,
                ShadowMaterialPropertyBlock = shadowMpb
            };

            if (renderModel.LODs.Count == 0)
            {
                lod.Distance = 0;
            }
            else
            {
                lod.Distance = GetLODDistance(renderModel.Prefab, renderModel.LODs.Count - 1);
            }

            renderModel.LODs.Add(lod);
        }

        private static float GetLODDistance(GameObject rootModel, int lodIndex)
        {
            LODGroup lodGroup = rootModel.GetComponentInChildren<LODGroup>();
            if (lodGroup)
            {
                UnityEngine.LOD[] lods = lodGroup.GetLODs();
                if (lodIndex >= 0 && lodIndex < lods.Length)
                {
                    return lodGroup.size / lods[lodIndex].screenRelativeTransitionHeight;
                }
            }

            return -1;
        }

        private static List<Material> CreateMaterialsWithGPUInstancedIndirect(Material[] sharedMaterials)
        {
            var materialList = new List<Material>();
            for (var i = 0; i < sharedMaterials.Length; i++)
            {
                Material mat = sharedMaterials[i];

                if (RendererStackSettings.Instance.AutoShaderConversion)
                {
                    mat = GPUInstancedIndirectShaderStack.Instance.GetMaterial(sharedMaterials[i]);
                }

                mat.EnableKeyword("LOD_FADE_CROSSFADE");

                materialList.Add(mat);
            }

            return materialList;
        }
    }
}
