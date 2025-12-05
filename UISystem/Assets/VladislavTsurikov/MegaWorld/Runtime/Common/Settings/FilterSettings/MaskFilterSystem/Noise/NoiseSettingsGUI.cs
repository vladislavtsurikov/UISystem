#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.API;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise
{
    public class NoiseSettingsGUI
    {
        public NoiseSettings MNoiseSettings;

        public NoiseSettingsGUI(NoiseSettings noiseSettings) => MNoiseSettings = noiseSettings;

        public RenderTexture previewRT { get; private set; }

        public void OnGUI(Rect rect)
        {
            DrawPreviewTexture(ref rect, 256f);
            rect.y += EditorGUIUtility.singleLineHeight;
            TransformSettingsGUI(ref rect);
            rect.y += EditorGUIUtility.singleLineHeight;
            DomainSettingsGUI(ref rect);
        }

        private void TransformSettingsGUI(ref Rect rect)
        {
            MNoiseSettings.ShowNoiseTransformSettings = EditorGUI.Foldout(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                MNoiseSettings.ShowNoiseTransformSettings, new GUIContent("TransformSettings"));
            if (MNoiseSettings.ShowNoiseTransformSettings)
            {
                EditorGUI.indentLevel++;
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Translation"));
                rect.y += EditorGUIUtility.singleLineHeight;
                MNoiseSettings.TransformSettings.Translation = EditorGUI.Vector3Field(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent(""),
                    MNoiseSettings.TransformSettings.Translation);
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Rotation"));
                rect.y += EditorGUIUtility.singleLineHeight;
                MNoiseSettings.TransformSettings.Rotation = EditorGUI.Vector3Field(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent(""),
                    MNoiseSettings.TransformSettings.Rotation);
                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Scale"));
                rect.y += EditorGUIUtility.singleLineHeight;
                MNoiseSettings.TransformSettings.Scale = EditorGUI.Vector3Field(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent(""),
                    MNoiseSettings.TransformSettings.Scale);
                EditorGUI.indentLevel--;
            }
        }

        private void DomainSettingsGUI(ref Rect rect)
        {
            MNoiseSettings.ShowNoiseTypeSettings = EditorGUI.Foldout(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                MNoiseSettings.ShowNoiseTypeSettings, new GUIContent("DomainSettings"));
            if (MNoiseSettings.ShowNoiseTypeSettings)
            {
                EditorGUI.indentLevel++;
                rect.y += EditorGUIUtility.singleLineHeight;
                MNoiseSettings.DomainSettings.NoiseTypeName = NoiseLib.NoiseTypePopup(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Type"),
                    MNoiseSettings.DomainSettings.NoiseTypeName);
                rect.y += EditorGUIUtility.singleLineHeight;
                IFractalType fractalType =
                    NoiseLib.GetFractalTypeInstance(MNoiseSettings.DomainSettings.FractalTypeName);
                MNoiseSettings.DomainSettings.FractalTypeParams =
                    fractalType?.DoGUI(rect, MNoiseSettings.DomainSettings.FractalTypeParams);
                EditorGUI.indentLevel--;
            }
        }

        private void HandlePreviewTextureInput(Rect previewRect)
        {
            if (GUIUtility.hotControl != 0)
            {
                return;
            }

            Vector3 t = MNoiseSettings.TransformSettings.Translation;
            Vector3 r = MNoiseSettings.TransformSettings.Rotation;
            Vector3 s = MNoiseSettings.TransformSettings.Scale;

            EventType eventType = Event.current.type;

            var draggingPreview = Event.current.button == 0 &&
                                  (eventType == EventType.MouseDown ||
                                   eventType == EventType.MouseDrag);

            var previewDims = new Vector2(previewRect.width, previewRect.height);
            var abs = new Vector2(Mathf.Abs(s.x), Mathf.Abs(s.z));

            if (Event.current.type == EventType.ScrollWheel)
            {
                abs += Vector2.one * .001f;

                var scroll = Event.current.delta.y;

                s.x += abs.x * scroll * .05f;
                s.z += abs.y * scroll * .05f;

                MNoiseSettings.TransformSettings.Scale = s;
                GUI.changed = true;

                Event.current.Use();
            }
            else if (draggingPreview)
            {
                // change noise offset panning icon
                var sign = new Vector2(-Mathf.Sign(s.x), Mathf.Sign(s.z));
                Vector2 delta = Event.current.delta / previewDims * abs * sign;
                var d3 = new Vector3(delta.x, 0, delta.y);

                d3 = Quaternion.Euler(r) * d3;

                t += d3;

                MNoiseSettings.TransformSettings.Translation = t;
                GUI.changed = true;

                Event.current.Use();
            }
        }

        /// <summary>
        ///     Renders an interactive Noise Preview along with tooltip icons and an optional Export button that opens a new
        ///     ExportNoiseWindow.
        ///     A background image is also rendered behind the preview that takes up the entire width of the EditorWindow currently
        ///     being drawn.
        /// </summary>
        /// <param name="minSize"> Minimum size for the Preview </param>
        /// <param name="showExportButton"> Whether or not to render the Export button </param>
        public void DrawPreviewTexture(ref Rect rect, float minSize, bool showExportButton = true)
        {
            MNoiseSettings.ShowNoisePreviewTexture = EditorGUI.Foldout(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                MNoiseSettings.ShowNoisePreviewTexture, new GUIContent("PreviewTexture"));
            if (MNoiseSettings.ShowNoisePreviewTexture)
            {
                var padding = 4f;
                var iconWidth = 40f;
                var size = (int)Mathf.Min(minSize, EditorGUIUtility.currentViewWidth);

                Rect currentRect = rect;
                currentRect.y += EditorGUIUtility.singleLineHeight * 2f;
                var totalRect = new Rect(currentRect.x, currentRect.y, currentRect.width, size);

                //Rect totalRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, size + padding * 2); // extra pixels for highlight border

                Color prev = GUI.color;
                GUI.color = new Color(.15f, .15f, .15f, 1f);
                GUI.DrawTexture(totalRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false);
                GUI.color = Color.white;

                // draw info icon
                // if(totalRect.Contains(Event.current.mousePosition))
                {
                    var infoIconRect = new Rect(totalRect.x + padding, totalRect.y + padding, iconWidth, iconWidth);
                    GUI.Label(infoIconRect, Styles.InfoIcon);
                    // GUI.Label( infoIconRect, Styles.noiseTooltip );
                }

                // draw export button

                var buttonWidth = GUI.skin.button.CalcSize(Styles.Export).x;
                var buttonHeight = EditorGUIUtility.singleLineHeight;

                var safeSpace = Mathf.Max(iconWidth * 2, buttonWidth * 2) + padding * 4;
                var minWidth = Mathf.Min(size, totalRect.width - safeSpace);
                var previewRect = new Rect(totalRect.x + totalRect.width / 2 - minWidth / 2,
                    totalRect.y + totalRect.height / 2 - minWidth / 2, minWidth, minWidth);

                EditorGUIUtility.AddCursorRect(previewRect, MouseCursor.Pan);

                if (previewRect.Contains(Event.current.mousePosition))
                {
                    HandlePreviewTextureInput(previewRect);
                }

                if (Event.current.type == EventType.Repaint)
                {
                    // create preview RT here and keep until the next Repaint
                    if (previewRT != null)
                    {
                        RenderTexture.ReleaseTemporary(previewRT);
                    }

                    previewRT = RenderTexture.GetTemporary(512, 512, 0, RenderTextureFormat.ARGB32);
                    var tempRT = RenderTexture.GetTemporary(512, 512, 0, RenderTextureFormat.RFloat);

                    RenderTexture prevActive = RenderTexture.active;

                    NoiseUtils.Blit2D(MNoiseSettings, tempRT);

                    NoiseUtils.BlitPreview2D(tempRT, previewRT);

                    RenderTexture.active = prevActive;

                    GUI.DrawTexture(previewRect, previewRT, ScaleMode.ScaleToFit, false);

                    RenderTexture.ReleaseTemporary(tempRT);
                }

                GUI.color = prev;

                rect.y += 256f + EditorGUIUtility.singleLineHeight;
            }
        }

        private static class Styles
        {
            public static GUIContent NoisePreview;

            public static readonly GUIContent Export = EditorGUIUtility.TrTextContent("Export",
                "Open a window providing options for exporting Noise to Textures");

            public static readonly GUIContent InfoIcon = new("", EditorGUIUtility.FindTexture("console.infoicon"),
                "Scroll Mouse Wheel:\nZooms the preview in and out and changes the noise scale\n\n" +
                "Left-mouse Drag:\nPans the noise field and changes the noise translation\n\n" +
                "Color Key:\nCyan = negative noise values\nGrayscale = values between 0 and 1\nBlack = values are 0\nRed = Values greater than 1. Used for debugging texture normalization");

            static Styles() => NoisePreview = EditorGUIUtility.TrTextContent("Noise Field Preview:");
        }
    }
}
#endif
