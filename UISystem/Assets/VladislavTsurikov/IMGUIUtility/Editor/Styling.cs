#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.IMGUIUtility.Editor
{
    /// <summary>
    ///     Common styles used for Post-processing editor controls.
    /// </summary>
    public static class Styling
    {
        /// <summary>
        ///     Style for the override checkbox.
        /// </summary>
        public static readonly GUIStyle smallTickbox;

        /// <summary>
        ///     Style for the labels in the toolbar of each effect.
        /// </summary>
        public static readonly GUIStyle miniLabelButton;

        private static readonly Color splitterDark;
        private static readonly Color splitterLight;

        private static readonly Texture2D paneOptionsIconDark;
        private static readonly Texture2D paneOptionsIconLight;

        /// <summary>
        ///     Style for effect header labels.
        /// </summary>
        public static readonly GUIStyle headerLabel;

        private static readonly Color headerBackgroundDark;
        private static readonly Color headerBackgroundLight;

        /// <summary>
        ///     Style for the trackball labels.
        /// </summary>
        public static readonly GUIStyle wheelLabel;

        /// <summary>
        ///     Style for the trackball cursors.
        /// </summary>
        public static readonly GUIStyle wheelThumb;

        /// <summary>
        ///     Size of the trackball cursors.
        /// </summary>
        public static readonly Vector2 wheelThumbSize;

        /// <summary>
        ///     Style for the curve editor position info.
        /// </summary>
        public static readonly GUIStyle preLabel;

        private static Texture2D m_TransparentTexture;

        static Styling()
        {
            smallTickbox = new GUIStyle("ShurikenToggle");

            miniLabelButton = new GUIStyle(EditorStyles.miniLabel);
            miniLabelButton.normal = new GUIStyleState
            {
                background = transparentTexture, scaledBackgrounds = null, textColor = Color.grey
            };
            var activeState = new GUIStyleState
            {
                background = transparentTexture, scaledBackgrounds = null, textColor = Color.white
            };
            miniLabelButton.active = activeState;
            miniLabelButton.onNormal = activeState;
            miniLabelButton.onActive = activeState;

            splitterDark = new Color(0.12f, 0.12f, 0.12f, 1.333f);
            splitterLight = new Color(0.6f, 0.6f, 0.6f, 1.333f);

            headerBackgroundDark = new Color(0.1f, 0.1f, 0.1f, 0.2f);
            headerBackgroundLight = new Color(1f, 1f, 1f, 0.2f);

            paneOptionsIconDark = (Texture2D)EditorGUIUtility.Load("Builtin Skins/DarkSkin/Images/pane options.png");
            paneOptionsIconLight = (Texture2D)EditorGUIUtility.Load("Builtin Skins/LightSkin/Images/pane options.png");

            headerLabel = new GUIStyle(EditorStyles.miniLabel);

            wheelThumb = new GUIStyle("ColorPicker2DThumb");

            wheelThumbSize = new Vector2(
                !Mathf.Approximately(wheelThumb.fixedWidth, 0f) ? wheelThumb.fixedWidth : wheelThumb.padding.horizontal,
                !Mathf.Approximately(wheelThumb.fixedHeight, 0f) ? wheelThumb.fixedHeight : wheelThumb.padding.vertical
            );

            wheelLabel = new GUIStyle(EditorStyles.miniLabel);

            preLabel = new GUIStyle("ShurikenLabel");
        }

        /// <summary>
        ///     Color of UI splitters.
        /// </summary>
        public static Color splitter => EditorGUIUtility.isProSkin ? splitterDark : splitterLight;

        /// <summary>
        ///     Option icon used in effect headers.
        /// </summary>
        public static Texture2D paneOptionsIcon =>
            EditorGUIUtility.isProSkin ? paneOptionsIconDark : paneOptionsIconLight;

        /// <summary>
        ///     Color of effect header backgrounds.
        /// </summary>
        public static Color headerBackground =>
            EditorGUIUtility.isProSkin ? headerBackgroundDark : headerBackgroundLight;

        /// <summary>
        ///     A 1x1 transparent texture.
        /// </summary>
        /// <remarks>
        ///     This texture is only created once and recycled afterward. You shouldn't modify it.
        /// </remarks>
        public static Texture2D transparentTexture
        {
            get
            {
                if (m_TransparentTexture == null)
                {
                    m_TransparentTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false)
                    {
                        name = "Transparent Texture"
                    };
                    m_TransparentTexture.SetPixel(0, 0, Color.clear);
                    m_TransparentTexture.Apply();
                }

                return m_TransparentTexture;
            }
        }
    }
}
#endif
