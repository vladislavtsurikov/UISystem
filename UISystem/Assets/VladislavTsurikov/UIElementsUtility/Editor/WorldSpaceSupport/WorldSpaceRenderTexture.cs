#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.UIElementsUtility.Editor.WorldSpaceSupport
{
    [Serializable]
    public class WorldSpaceRenderTexture
    {
        [SerializeField]
        private RenderTexture _targetRenderTexture;

        public RenderTexture TargetRenderTexture => _targetRenderTexture;

        public void Setup(Vector2Int resolution) => CreateRenderTextureIfNecessary(resolution);

        public void RecreateRenderTexture(Vector2Int resolution)
        {
            if (_targetRenderTexture == null)
            {
                CreateRenderTextureIfNecessary(resolution);
                return;
            }

            var renderTexturePath = AssetDatabase.GetAssetPath(_targetRenderTexture);
            var renderTextureName = _targetRenderTexture.name;

            AssetDatabase.DeleteAsset(renderTexturePath);

            CreateRenderTextureIfNecessary(resolution, renderTextureName, renderTexturePath);
        }

        public bool CreateRenderTextureIfNecessary(Vector2Int resolution, string name = null, string path = null)
        {
            if (_targetRenderTexture != null)
            {
                return false;
            }

            _targetRenderTexture = new RenderTexture(resolution.x, resolution.y, 16)
            {
                name = name ?? "WorldSpaceUIRenderTexture"
            };

            if (path == null)
            {
                if (!AssetDatabase.IsValidFolder($"Assets/{EditorWorldSpaceUIDocumentSupport.AssetsFolderName}"))
                {
                    AssetDatabase.CreateFolder("Assets", EditorWorldSpaceUIDocumentSupport.AssetsFolderName);
                }

                var searchInFolders = new[] { $"Assets/{EditorWorldSpaceUIDocumentSupport.AssetsFolderName}" };

                var texturesGUIDs = AssetDatabase.FindAssets("t:RenderTexture", searchInFolders);

                AssetDatabase.CreateAsset(_targetRenderTexture,
                    $"Assets/{EditorWorldSpaceUIDocumentSupport.AssetsFolderName}/{_targetRenderTexture.name}{texturesGUIDs.Length + 1}.asset");
            }
            else
            {
                AssetDatabase.CreateAsset(_targetRenderTexture, path);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return true;
        }
    }
}
#endif
