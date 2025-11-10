#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    [ElementEditor(typeof(VisualisationMaskFiltersPreference))]
    public class VisualisationMaskFiltersPreferenceEditor : IMGUIElementEditor
    {
        private VisualisationMaskFiltersPreference _element => (VisualisationMaskFiltersPreference)Target;

        public override void OnGUI()
        {
            _element.ColorSpace =
                (ColorSpaceForBrushMaskFilter)CustomEditorGUILayout.EnumPopup(new GUIContent("Color Space"),
                    _element.ColorSpace);

            switch (_element.ColorSpace)
            {
                case ColorSpaceForBrushMaskFilter.СustomColor:
                {
                    _element.Color = CustomEditorGUILayout.ColorField(new GUIContent("Color"), _element.Color);
                    _element.EnableStripe =
                        CustomEditorGUILayout.Toggle(new GUIContent("Enable Brush Stripe"), _element.EnableStripe);

                    _element.AlphaVisualisationType =
                        (AlphaVisualisationType)CustomEditorGUILayout.EnumPopup(
                            new GUIContent("Alpha Visualisation Type"), _element.AlphaVisualisationType);

                    break;
                }
                case ColorSpaceForBrushMaskFilter.Colorful:
                {
                    _element.AlphaVisualisationType =
                        (AlphaVisualisationType)CustomEditorGUILayout.EnumPopup(
                            new GUIContent("Alpha Visualisation Type"), _element.AlphaVisualisationType);

                    break;
                }
                case ColorSpaceForBrushMaskFilter.Heightmap:
                {
                    _element.AlphaVisualisationType =
                        (AlphaVisualisationType)CustomEditorGUILayout.EnumPopup(
                            new GUIContent("Alpha Visualisation Type"), _element.AlphaVisualisationType);

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _element.CustomAlpha = CustomEditorGUILayout.Slider(new GUIContent("Alpha"), _element.CustomAlpha, 0, 1);
        }
    }
}
#endif
