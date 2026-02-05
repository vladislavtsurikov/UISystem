#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CPUNoise.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings
{
    [Serializable]
    public class ProceduralMaskEditor
    {
        public bool ProceduralMaskFoldout = true;
        public bool AdditionalNoiseSettingsFoldout = true;
        public bool ProceduralBrushPreviewTextureFoldout = true;

        [NonSerialized]
        private readonly GUIContent _brushFalloff = new("Brush Falloff (%)",
            "Allows you to control the brush fall by creating a gradient.");

        [NonSerialized]
        private readonly GUIContent _brushStrength = new("Brush Strength (%)",
            "Allows you to change the maximum strength of the brush the lower this parameter, the closer the value.");

        [NonSerialized]
        private readonly GUIContent _fractalNoise = new("Fractal Noise",
            "Mathematical algorithm for generating a procedural texture by a pseudo-random method.");

        [NonSerialized]
        private readonly GUIContent _shape = new("Shape", "Allows you to select the geometric shape of the mask.");

        private readonly ProceduralMask _target;

        public ProceduralMaskEditor(ProceduralMask target) => _target = target;

        public void OnGUI()
        {
            ProceduralMaskFoldout = CustomEditorGUILayout.Foldout(ProceduralMaskFoldout, "Procedural Mask");

            if (ProceduralMaskFoldout)
            {
                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();

                ProceduralBrushPreviewTextureFoldout =
                    CustomEditorGUILayout.Foldout(ProceduralBrushPreviewTextureFoldout, "Preview Texture");

                if (ProceduralBrushPreviewTextureFoldout)
                {
                    EditorGUI.indentLevel++;

                    Rect textureRect = EditorGUILayout.GetControlRect(GUILayout.Height(200), GUILayout.Width(200),
                        GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                    Rect indentedRect = EditorGUI.IndentedRect(textureRect);
                    var finalRect = new Rect(new Vector2(indentedRect.x, indentedRect.y), new Vector2(200, 200));

                    GUI.DrawTexture(finalRect, _target.Mask);

                    EditorGUI.indentLevel--;
                }

                _target.Shape = (Shape)EditorGUILayout.EnumPopup(_shape, _target.Shape);
                _target.Falloff = EditorGUILayout.Slider(_brushFalloff, _target.Falloff, 0f, 100f);
                _target.Strength = EditorGUILayout.Slider(_brushStrength, _target.Strength, 0f, 100f);

                DrawNoiseForProceduralBrush();

                if (EditorGUI.EndChangeCheck())
                {
                    _target.CreateProceduralTexture();
                }

                EditorGUI.indentLevel--;
            }
        }

        public void DrawNoiseForProceduralBrush()
        {
            EditorGUI.BeginChangeCheck();

            _target.FractalNoise = EditorGUILayout.Toggle(_fractalNoise, _target.FractalNoise);

            if (_target.FractalNoise)
            {
                EditorGUI.indentLevel++;

                _target.NoiseType =
                    (NoiseType)EditorGUILayout.EnumPopup(new GUIContent("Check Fractal Noise"),
                        _target.NoiseType);

                _target.Seed = EditorGUILayout.IntSlider(new GUIContent("Seed"), _target.Seed, 0, 65000);
                _target.Octaves = EditorGUILayout.IntSlider(new GUIContent("Octaves"), _target.Octaves, 1, 12);
                _target.Frequency =
                    EditorGUILayout.Slider(new GUIContent("Frequency"), _target.Frequency, 0f, 0.1f);

                _target.Persistence =
                    EditorGUILayout.Slider(new GUIContent("Persistence"), _target.Persistence, 0f, 1f);
                _target.Lacunarity =
                    EditorGUILayout.Slider(new GUIContent("Lacunarity"), _target.Lacunarity, 1f, 3.5f);

                AdditionalNoiseSettingsFoldout =
                    CustomEditorGUILayout.Foldout(AdditionalNoiseSettingsFoldout, "Additional Settings");

                if (AdditionalNoiseSettingsFoldout)
                {
                    EditorGUI.indentLevel++;

                    _target.RemapMin =
                        EditorGUILayout.Slider(new GUIContent("Remap Min"), _target.RemapMin, 0f, 1f);
                    _target.RemapMax =
                        EditorGUILayout.Slider(new GUIContent("Remap Max"), _target.RemapMax, 0f, 1f);

                    _target.Invert = EditorGUILayout.Toggle(new GUIContent("Invert"), _target.Invert);

                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            if (EditorGUI.EndChangeCheck())
            {
                _target.Fractal = new FractalNoiseCPU(_target.GetNoiseForProceduralBrush(), _target.Octaves,
                    _target.Frequency / 7, _target.Lacunarity, _target.Persistence);

                _target.FindNoiseRangeMinMaxForProceduralNoise(150, 150);
            }
        }
    }
}
#endif
