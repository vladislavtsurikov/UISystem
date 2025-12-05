using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.RendererStack.Runtime.Core.Utility;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Core.RenderManager.GPUInstancedIndirect
{
    [LocationAsset("RendererStack/GPUInstancedIndirectShaderStack")]
    public class GPUInstancedIndirectShaderStack : SerializedScriptableObjectSingleton<GPUInstancedIndirectShaderStack>
    {
        public List<GPUInstancedIndirectShader> GPUInstancedIndirectShaderList = new();

        public Shader GetShader(string shaderName)
        {
            foreach (GPUInstancedIndirectShader item in GPUInstancedIndirectShaderList)
            {
                if (item.OriginalName.Equals(shaderName))
                {
                    return item.Shader;
                }
            }

            if (UnityBuiltInSupportShaders.StandardUnityShaders.Contains(shaderName))
            {
                return Shader.Find(
                    UnityBuiltInSupportShaders.StandardUnityShadersGPUI[
                        UnityBuiltInSupportShaders.StandardUnityShaders.IndexOf(shaderName)]);
            }

            if (!shaderName.Equals(UnityBuiltInSupportShaders.ShaderUnityStandard))
            {
                if (Application.isPlaying)
                {
                    Debug.LogWarning("Can not find instanced shader for : " + shaderName +
                                     ". Using Standard shader instead.");
                }

                return GetShader(UnityBuiltInSupportShaders.ShaderUnityStandard);
            }

            Debug.LogWarning("Can not find GPU Instanced Indirect shader for : " + shaderName);
            return null;
        }

        public void ResetShaders()
        {
            GPUInstancedIndirectShaderList.Clear();

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public void ClearEmptyShaders()
        {
#if UNITY_EDITOR
            if (GPUInstancedIndirectShaderList.RemoveAll(si =>
                    si == null || si.Shader == null || string.IsNullOrEmpty(si.OriginalName)) >
                0)
            {
                EditorUtility.SetDirty(this);
            }
#endif
        }

        public void AddShaderInstance(string name, Shader instancedShader, bool isOriginalInstanced = false)
        {
            GPUInstancedIndirectShaderList.Add(new GPUInstancedIndirectShader(name, instancedShader,
                isOriginalInstanced));
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public bool IsShadersInstancedVersionExists(string shaderName)
        {
            if (UnityBuiltInSupportShaders.StandardUnityShaders.Contains(shaderName))
            {
                return true;
            }

            foreach (GPUInstancedIndirectShader item in GPUInstancedIndirectShaderList)
            {
                if (item.OriginalName.Equals(shaderName))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsShaderOriginalInstanced(string shaderName)
        {
            foreach (GPUInstancedIndirectShader item in GPUInstancedIndirectShaderList)
            {
                if (item.OriginalName.Equals(shaderName))
                {
                    if (item.IsOriginal)
                    {
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

        public Material GetMaterial(Material originalMaterial)
        {
            if (originalMaterial == null || originalMaterial.shader == null)
            {
                Debug.LogError(
                    "One of the prototypes is missing material reference! Check the Material references in MeshRenderer.");

                return new Material(GetShader(RendererStackConstants.ShaderUnityInternalError));
            }

            if (IsShaderOriginalInstanced(originalMaterial.shader.name))
            {
                return originalMaterial;
            }

            var instancedMaterial = new Material(GetShader(originalMaterial.shader.name));
            instancedMaterial.CopyPropertiesFromMaterial(originalMaterial);
            instancedMaterial.name = originalMaterial.name + "_GPUInstancedIndirect";
            instancedMaterial.enableInstancing = true;

            return instancedMaterial;
        }

        public void RegenerateShaders()
        {
            foreach (GPUInstancedIndirectShader item in GPUInstancedIndirectShaderList)
            {
                Regenerate(item);
            }
        }

        public void Regenerate(GPUInstancedIndirectShader instancedIndirectShader)
        {
#if UNITY_EDITOR
            if (instancedIndirectShader.IsOriginal)
            {
                instancedIndirectShader.Shader =
                    GenerateGPUInstancedIndirectShader.Create(instancedIndirectShader.Shader, true);
                return;
            }

            var originalShader = Shader.Find(instancedIndirectShader.OriginalName);
            if (originalShader != null)
            {
                instancedIndirectShader.Shader = GenerateGPUInstancedIndirectShader.Create(originalShader);
            }
#endif
        }
    }
}
