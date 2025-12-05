#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UIElementsUtility.Editor.WorldSpaceSupport
{
    [Serializable]
    public class WorldSpacePanelSettings
    {
        [SerializeField]
        private PanelSettings _panelSettings;

        public PanelSettings PanelSettings => _panelSettings;

        public void Setup(UIDocument uiDocument, Vector2Int referenceResolution, RenderTexture targetRenderTexture)
        {
            if (uiDocument.panelSettings != null)
            {
                _panelSettings = uiDocument.panelSettings;
            }
            else if (CreatePanelSettingsIfNecessary())
            {
                uiDocument.panelSettings = _panelSettings;
            }

            _panelSettings.UpdatePanelSettingsToWorldSpaceSupport(referenceResolution, targetRenderTexture);
        }

        public bool CreatePanelSettingsIfNecessary()
        {
            if (_panelSettings != null)
            {
                return false;
            }

            if (!AssetDatabase.IsValidFolder($"Assets/{EditorWorldSpaceUIDocumentSupport.AssetsFolderName}"))
            {
                AssetDatabase.CreateFolder("Assets", EditorWorldSpaceUIDocumentSupport.AssetsFolderName);
            }

            var searchInFolders = new[] { $"Assets/{EditorWorldSpaceUIDocumentSupport.AssetsFolderName}" };

            _panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            _panelSettings.name = "WorldSpaceUIPanelSettings";

            var panelSettingsGUIDs = AssetDatabase.FindAssets("t:PanelSettings", searchInFolders);

            AssetDatabase.CreateAsset(_panelSettings,
                $"Assets/{EditorWorldSpaceUIDocumentSupport.AssetsFolderName}/{_panelSettings.name}{panelSettingsGUIDs.Length + 1}.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return true;
        }
    }

    public static class PanelSettingsExtension
    {
        public static void UpdatePanelSettingsToWorldSpaceSupport(this PanelSettings panelSettings,
            Vector2Int referenceResolution, RenderTexture targetRenderTexture)
        {
            panelSettings.targetTexture = targetRenderTexture;
            panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            panelSettings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            panelSettings.referenceResolution = referenceResolution;
            panelSettings.match = 0.5f;
            panelSettings.clearColor = true;
        }
    }
}
#endif
