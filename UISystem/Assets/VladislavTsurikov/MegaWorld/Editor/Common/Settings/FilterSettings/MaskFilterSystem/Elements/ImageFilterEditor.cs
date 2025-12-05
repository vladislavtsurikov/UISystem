#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using FilterMode = VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.FilterMode;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    [ElementEditor(typeof(ImageFilter))]
    public class ImageFilterEditor : MaskFilterEditor
    {
        private ImageFilter _imageFilter;

        public override void OnEnable() => _imageFilter = (ImageFilter)Target;

        public override void OnGUI(Rect rect, int index)
        {
            if (index != 0)
            {
                _imageFilter.BlendMode = (BlendMode)EditorGUI.EnumPopup(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Blend Mode"), _imageFilter.BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            _imageFilter.Image = (Texture2D)EditorGUI.ObjectField(
                new Rect(rect.x, rect.y, rect.width - 73, EditorGUIUtility.singleLineHeight), "Image",
                _imageFilter.Image, typeof(Texture2D), false);

            rect.y += EditorGUIUtility.singleLineHeight;

            if (_imageFilter.Image == null)
            {
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.height = EditorGUIUtility.singleLineHeight * 2;
                EditorGUI.HelpBox(rect,
                    "Add an image. You can set any image as black and white, as well as having several colors",
                    MessageType.Warning);
            }
            else
            {
                if (_imageFilter.Image != null && !_imageFilter.Valid16BitFormats.Contains(_imageFilter.Image.format))
                {
                    rect.y += EditorGUIUtility.singleLineHeight;
                    rect.height = EditorGUIUtility.singleLineHeight * 5;
                    EditorGUI.HelpBox(rect,
                        "The supplied texture does not have a quality texture format or is less than 16 bit. For optimal quality, use these formats:\nTextureFormat.ARGB4444, TextureFormat.R16, TextureFormat.RFloat, TextureFormat.RGB565, TextureFormat.RGBA4444, TextureFormat.RGBAFloat, TextureFormat.RGBAHalf, TextureFormat.RGFloat, TextureFormat.RGHalf, TextureFormat.RHalf",
                        MessageType.Warning);
                    rect.y += EditorGUIUtility.singleLineHeight * 5;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    rect.y += EditorGUIUtility.singleLineHeight;
                }

                _imageFilter.FilterMode = (FilterMode)EditorGUI.EnumPopup(rect, "Filter Mode", _imageFilter.FilterMode);
                rect.y += EditorGUIUtility.singleLineHeight;

                if (_imageFilter.FilterMode == FilterMode.Color)
                {
                    _imageFilter.Color = EditorGUI.ColorField(rect, "Color", _imageFilter.Color);
                    rect.y += EditorGUIUtility.singleLineHeight;
                    _imageFilter.ColorAccuracy =
                        EditorGUI.Slider(rect, "Color Accuracy", _imageFilter.ColorAccuracy, 0f, 1f);
                }

                _imageFilter.OffSetX = EditorGUI.FloatField(rect, "XOffset", _imageFilter.OffSetX);
                rect.y += EditorGUIUtility.singleLineHeight;
                _imageFilter.OffSetZ = EditorGUI.FloatField(rect, "ZOffset", _imageFilter.OffSetZ);
            }
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            if (_imageFilter.Image == null)
            {
                height += EditorGUIUtility.singleLineHeight * 6;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight * 6;
                if (_imageFilter.Image != null && !_imageFilter.Valid16BitFormats.Contains(_imageFilter.Image.format))
                {
                    height += EditorGUIUtility.singleLineHeight * 7;
                }

                if (_imageFilter.FilterMode == FilterMode.Color)
                {
                    height += EditorGUIUtility.singleLineHeight * 2;
                }
            }

            return height;
        }
    }
}
#endif
