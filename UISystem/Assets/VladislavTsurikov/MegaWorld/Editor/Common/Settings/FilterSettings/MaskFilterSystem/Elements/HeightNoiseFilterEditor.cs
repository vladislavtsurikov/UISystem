#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    [ElementEditor(typeof(HeightNoiseFilter))]
    public class HeightNoiseFilterEditor : MaskFilterEditor
    {
        private HeightNoiseFilter _heightNoiseFilter;

        private NoiseSettingsGUI _mNoiseSettingsGUI;

        private NoiseSettingsGUI noiseSettingsGUI
        {
            get
            {
                if (_mNoiseSettingsGUI == null)
                {
                    _mNoiseSettingsGUI = new NoiseSettingsGUI(_heightNoiseFilter.NoiseSettings);
                }

                return _mNoiseSettingsGUI;
            }
        }

        public override void OnEnable() => _heightNoiseFilter = (HeightNoiseFilter)Target;

        public override void OnGUI(Rect rect, int index)
        {
            if (index != 0)
            {
                _heightNoiseFilter.BlendMode = (BlendMode)EditorGUI.EnumPopup(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Blend Mode"), _heightNoiseFilter.BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            CreateNoiseSettingsIfNecessary();

            DrawHeightSettings(ref rect);

            noiseSettingsGUI.OnGUI(rect);
        }

        private void DrawHeightSettings(ref Rect rect)
        {
            _heightNoiseFilter.MinHeight = EditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Min Height"),
                _heightNoiseFilter.MinHeight);

            rect.y += EditorGUIUtility.singleLineHeight;

            _heightNoiseFilter.MaxHeight = EditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Height"),
                _heightNoiseFilter.MaxHeight);

            rect.y += EditorGUIUtility.singleLineHeight;

            _heightNoiseFilter.MinRangeNoise = EditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Min Range Noise"), _heightNoiseFilter.MinRangeNoise);
            rect.y += EditorGUIUtility.singleLineHeight;
            _heightNoiseFilter.MaxRangeNoise = EditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Max Range Noise"), _heightNoiseFilter.MaxRangeNoise);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            CreateNoiseSettingsIfNecessary();

            var height = EditorGUIUtility.singleLineHeight * 8;
            height += EditorGUIUtility.singleLineHeight;

            if (_heightNoiseFilter.NoiseSettings.ShowNoisePreviewTexture)
            {
                height += 256f + 40f;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            if (_heightNoiseFilter.NoiseSettings.ShowNoiseTransformSettings)
            {
                height += EditorGUIUtility.singleLineHeight * 7;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            if (_heightNoiseFilter.NoiseSettings.ShowNoiseTypeSettings)
            {
                height += EditorGUIUtility.singleLineHeight * 15;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            height += EditorGUIUtility.singleLineHeight;

            return height;
        }

        private void CreateNoiseSettingsIfNecessary()
        {
            if (_heightNoiseFilter.NoiseSettings == null)
            {
                _heightNoiseFilter.NoiseSettings = new NoiseSettings();
            }
        }
    }
}
#endif
