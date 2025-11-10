#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.RendererStack.Runtime.Common.GlobalSettings;

namespace VladislavTsurikov.RendererStack.Editor.Common.GlobalSettings
{
    [ElementEditor(typeof(Quality))]
    public class QualityEditor : IMGUIElementEditor
    {
        private Quality _quality;

        public GUIContent IsShadowCasting = new("Is Shadow Casting",
            "Sets whether to cast shadows from objects. If this parameter is disabled, then you can increase the optimization");

        public GUIContent LODBias = new("LOD Bias",
            "This value effects the LOD level distances per prototype. When it is set to a value less than 1, it favors less detail. A value of more than 1 favors greater detail. You can use this to manipulate the instance LOD distances without changing LOD Group on the original prefab.");

        public GUIContent MaxRenderDistance = new("Max Render Distance",
            "Sets the maximum distance from the camera that prototypes will be rendered. This setting sets the render distance limit for all prototypes.");

        public override void OnEnable() => _quality = (Quality)Target;

        public override void OnGUI()
        {
            _quality.IsShadowCasting = CustomEditorGUILayout.Toggle(IsShadowCasting, _quality.IsShadowCasting);
            _quality.MaxRenderDistance =
                CustomEditorGUILayout.FloatField(MaxRenderDistance, _quality.MaxRenderDistance);
            _quality.LODBias = CustomEditorGUILayout.Slider(LODBias, _quality.LODBias, 0.1f, 5);
        }
    }
}
#endif
