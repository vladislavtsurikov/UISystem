#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    [Serializable]
    [ElementEditor(typeof(AspectFilter))]
    public class AspectFilterEditor : MaskFilterEditor
    {
        private static readonly GUIContent _strengthLabel =
            EditorGUIUtility.TrTextContent("Strength", "Controls the strength of the masking effect.");

        private static readonly GUIContent _curveLabel = EditorGUIUtility.TrTextContent("Remap Curve",
            "Remaps the concavity input before computing the final mask.");

        private AspectFilter _aspectFilter;

        public override void OnEnable() => _aspectFilter = (AspectFilter)Target;

        public override void OnGUI(Rect rect, int index)
        {
            if (index != 0)
            {
                _aspectFilter.BlendMode = (BlendMode)EditorGUI.EnumPopup(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Blend Mode"), _aspectFilter.BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            EditorGUI.BeginChangeCheck();

            _aspectFilter.Rotation =
                EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Rotation"), _aspectFilter.Rotation, -180, 180);
            rect.y += EditorGUIUtility.singleLineHeight;
            _aspectFilter.EffectStrength =
                EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    _strengthLabel, _aspectFilter.EffectStrength, 0.0f, 1.0f);
            rect.y += EditorGUIUtility.singleLineHeight;
            _aspectFilter.RemapCurve =
                EditorGUI.CurveField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    _curveLabel, _aspectFilter.RemapCurve);

            if (EditorGUI.EndChangeCheck())
            {
                TextureUtility.AnimationCurveToTexture(_aspectFilter.RemapCurve, ref _aspectFilter.RemapTex);
            }
        }

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            if (index != 0)
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif
