#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.SceneSettings.Camera;

namespace VladislavTsurikov.RendererStack.Editor.TerrainObjectRenderer.SceneSettings.Camera
{
    [ElementEditor(typeof(TerrainObjectRendererCameraSettings))]
    public class TerrainObjectRendererCameraSettingsEditor : ReorderableListComponentEditor
    {
        public GUIContent CameraCullingMode = new("Camera Culling Mode",
            "Camera culling mode will decide how the camera does culling of objects.");

        public GUIContent LODBias = new("LOD Bias",
            "This value effects the LOD level distances per prototype. When it is set to a value less than 1, it favors less detail. A value of more than 1 favors greater detail. You can use this to manipulate the instance LOD distances without changing LOD Group on the original prefab.");

        public TerrainObjectRendererCameraSettings TerrainObjectRendererCameraSettings;

        public override void OnEnable() =>
            TerrainObjectRendererCameraSettings = (TerrainObjectRendererCameraSettings)Target;

        public override void OnGUI(Rect rect, int index)
        {
            var cameraManager =
                (CameraManager)RendererStackManager.Instance.SceneComponentStack.GetElement(typeof(CameraManager));

            TerrainObjectRendererCameraSettings.LodBias = CustomEditorGUI.Slider(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), LODBias,
                TerrainObjectRendererCameraSettings.LodBias, 0.1f, 1);
            rect.y += CustomEditorGUI.SingleLineHeight;
            TerrainObjectRendererCameraSettings.CameraCullingMode = (CameraCullingMode)CustomEditorGUI.EnumPopup(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), CameraCullingMode,
                TerrainObjectRendererCameraSettings.CameraCullingMode);
            rect.y += CustomEditorGUI.SingleLineHeight;

            if (cameraManager.IsMultipleCameras())
            {
                if (cameraManager.VirtualCameraList.Count > 2)
                {
                    TerrainObjectRendererCameraSettings.EnableColliders = CustomEditorGUI.Toggle(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Enable Colliders"), TerrainObjectRendererCameraSettings.EnableColliders);
                    rect.y += CustomEditorGUI.SingleLineHeight;
                }
            }
        }

        public override float GetElementHeight(int index)
        {
            var cameraManager =
                (CameraManager)RendererStackManager.Instance.SceneComponentStack.GetElement(typeof(CameraManager));

            float height = 0;

            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;

            if (cameraManager.IsMultipleCameras())
            {
                if (cameraManager.VirtualCameraList.Count > 2)
                {
                    height += CustomEditorGUI.SingleLineHeight;
                }
            }

            return height;
        }
    }
}
#endif
