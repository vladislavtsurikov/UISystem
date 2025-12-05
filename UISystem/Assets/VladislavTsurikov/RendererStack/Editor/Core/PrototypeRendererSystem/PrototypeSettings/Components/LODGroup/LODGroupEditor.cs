#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;

namespace VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.PrototypeSettings.LODGroup
{
    [ElementEditor(typeof(Runtime.Core.PrototypeRendererSystem.PrototypeSettings.LODGroup))]
    public class LODGroupEditor : PrototypeComponentEditor
    {
        private Runtime.Core.PrototypeRendererSystem.PrototypeSettings.LODGroup _lodGroup;

        public GUIContent LODFade = new("LOD Fade",
            "Enables fade style blending between the LOD levels of this prototype. This can have a minor impact on performance since during fading, both LOD levels will be rendering.");

        public GUIContent LODFadeForLastLOD = new("LOD Fade For Last LOD",
            "Allows you to use LOD Fade for the last LOD. This allows you to slightly increase FPS because LOD Fade draws additional meshes for other LODs and use LOD Fade for the very first LOD in some cases is not reputed.");

        public GUIContent LODFadeTransitionDistance = new("LOD Fade Transition Distance",
            "This allows you to adjust the distance where the LOD Fade will end.");

        public override void OnEnable() =>
            _lodGroup = (Runtime.Core.PrototypeRendererSystem.PrototypeSettings.LODGroup)Target;

        public override void OnGUI(Rect rect, int index)
        {
            if (Prototype.RenderModel.LODs.Count < 2)
            {
                CustomEditorGUI.WarningBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "Missing LOD Group in prefab.");
                rect.y += CustomEditorGUI.SingleLineHeight;

                return;
            }

            _lodGroup.SetLODFade(Prototype.RenderModel,
                CustomEditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), LODFade,
                    _lodGroup.EnabledLODFade));
            rect.y += CustomEditorGUI.SingleLineHeight;

            if (_lodGroup.EnabledLODFade)
            {
                _lodGroup.LodFadeForLastLOD = CustomEditorGUI.Toggle(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), LODFadeForLastLOD,
                    _lodGroup.LodFadeForLastLOD);
                rect.y += CustomEditorGUI.SingleLineHeight;
                _lodGroup.LodFadeTransitionDistance = CustomEditorGUI.Slider(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    LODFadeTransitionDistance, _lodGroup.LodFadeTransitionDistance, 0f, 20f);
                rect.y += CustomEditorGUI.SingleLineHeight;
            }

            if (_lodGroup.EnabledLODFade)
            {
                CustomEditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "Your shader must have the \"LOD_FADE_CROSSFADE\" keyword and also the LOD Fade algorithm in the shader. You also need the unity_LODFade variable.");
                rect.y += CustomEditorGUI.SingleLineHeight;
            }

            CustomEditorGUI.Label(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                "Number of LODs: " + Prototype.RenderModel.LODs.Count);
            rect.y += CustomEditorGUI.SingleLineHeight;

            _lodGroup.LODDistanceRandomOffset = Mathf.Max(0, CustomEditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("LOD Distance Random Offset"), _lodGroup.LODDistanceRandomOffset));
            rect.y += CustomEditorGUI.SingleLineHeight;

            _lodGroup.LODBias =
                CustomEditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("LOD Bias"), _lodGroup.LODBias, 0.1f, 5f);
            rect.y += CustomEditorGUI.SingleLineHeight;

            // LODEditorUtility.DrawLODSettingsStack(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), RendererType, Prototype, Prototype.RenderModel);
            // rect.y += CustomEditorGUI.SingleLineHeight * 2;
            //
            // if(QualitySettings.lodBias != 1)
            // {
            // 	EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),"Active QualitySettings.lodBias is " + QualitySettings.lodBias.ToString("F2") + ". Distances are adjusted accordingly.", MessageType.Warning);
            // 	rect.y += CustomEditorGUI.SingleLineHeight;
            // }
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            if (Prototype.RenderModel.LODs.Count < 2)
            {
                height += CustomEditorGUI.SingleLineHeight;

                return height;
            }

            height += CustomEditorGUI.SingleLineHeight;

            if (_lodGroup.EnabledLODFade)
            {
                height += CustomEditorGUI.SingleLineHeight;
                height += CustomEditorGUI.SingleLineHeight;
            }

            if (_lodGroup.EnabledLODFade)
            {
                height += CustomEditorGUI.SingleLineHeight;
            }

            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;

            // height += CustomEditorGUI.SingleLineHeight * 2;
            //
            // if(QualitySettings.lodBias != 1)
            // {
            // 	height += CustomEditorGUI.SingleLineHeight;
            // }

            return height;
        }
    }
}
#endif
