#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;

namespace VladislavTsurikov.IMGUIUtility.Editor
{
    public static class SelectedWindowUtility
    {
        private const int kWindowResizeBarHeight = 8;
        private static readonly int s_WindowResizeBarHash = "WindowResize".GetHashCode();

        public static void DrawLabelForIcons(Color InitialGUIColor, Rect windowRect, string text = null)
        {
            GUIStyle LabelTextForSelectedArea = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForSelectedArea);
            GUIStyle boxStyle = CustomEditorGUILayout.GetStyle(StyleName.Box);

            GUI.color = EditorColors.Instance.boxColor;
            GUI.Label(windowRect, "", boxStyle);
            GUI.color = InitialGUIColor;

            if (text != null)
            {
                GUI.color = EditorColors.Instance.LabelColor;
                EditorGUI.LabelField(windowRect, text, LabelTextForSelectedArea);
                GUI.color = InitialGUIColor;
            }
        }

        public static Rect GetVirtualRect(Rect windowRect, int count, int iconWidth, int iconHeight)
        {
            var virtualRect = new Rect(windowRect);
            {
                virtualRect.width = Mathf.Max(virtualRect.width - 20, 1); // space for scroll 

                var presetColumns = Mathf.FloorToInt(Mathf.Max(1, virtualRect.width / iconWidth));
                var virtualRows = Mathf.CeilToInt((float)count / presetColumns);

                virtualRect.height = Mathf.Max(virtualRect.height, iconHeight * virtualRows);
            }

            return virtualRect;
        }

        public static void SetDragRect(Event e, Rect iconRect, ref Rect dragRect, out bool isAfter)
        {
            isAfter = e.mousePosition.x - iconRect.xMin > iconRect.width / 2;

            dragRect = new Rect(iconRect);

            if (isAfter)
            {
                dragRect.xMin = dragRect.xMax - 2;
                dragRect.xMax = dragRect.xMax + 2;
            }
            else
            {
                dragRect.xMax = dragRect.xMin + 2;
                dragRect.xMin = dragRect.xMin - 2;
            }
        }

        public static void DrawResizeBar(Event e, int IconHeight, ref float windowHeight)
        {
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(kWindowResizeBarHeight));
            var controlID = GUIUtility.GetControlID(s_WindowResizeBarHash, FocusType.Passive, rect);

            EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeUpDown);

            switch (e.type)
            {
                case EventType.MouseDown:
                {
                    if (rect.Contains(e.mousePosition) && e.button == 0)
                    {
                        GUIUtility.keyboardControl = controlID;
                        GUIUtility.hotControl = controlID;
                        e.Use();
                    }

                    break;
                }
                case EventType.MouseUp:
                {
                    if (GUIUtility.hotControl == controlID && e.button == 0)
                    {
                        GUIUtility.hotControl = 0;
                        e.Use();
                    }

                    break;
                }
                case EventType.MouseDrag:
                {
                    if (GUIUtility.hotControl == controlID)
                    {
                        if (CustomEditorGUILayout.IsInspector)
                        {
                            windowHeight = Mathf.Max(IconHeight, windowHeight + e.delta.y);

                            e.Use();
                        }
                        else
                        {
                            Rect windowRect = GUIUtility.ScreenToGUIRect(CustomEditorGUILayout.ScreenRect);

                            windowHeight = Mathf.Clamp(windowHeight + e.delta.y,
                                IconHeight, windowHeight + (windowRect.yMax - rect.yMax));

                            e.Use();
                        }
                    }

                    break;
                }
                case EventType.Repaint:
                {
                    Rect drawRect = rect;
                    drawRect.yMax -= 2;
                    drawRect.yMin += 2;
                    drawRect = EditorGUI.IndentedRect(drawRect);
                    EditorGUI.DrawRect(drawRect, EditorColors.Instance.orangeDark.WithAlpha(0.7f));
                    break;
                }
            }
        }

        public static string GetShortNameForIcon(string name, int iconWidth)
        {
            var shortNamesDictionary = new Dictionary<string, string>();

            GUIStyle LabelTextForIcon = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForIcon);

            if (name == null || name.Length == 0)
            {
                return "";
            }

            string shortString;

            if (shortNamesDictionary.TryGetValue(name, out shortString))
            {
                return shortString;
            }

            return shortNamesDictionary[name] = TruncateString(name, LabelTextForIcon, iconWidth);
        }

        public static string TruncateString(string str, GUIStyle style, int maxWidth)
        {
            var ellipsis = new GUIContent("...");
            var shortStr = "";

            var ellipsisSize = style.CalcSize(ellipsis).x;
            var textContent = new GUIContent("");

            var charArray = str.ToCharArray();
            for (var i = 0; i < charArray.Length; i++)
            {
                textContent.text += charArray[i];

                var size = style.CalcSize(textContent).x;

                if (size > maxWidth - ellipsisSize)
                {
                    shortStr += ellipsis.text;
                    break;
                }

                shortStr += charArray[i];
            }

            return shortStr;
        }

        public static void SetNextXYIcon(Rect virtualRect, int iconWidth, int iconHeight, ref int y, ref int x)
        {
            if (x + iconWidth < (int)virtualRect.xMax - iconWidth)
            {
                x += iconWidth;
            }
            else if (y < (int)virtualRect.yMax)
            {
                y += iconHeight;
                x = (int)virtualRect.xMin;
            }
        }

        public static string DelayedTextField(Rect position, string text)
        {
#if (UNITY_5_4_OR_NEWER)
            return EditorGUI.DelayedTextField(position, text);
#else
			return EditorGUI.TextField(position, text);
#endif
        }

        public static void AddDelayedAction(EditorApplication.CallbackFunction action) =>
            EditorApplication.delayCall += action;
    }
}
#endif
