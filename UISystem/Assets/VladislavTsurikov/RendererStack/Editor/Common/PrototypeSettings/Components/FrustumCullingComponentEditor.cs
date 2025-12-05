#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Common.GlobalSettings;
using VladislavTsurikov.RendererStack.Runtime.Common.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Core;

namespace VladislavTsurikov.RendererStack.Editor.Common.PrototypeSettings
{
    [Serializable]
    [ElementEditor(typeof(FrustumCulling))]
    public class FrustumCullingComponentEditor : PrototypeComponentEditor
    {
        public GUIContent IncreaseBoundingSphere = new("Increase Bounding Sphere",
            "Objects have a Bounding Box or Bounding Sphere, this can be used for Frustum Culling to determine if the camera can see the object. The Renderer uses the Bounding Sphere. The Increase Bounding Sphere parameter allows you to increase the Bounding Sphere, this is necessary so that the object does not disappear when the object is expected to have more Scale, and also if the shader bends the object too much.");

        public GUIContent GetAdditionalShadow = new("Get Additional Shadow",
            "Allows you to choose how the shadows appear when the shadows are not in Camera Frustum. This parameter is necessary so that you can see the shadows behind the camera, mainly this parameter is necessary when the sun is not directed straight down.");

        public GUIContent MinCullingDistance = new("Min Culling Distance",
            "Defines the minimum distance that any kind of culling will occur. If it is a value higher than 0, the instances with a distance less than the specified value to the Camera will not be culled.");

        public GUIContent IncreaseShadowsBoundingSphere = new("Increase Shadows Bounding",
            "The Increase Bounding Sphere parameter allows you to increase the Bounding Sphere, then the number of visible shadows becomes larger.");

        public GUIContent DirectionalLight = new("Directional Light",
            "This parameter is needed to know the direction of the sun's shadows, this allows only visible sun shadows to be displayed.");

        private FrustumCulling _render;

        public override void OnEnable() => _render = (FrustumCulling)Target;

        public override void OnGUI(Rect rect, int index)
        {
            var quality =
                (Quality)Runtime.Core.GlobalSettings.GlobalSettings.Instance.GetElement(typeof(Quality), RendererType);
            var sceneQuality =
                (Runtime.TerrainObjectRenderer.SceneSettings.Quality)RendererStackManager.Instance.SceneComponentStack
                    .GetElement(typeof(Runtime.TerrainObjectRenderer.SceneSettings.Quality));

            var shadow = (Shadow)Prototype.GetSettings(typeof(Shadow));

            _render.IncreaseBoundingSphere = Mathf.Max(0,
                CustomEditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    IncreaseBoundingSphere, _render.IncreaseBoundingSphere));
            rect.y += CustomEditorGUI.SingleLineHeight;

            if (shadow.IsValid() && quality.IsShadowCasting)
            {
                _render.GetAdditionalShadow = (GetAdditionalShadow)CustomEditorGUI.EnumPopup(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), GetAdditionalShadow,
                    _render.GetAdditionalShadow);
                rect.y += CustomEditorGUI.SingleLineHeight;
                EditorGUI.indentLevel++;

                switch (_render.GetAdditionalShadow)
                {
                    case Runtime.Common.PrototypeSettings.GetAdditionalShadow.MinCullingDistance:
                    {
                        _render.MinCullingDistanceForAdditionalShadow = CustomEditorGUI.FloatField(
                            new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), MinCullingDistance,
                            _render.MinCullingDistanceForAdditionalShadow);
                        rect.y += CustomEditorGUI.SingleLineHeight;
                        break;
                    }
                    case Runtime.Common.PrototypeSettings.GetAdditionalShadow.IncreaseBoundingSphere:
                    {
                        _render.IncreaseShadowsBoundingSphere = Mathf.Max(0,
                            CustomEditorGUI.FloatField(
                                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                                IncreaseShadowsBoundingSphere, _render.IncreaseShadowsBoundingSphere));
                        rect.y += CustomEditorGUI.SingleLineHeight;
                        break;
                    }
                    case Runtime.Common.PrototypeSettings.GetAdditionalShadow.DirectionLightShadowVisible:
                    {
                        if (sceneQuality.DirectionalLight == null)
                        {
                            if (CustomEditorGUI.ClickButton(
                                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                                    "Find Directional Light", ButtonStyle.Add))
                            {
                                sceneQuality.FindDirectionalLight();
                            }

                            rect.y += CustomEditorGUI.SingleLineHeight;

                            sceneQuality.DirectionalLight = (Light)CustomEditorGUI.ObjectField(
                                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                                DirectionalLight, sceneQuality.DirectionalLight, typeof(Light));
                            rect.y += CustomEditorGUI.SingleLineHeight;
                        }
                        else
                        {
                            sceneQuality.DirectionalLight = (Light)CustomEditorGUI.ObjectField(
                                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                                DirectionalLight, sceneQuality.DirectionalLight, typeof(Light));
                            rect.y += CustomEditorGUI.SingleLineHeight;
                        }

                        break;
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        public override float GetElementHeight(int index)
        {
            var quality =
                (Quality)Runtime.Core.GlobalSettings.GlobalSettings.Instance.GetElement(typeof(Quality), RendererType);
            var sceneQuality =
                (Runtime.TerrainObjectRenderer.SceneSettings.Quality)RendererStackManager.Instance.SceneComponentStack
                    .GetElement(typeof(Runtime.TerrainObjectRenderer.SceneSettings.Quality));

            var shadow = (Shadow)Prototype.GetSettings(typeof(Shadow));

            var height = EditorGUIUtility.singleLineHeight;

            if (shadow.IsValid() && quality.IsShadowCasting)
            {
                height += CustomEditorGUI.SingleLineHeight;

                switch (_render.GetAdditionalShadow)
                {
                    case Runtime.Common.PrototypeSettings.GetAdditionalShadow.MinCullingDistance:
                    {
                        height += CustomEditorGUI.SingleLineHeight;
                        break;
                    }
                    case Runtime.Common.PrototypeSettings.GetAdditionalShadow.IncreaseBoundingSphere:
                    {
                        height += CustomEditorGUI.SingleLineHeight;
                        break;
                    }
                    case Runtime.Common.PrototypeSettings.GetAdditionalShadow.DirectionLightShadowVisible:
                    {
                        if (sceneQuality.DirectionalLight == null)
                        {
                            height += CustomEditorGUI.SingleLineHeight;
                            height += CustomEditorGUI.SingleLineHeight;
                        }
                        else
                        {
                            height += CustomEditorGUI.SingleLineHeight;
                        }

                        break;
                    }
                }
            }

            return height;
        }
    }
}
#endif
