#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.SceneSettings;

namespace VladislavTsurikov.RendererStack.Editor.TerrainObjectRenderer.SceneSettings
{
    [ElementEditor(typeof(Quality))]
    public class QualityEditor : IMGUIElementEditor
    {
        private Quality _quality;

        public GUIContent DirectionalLight = new("Directional Light",
            "This parameter is needed to know the direction of the sun's shadows, this allows only visible sun shadows to be displayed.");

        public GUIContent TransformOfFloatingOrigin = new("Transform of Floating Origin",
            "When using floating origin Renderer needs to know what object defines the root of the world. Renderer calculates an offset using this transform and applies it in the renderloop at no extra render cost.");

        public override void OnEnable() => _quality = (Quality)Target;

        public override void OnGUI()
        {
            _quality.TransformOfFloatingOrigin = (Transform)CustomEditorGUILayout.ObjectField(TransformOfFloatingOrigin,
                _quality.TransformOfFloatingOrigin, typeof(Transform));

            if (_quality.DirectionalLight == null)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                    if (CustomEditorGUILayout.ClickButton("Find Directional Light", ButtonStyle.Add))
                    {
                        _quality.FindDirectionalLight();
                    }

                    GUILayout.Space(5);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(3);

                _quality.DirectionalLight = (Light)CustomEditorGUILayout.ObjectField(DirectionalLight,
                    _quality.DirectionalLight == null,
                    _quality.DirectionalLight, typeof(Light));
            }
            else
            {
                _quality.DirectionalLight =
                    (Light)CustomEditorGUILayout.ObjectField(DirectionalLight, _quality.DirectionalLight,
                        typeof(Light));
            }
        }
    }
}
#endif
