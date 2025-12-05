#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Common.GlobalSettings;
using VladislavTsurikov.RendererStack.Runtime.Common.PrototypeSettings;

namespace VladislavTsurikov.RendererStack.Editor.Common.PrototypeSettings
{
    [ElementEditor(typeof(Shadow))]
    public class ShadowEditor : PrototypeComponentEditor
    {
        private float _height;
        private bool _selectLODShadowFoldout = true;
        private Shadow _shadowSettings;

        public GUIContent IsShadowCasting = new("Is Shadow Casting",
            "Sets whether to cast shadows from objects. If this parameter is disabled, then you can increase the optimization");

        public List<GUIContent> LODs = new()
        {
            new GUIContent("LOD 0"),
            new GUIContent("LOD 1"),
            new GUIContent("LOD 2"),
            new GUIContent("LOD 3"),
            new GUIContent("LOD 4"),
            new GUIContent("LOD 5"),
            new GUIContent("LOD 6"),
            new GUIContent("LOD 7"),
            new GUIContent("None")
        };

        public GUIContent ShadowDistance = new("Shadow Distance");

        public GUIContent ShadowLODMap = new("Shadow",
            "These options appear if the registered.p has an LOD Group on it. There will be as many options showing here as there are LOD levels on the LOD Group. Using the dropdowns, you can select which LOD level to render the shadows from for each LOD individual level; or choose \"None\" if you don't wish to render shadows for a specific LOD Group. Managing these options correctly can increase the scene performance greatly.");

        public List<GUIContent> shadowLODs = new()
        {
            new GUIContent("LOD 0 Shadow"),
            new GUIContent("LOD 1 Shadow"),
            new GUIContent("LOD 2 Shadow"),
            new GUIContent("LOD 3 Shadow"),
            new GUIContent("LOD 4 Shadow"),
            new GUIContent("LOD 5 Shadow"),
            new GUIContent("LOD 6 Shadow"),
            new GUIContent("LOD 7 Shadow")
        };

        public GUIContent UseCustomShadowDistance = new("Use Custom Shadow Distance",
            "If this option is enabled, you can set a custom shadow distance for this prototype. This is particularly helpful since shadows can have the hugest impact on performance. If, for example, your scene fog is blurring objects further away from the camera, then using using a smaller shadow distance can increase the performance while not changing much of visual quality. If this option is not enabled, then Renderer will use the shadow distance defined in Unity's quality settings.");

        public override void OnEnable() => _shadowSettings = (Shadow)Target;

        public override void OnGUI(Rect rect, int index)
        {
            var startRectY = rect.y;

            var quality =
                (Quality)Runtime.Core.GlobalSettings.GlobalSettings.Instance.GetElement(typeof(Quality), RendererType);

            if (quality.IsShadowCasting)
            {
                _shadowSettings.ShadowDistance = CustomEditorGUI.FloatField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), ShadowDistance,
                    _shadowSettings.ShadowDistance);
                rect.y += CustomEditorGUI.SingleLineHeight;

                DrawLODShadowSettings(ref rect);
            }
            else
            {
                CustomEditorGUI.WarningBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "Shadows disabled in Global Settings.");
                rect.y += CustomEditorGUI.SingleLineHeight;
            }

            _height = rect.y - startRectY;
        }

        public override float GetElementHeight(int index) => _height;

        private void DrawLODShadowSettings(ref Rect rect)
        {
            if (Prototype.RenderModel.LODs.Count > 1)
            {
                rect.x -= 5;
                _selectLODShadowFoldout = CustomEditorGUI.Foldout(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "LOD Shadow Settings",
                    _selectLODShadowFoldout);
                rect.y += CustomEditorGUI.SingleLineHeight;

                if (_selectLODShadowFoldout)
                {
                    EditorGUI.indentLevel++;

                    rect.x += 3;

                    List<GUIContent> optionsList = LODs.GetRange(0, Prototype.RenderModel.LODs.Count);
                    optionsList.Add(LODs[8]);
                    GUIContent[] options = optionsList.ToArray();

                    for (var i = 0; i < Prototype.RenderModel.LODs.Count; i++)
                    {
                        var index = i * 4;
                        if (i >= 4)
                        {
                            index = (i - 4) * 4 + 1;
                        }

                        var lodIndex = (int)_shadowSettings.ShadowLODMap[index];

                        _shadowSettings.ShadowLODMap[index] = CustomEditorGUI.Popup(
                            new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                            shadowLODs[i], lodIndex >= options.Length ? options.Length - 1 : lodIndex, options);

                        rect.y += CustomEditorGUI.SingleLineHeight;
                    }

                    EditorGUI.indentLevel--;
                }
            }
        }
    }
}
#endif
