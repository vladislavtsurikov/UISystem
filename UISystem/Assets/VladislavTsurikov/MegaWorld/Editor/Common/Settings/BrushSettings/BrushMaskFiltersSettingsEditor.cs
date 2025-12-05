#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.BrushSettings
{
    [Serializable]
    public class BrushMaskFiltersSettingsEditor
    {
        private bool _maskFiltersSettingsFoldout = true;

        public void OnGUI(VisualisationMaskFiltersPreference brushVisualisationMaskFiltersPreference) =>
            BrushMaskFiltersSettings(brushVisualisationMaskFiltersPreference);

        public void BrushMaskFiltersSettings(VisualisationMaskFiltersPreference brushVisualisationMaskFiltersPreference)
        {
            _maskFiltersSettingsFoldout =
                CustomEditorGUILayout.Foldout(_maskFiltersSettingsFoldout, "Mask Filters Settings");

            if (_maskFiltersSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                brushVisualisationMaskFiltersPreference.ColorSpace =
                    (ColorSpaceForBrushMaskFilter)CustomEditorGUILayout.EnumPopup(new GUIContent("Color Space"),
                        brushVisualisationMaskFiltersPreference.ColorSpace);

                switch (brushVisualisationMaskFiltersPreference.ColorSpace)
                {
                    case ColorSpaceForBrushMaskFilter.СustomColor:
                    {
                        brushVisualisationMaskFiltersPreference.Color =
                            CustomEditorGUILayout.ColorField(new GUIContent("Color"),
                                brushVisualisationMaskFiltersPreference.Color);
                        brushVisualisationMaskFiltersPreference.EnableStripe = CustomEditorGUILayout.Toggle(
                            new GUIContent("Enable Stripe"), brushVisualisationMaskFiltersPreference.EnableStripe);

                        brushVisualisationMaskFiltersPreference.AlphaVisualisationType =
                            (AlphaVisualisationType)CustomEditorGUILayout.EnumPopup(
                                new GUIContent("Alpha Visualisation Type"),
                                brushVisualisationMaskFiltersPreference.AlphaVisualisationType);

                        break;
                    }
                    case ColorSpaceForBrushMaskFilter.Colorful:
                    {
                        brushVisualisationMaskFiltersPreference.AlphaVisualisationType =
                            (AlphaVisualisationType)CustomEditorGUILayout.EnumPopup(
                                new GUIContent("Alpha Visualisation Type"),
                                brushVisualisationMaskFiltersPreference.AlphaVisualisationType);

                        break;
                    }
                    case ColorSpaceForBrushMaskFilter.Heightmap:
                    {
                        brushVisualisationMaskFiltersPreference.AlphaVisualisationType =
                            (AlphaVisualisationType)CustomEditorGUILayout.EnumPopup(
                                new GUIContent("Alpha Visualisation Type"),
                                brushVisualisationMaskFiltersPreference.AlphaVisualisationType);

                        break;
                    }
                }

                brushVisualisationMaskFiltersPreference.CustomAlpha =
                    CustomEditorGUILayout.Slider(new GUIContent("Alpha"),
                        brushVisualisationMaskFiltersPreference.CustomAlpha, 0, 1);

                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif
