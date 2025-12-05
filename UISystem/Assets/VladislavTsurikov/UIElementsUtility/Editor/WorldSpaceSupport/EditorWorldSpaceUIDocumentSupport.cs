#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using VladislavTsurikov.UIElementsUtility.Runtime.WorldSpaceSupport;

namespace VladislavTsurikov.UIElementsUtility.Editor.WorldSpaceSupport
{
    [Serializable]
    public class EditorWorldSpaceUIDocumentSupport
    {
        private const float DelayBeforeUpdateRenderTextureResolution = 0.1f;

        public const string AssetsFolderName = "WorldSpaceUI";

        private static readonly int _mainTex = Shader.PropertyToID("_MainTex");

        [HideInInspector]
        [SerializeField]
        private UIDocument _uiDocument;

        [SerializeField]
        private WorldSpacePanelSettings _worldSpacePanelSettings = new();

        [SerializeField]
        private WorldSpaceRenderTexture _worldSpaceRenderTexture = new();

        [SerializeField]
        private Vector2Int _resolution;

        private Coroutine _delayedUpdateRenderTextureResolutionCoroutine;
        private MeshCollider _meshCollider;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private WorldSpaceUIDocument _worldSpaceUIDocument;

        public Vector2Int Resolution
        {
            get => _resolution;
            set
            {
                if (_resolution == value)
                {
                    return;
                }

                _resolution = value;

                if (_delayedUpdateRenderTextureResolutionCoroutine != null)
                {
                    _worldSpaceUIDocument.StopCoroutine(_delayedUpdateRenderTextureResolutionCoroutine);
                }

                _delayedUpdateRenderTextureResolutionCoroutine =
                    _worldSpaceUIDocument.StartCoroutine(DelayedUpdateRenderTextureResolution());
            }
        }

        public void Setup(WorldSpaceUIDocument worldSpaceUIDocument, UIDocument uiDocument)
        {
            _worldSpaceUIDocument = worldSpaceUIDocument;
            _uiDocument = uiDocument;

            if (_resolution == Vector2Int.zero)
            {
                Vector3 localScale = worldSpaceUIDocument.transform.localScale;
                _resolution = new Vector2Int((int)(localScale.x * 100), (int)(localScale.y * 100));
            }

            SetupAssets();
            SetupMeshComponents();
        }

        private void SetupMeshComponents()
        {
            _meshCollider = _worldSpaceUIDocument.GetComponent<MeshCollider>();
            _meshFilter = _worldSpaceUIDocument.GetComponent<MeshFilter>();
            _meshRenderer = _worldSpaceUIDocument.GetComponent<MeshRenderer>();

            Mesh mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
            var material = new Material(Shader.Find("Unlit/Transparent"));

            _meshCollider.sharedMesh = mesh;
            _meshFilter.mesh = mesh;

            material.SetTexture(_mainTex, _worldSpaceRenderTexture.TargetRenderTexture);
            _meshRenderer.sharedMaterial = material;
            _meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        }

        private void SetupAssets()
        {
            _worldSpaceRenderTexture.Setup(_resolution);
            _worldSpacePanelSettings.Setup(_uiDocument, _resolution, _worldSpaceRenderTexture.TargetRenderTexture);
        }

        private IEnumerator DelayedUpdateRenderTextureResolution()
        {
            yield return new WaitForSecondsRealtime(DelayBeforeUpdateRenderTextureResolution);

            if (_uiDocument != null && _uiDocument.panelSettings != null)
            {
                _uiDocument.panelSettings.referenceResolution = _resolution;
            }

            _worldSpaceRenderTexture.RecreateRenderTexture(_resolution);

            if (!_worldSpacePanelSettings.CreatePanelSettingsIfNecessary())
            {
                _worldSpacePanelSettings.PanelSettings.UpdatePanelSettingsToWorldSpaceSupport(_resolution,
                    _worldSpaceRenderTexture.TargetRenderTexture);
            }

            if (_meshRenderer != null && _meshRenderer.sharedMaterial != null)
            {
                _meshRenderer.sharedMaterial.SetTexture(_mainTex, _worldSpaceRenderTexture.TargetRenderTexture);
            }
        }
    }
}
#endif
