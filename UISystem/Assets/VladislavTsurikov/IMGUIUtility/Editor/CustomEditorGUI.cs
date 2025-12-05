#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.IMGUIUtility.Editor
{
    public static class CustomEditorGUI
    {
        private static Vector2 _sliderClickPos;
        private static int _sliderDraggingId = -20000000;
        private static float _sliderOriginalValue;
        private static GUISkin Skin => AssetDatabase.LoadAssetAtPath<GUISkin>(IMGUIContentPath.SkinPath);

        public static float SingleLineHeight => EditorGUIUtility.singleLineHeight + 5;

        public static GUIStyle GetStyle(StyleName styleName) => GetStyle(styleName.ToString());

        public static bool Toggle(Rect rect, GUIContent text, bool value)
        {
            var initialValue = value;

            Rect rectField = PrefixLabel(rect, text);

            value = EditorGUI.Toggle(rectField, value);

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static Enum EnumPopup(Rect rect, GUIContent text, Enum value)
        {
            Enum initialValue = value;

            Rect rectField = PrefixLabel(rect, text);

            value = EditorGUI.EnumPopup(rectField, value);

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static int Popup(Rect rect, GUIContent text, int value, GUIContent[] displayedOptions)
        {
            var initialValue = value;

            Rect rectField = PrefixLabel(rect, text);

            value = EditorGUI.Popup(rectField, value, displayedOptions);

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static float Slider(Rect rect, GUIContent text, float value, float min, float max)
        {
            var initialValue = value;

            Rect rectField = PrefixLabel(rect, text);

            value = EditorGUI.Slider(rectField, value, min, max);

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static int IntSlider(Rect rect, GUIContent text, int value, int min, int max)
        {
            var initialValue = value;

            Rect rectField = PrefixLabel(rect, text);

            value = EditorGUI.IntSlider(rectField, value, min, max);

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static Bounds BoundsField(Rect rect, GUIContent text, Bounds value)
        {
            Bounds initialValue = value;

            Rect rectField = PrefixLabel(rect, text);

            value = EditorGUI.BoundsField(rectField, value);

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static float FloatField(Rect totalRect, GUIContent text, float value)
        {
            var initialValue = value;

            Rect rectField = PrefixLabel(totalRect, text);

            var labelRect = new Rect(totalRect.x + EditorGUI.indentLevel, totalRect.y,
                CustomEditorGUILayout.LabelWidth - EditorGUI.indentLevel, EditorGUIUtility.singleLineHeight);

            value = DragChangeField(value, labelRect);
            value = EditorGUI.FloatField(rectField, value);

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static int IntField(Rect totalRect, GUIContent text, int value)
        {
            var initialValue = value;

            Rect rectField = PrefixLabel(totalRect, text);
            var labelRect = new Rect(totalRect.x + EditorGUI.indentLevel, totalRect.y,
                CustomEditorGUILayout.LabelWidth - EditorGUI.indentLevel, EditorGUIUtility.singleLineHeight);
            value = (int)DragChangeField(value, labelRect);
            value = EditorGUI.IntField(rectField, value);

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static Color ColorField(Rect rect, GUIContent text, Color value)
        {
            Color initialValue = value;

            Rect rectField = PrefixLabel(rect, text);

            value = EditorGUI.ColorField(rectField, value);

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static Vector3 Vector3Field(Rect rect, GUIContent text, Vector3 value)
        {
            Vector3 initialValue = value;

            Rect rectField = PrefixLabel(rect, text);

            value = EditorGUI.Vector3Field(rectField, GUIContent.none, value);

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static Vector2 Vector2Field(Rect rect, GUIContent text, Vector2 value)
        {
            Vector2 initialValue = value;

            Rect rectField = PrefixLabel(rect, text);

            value = EditorGUI.Vector2Field(rectField, GUIContent.none, value);

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static string TextField(Rect rect, GUIContent text, string value)
        {
            var initialValue = value;

            Rect rectField = PrefixLabel(rect, text);

            value = EditorGUI.TextField(rectField, GUIContent.none, value);

            if (initialValue != null)
            {
                if (!initialValue.Equals(value))
                {
                    GUI.changed = true;
                }
            }

            return value;
        }

        public static LayerMask LayerField(Rect rect, GUIContent text, LayerMask value)
        {
            LayerMask initialValue = value;

            Rect rectField = PrefixLabel(rect, text);

            LayerMaskField(rectField, value);

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static Object ObjectField(Rect rect, GUIContent text, Object value, Type objType)
        {
            Object initialValue = value;

            if (text != null)
            {
                Rect rectField = PrefixLabel(rect, text);

                value = EditorGUI.ObjectField(rectField, value, objType, true);
            }
            else
            {
                value = EditorGUI.ObjectField(rect, value, objType, true);
            }

            if (initialValue != value)
            {
                GUI.changed = true;
            }

            return value;
        }

        public static void MinMaxSlider(Rect rect, GUIContent text, ref float min, ref float max, float minimumValue,
            float maximumValue)
        {
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var initialMinValue = min;
            var initialMaxValue = max;

            var localMinValue = min;
            var localMaxValue = max;

            Rect rectField = PrefixLabel(rect, text);

            MinMaxSlider(rectField, ref localMinValue, ref localMaxValue, minimumValue, maximumValue);

            if (!initialMinValue.Equals(localMinValue))
            {
                GUI.changed = true;
            }
            else if (!initialMaxValue.Equals(localMaxValue))
            {
                GUI.changed = true;
            }

            min = localMinValue;
            max = localMaxValue;

            EditorGUI.indentLevel = indentLevel;
        }

        public static void MinMaxIntSlider(Rect rect, GUIContent text, ref int min, ref int max, int minimumValue,
            int maximumValue)
        {
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var initialMinValue = min;
            var initialMaxValue = max;

            var localMinValue = min;
            var localMaxValue = max;

            Rect rectField = PrefixLabel(rect, text);

            MinMaxIntSlider(rectField, ref localMinValue, ref localMaxValue, minimumValue, maximumValue);

            if (!initialMinValue.Equals(localMinValue))
            {
                GUI.changed = true;
            }
            else if (!initialMaxValue.Equals(localMaxValue))
            {
                GUI.changed = true;
            }

            min = localMinValue;
            max = localMaxValue;

            EditorGUI.indentLevel = indentLevel;
        }

        public static void MinMaxSlider(Rect rect, ref float min, ref float max, float minimumValue, float maximumValue)
        {
            var minimumTmp = min;
            var maximumTmp = max;

            minimumTmp = EditorGUI.DelayedFloatField(rect, minimumTmp);
            rect.x += 5;
            EditorGUI.MinMaxSlider(rect, ref minimumTmp, ref maximumTmp, minimumValue, maximumValue);
            rect.x += 5;
            maximumTmp = Mathf.Max(minimumTmp, EditorGUI.DelayedFloatField(rect, maximumTmp));

            min = minimumTmp;
            max = maximumTmp;
        }

        private static void MinMaxIntSlider(Rect rect, ref int min, ref int max, int minimumValue, int maximumValue)
        {
            float minimumTmp = min;
            float maximumTmp = max;

            minimumTmp = EditorGUI.DelayedFloatField(rect, minimumTmp);
            rect.x += 5;
            EditorGUI.MinMaxSlider(rect, ref minimumTmp, ref maximumTmp, minimumValue, maximumValue);
            rect.x += 5;
            maximumTmp = Mathf.Max(minimumTmp, EditorGUI.DelayedFloatField(rect, maximumTmp));

            min = (int)minimumTmp;
            max = (int)maximumTmp;
        }

        public static bool DrawIcon(Rect rect, StyleName styleName, Color iconColor)
        {
            GUIStyle style = GetStyle(styleName);

            Color color = GUI.color;

            GUI.color = iconColor;
            var buttonClicked = GUI.Button(rect, GUIContent.none, style);
            GUI.color = color;

            if (!buttonClicked)
            {
                return false;
            }

            GUIUtility.keyboardControl = 0;
            Event.current.Use();
            return true;
        }

        public static bool ClickButton(Rect rect, string text) => ClickButton(rect, text, ButtonStyle.General);

        public static bool ClickButton(Rect rect, string text, ButtonStyle buttonStyle)
        {
            GUIStyle labelStyle = GetStyle(StyleName.LabelButtonClick);
            GUIStyle barStyle = GetStyle(StyleName.ActiveBar);

            Color labelColor = EditorColors.Instance.LabelColor;
            Color barColor = EditorColors.Instance.ClickButtonColor;

            SetButtonColors(buttonStyle, out barColor, out labelColor);

            return Button(rect, text, labelStyle, barStyle, labelColor, barColor);
        }

        public static bool Button(Rect rect, string text, GUIStyle labelStyle, GUIStyle barStyle, Color labelColor,
            Color barColor)
        {
            var result = GUI.Button(EditorGUI.IndentedRect(rect), "", GUIStyle.none);

            var localBarStyle = new GUIStyle(barStyle) { fixedHeight = rect.height };

            var localLabelStyle = new GUIStyle(labelStyle) { normal = { textColor = labelColor } };

            Color initialColor = GUI.color;

            GUI.color = barColor;
            EditorGUI.LabelField(rect, "", localBarStyle);
            GUI.color = initialColor;

            EditorGUI.LabelField(rect, text, localLabelStyle);

            return result;
        }

        public static void RectTab(Rect rect, string text, ButtonStyle colorSpace, float height, int fontSize)
        {
            GUIStyle labelStyle = GetStyle(StyleName.LabelButton);
            GUIStyle barStyle = GetStyle(StyleName.ActiveBar);

            Color labelColor = EditorColors.Instance.LabelColor;

            SetButtonColors(colorSpace, out Color barColor, out labelColor);

            var localBarStyle = new GUIStyle(barStyle) { fixedHeight = height };

            var localLabelStyle = new GUIStyle(labelStyle) { normal = { textColor = labelColor }, fontSize = fontSize };

            RectTab(text, rect, localLabelStyle, localBarStyle, barColor);
        }

        public static bool RectTab(Rect rect, string text, bool included)
        {
            var result = GUI.Button(rect, "", GUIStyle.none);

            Color barColor;
            Color labelColor = EditorColors.Instance.LabelColor;

            if (included)
            {
                barColor = EditorColors.Instance.ToggleButtonActiveColor;
            }
            else
            {
                barColor = EditorColors.Instance.ToggleButtonInactiveColor;
            }

            RectTab(rect, text, barColor, labelColor);

            return result;
        }

        public static void RectTab(Rect rect, string text, Color barColor, Color labelColor)
        {
            GUIStyle labelStyle = GetStyle(StyleName.LabelButton);
            GUIStyle barStyle = GetStyle(StyleName.ActiveBar);

            var localBarStyle = new GUIStyle(barStyle) { fixedHeight = rect.height };

            var localLabelStyle = new GUIStyle(labelStyle) { normal = { textColor = labelColor } };

            RectTab(text, rect, localLabelStyle, localBarStyle, barColor);
        }

        public static void RectTab(string text, Rect tabRect, GUIStyle labelStyle, GUIStyle barStyle, Color barColor)
        {
            Color initialColor = GUI.color;

            GUI.color = barColor;
            EditorGUI.LabelField(tabRect, "", barStyle);
            GUI.color = initialColor;

            EditorGUI.LabelField(tabRect, text, labelStyle);
        }

        private static void DrawMenu(Rect rect, Action menu)
        {
            Rect buttonRect = rect;
            buttonRect.x += rect.width - EditorGUIUtility.singleLineHeight * 2 - 3;
            buttonRect.width = EditorGUIUtility.singleLineHeight;

            Color color = GUI.color;
            Texture2D menuIcon = Styling.paneOptionsIcon;
            var menuRect = new Rect(buttonRect.x, buttonRect.y + 4f, menuIcon.width, menuIcon.height);
            GUI.DrawTexture(menuRect, Styling.paneOptionsIcon);
            if (GUI.Button(menuRect, GUIContent.none, GUIStyle.none))
            {
                menu.Invoke();
            }

            GUI.color = color;
        }

        public static bool HeaderWithMenu(Rect rect, string content, bool foldout, ref bool active, Action menu)
        {
            DrawMenu(rect, menu);

            return DrawHeader(rect, content, foldout, ref active, menu);
        }

        public static bool HeaderWithMenu(Rect rect, string content, bool foldout, Action menu)
        {
            DrawMenu(rect, menu);

            return DrawHeader(rect, content, foldout, menu);
        }

        internal static bool DrawHeader(Rect rect, string content, bool foldout, Action menu)
        {
            Texture texture;

            if (foldout)
            {
                texture = AssetDatabase.LoadAssetAtPath<Texture>(IMGUIContentPath.FoldoutDownPath);
            }
            else
            {
                texture = AssetDatabase.LoadAssetAtPath<Texture>(IMGUIContentPath.FoldoutRightPath);
            }

            Rect foldoutRect = rect;

            Color initialColor = GUI.color;
            GUI.color = EditorColors.Instance.LabelColor;
            GUI.Label(foldoutRect, texture);
            GUI.color = initialColor;

            Rect labelRect = foldoutRect;
            labelRect.x += EditorGUIUtility.singleLineHeight;
            labelRect.width = rect.width;

            GUIStyle labelStyle = GetStyle(StyleName.LabelFoldout);
            labelStyle.normal.textColor = EditorColors.Instance.LabelColor;
            EditorGUI.LabelField(labelRect, content, labelStyle);

            Event e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (rect.Contains(e.mousePosition))
                {
                    // Left click: Expand/Collapse
                    if (e.button == 0)
                    {
                        foldout = !foldout;
                    }
                    // Right click: Context menu
                    else if (menu != null)
                    {
                        menu();
                    }

                    e.Use();
                }
            }

            return foldout;
        }

        internal static bool DrawHeader(Rect rect, string content, bool foldout, ref bool active, Action menu)
        {
            Texture texture;

            if (foldout)
            {
                texture = AssetDatabase.LoadAssetAtPath<Texture>(IMGUIContentPath.FoldoutDownPath);
            }
            else
            {
                texture = AssetDatabase.LoadAssetAtPath<Texture>(IMGUIContentPath.FoldoutRightPath);
            }

            Rect foldoutRect = rect;

            Color initialColor = GUI.color;
            GUI.color = EditorColors.Instance.LabelColor;
            GUI.Label(foldoutRect, texture);
            GUI.color = initialColor;

            Rect toggleRect = foldoutRect;

            toggleRect.x += EditorGUIUtility.singleLineHeight;
            toggleRect.width = 30;

            active = EditorGUI.Toggle(toggleRect, "", active);

            Rect labelRect = toggleRect;
            labelRect.x += EditorGUIUtility.singleLineHeight;
            labelRect.width = rect.width;

            GUIStyle labelStyle = GetStyle(StyleName.LabelFoldout);
            labelStyle.normal.textColor = EditorColors.Instance.LabelColor;
            EditorGUI.LabelField(labelRect, content, labelStyle);

            Event e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (rect.Contains(e.mousePosition))
                {
                    // Left click: Expand/Collapse
                    if (e.button == 0)
                    {
                        foldout = !foldout;
                    }
                    // Right click: Context menu
                    else if (menu != null)
                    {
                        menu();
                    }

                    e.Use();
                }
            }

            return foldout;
        }

        internal static bool DrawHeader(Rect rect, string content, bool foldout)
        {
            Texture texture;

            if (foldout)
            {
                texture = AssetDatabase.LoadAssetAtPath<Texture>(IMGUIContentPath.FoldoutDownPath);
            }
            else
            {
                texture = AssetDatabase.LoadAssetAtPath<Texture>(IMGUIContentPath.FoldoutRightPath);
            }

            if (GUI.Button(rect, "", GUIStyle.none))
            {
                foldout = !foldout;
            }

            Color initialColor = GUI.color;
            GUI.color = EditorColors.Instance.LabelColor;
            GUI.Label(EditorGUI.IndentedRect(rect), texture);
            GUI.color = initialColor;

            rect.x += EditorGUIUtility.singleLineHeight;
            GUIStyle labelStyle = GetStyle(StyleName.LabelFoldout);
            labelStyle.normal.textColor = EditorColors.Instance.LabelColor;
            EditorGUI.LabelField(rect, content, labelStyle);

            return foldout;
        }

        private static void SetLabelGUIStyle(out GUIStyle labelTextStyle, out GUIStyle barStyle)
        {
            barStyle = GetStyle(StyleName.ActiveBar);
            labelTextStyle = GetStyle(StyleName.LabelText);
        }

        private static void SetButtonColors(ButtonStyle colorSpace, out Color barColor, out Color labelColor)
        {
            switch (colorSpace)
            {
                case ButtonStyle.Add:
                    labelColor = EditorColors.Instance.LabelColor;
                    barColor = EditorColors.Instance.Green.WithAlpha(0.4f);
                    break;
                case ButtonStyle.Remove:
                    labelColor = EditorColors.Instance.LabelColor;
                    barColor = EditorColors.Instance.Red.WithAlpha(0.4f);
                    break;
                default:
                    labelColor = EditorColors.Instance.LabelColor;
                    barColor = EditorColors.Instance.ClickButtonColor;
                    break;
            }
        }

        private static LayerMask LayerMaskField(Rect rect, LayerMask layerMask)
        {
            var layers = new List<string>(32);
            var layerNumbers = new List<int>(32);

            for (var i = 0; i < 32; i++)
            {
                var layerName = LayerMask.LayerToName(i);
                if (layerName != "")
                {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }

            var maskWithoutEmpty = 0;
            for (var i = 0; i < layerNumbers.Count; i++)
            {
                if (((1 << layerNumbers[i]) & layerMask.value) != 0)
                {
                    maskWithoutEmpty |= 1 << i;
                }
            }

            maskWithoutEmpty = EditorGUI.MaskField(rect, maskWithoutEmpty, layers.ToArray());
            var mask = 0;
            for (var i = 0; i < layerNumbers.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) != 0)
                {
                    mask |= 1 << layerNumbers[i];
                }
            }

            layerMask.value = mask;
            return layerMask;
        }

        private static float DragChangeField(float value, Rect sliderRect, float min = 0, float max = 0,
            float minStep = 0.2f)
        {
            var controlId = GUIUtility.GetControlID(FocusType.Passive);
#if UNITY_EDITOR
            EditorGUIUtility.AddCursorRect(sliderRect, MouseCursor.SlideArrow);
#endif
            if (Event.current.type == EventType.MouseDown && sliderRect.Contains(Event.current.mousePosition))
            {
                _sliderClickPos = Event.current.mousePosition;
                _sliderOriginalValue = value;
                _sliderDraggingId = controlId;
            }

            if (Event.current.rawType == EventType.MouseUp)
            {
                _sliderDraggingId = -20000000;

#if UNITY_EDITOR
                if (EditorWindow.focusedWindow != null)
                {
                    EditorWindow.focusedWindow.Repaint();
                }
#endif

                return value;
            }

            if (_sliderDraggingId == controlId && Event.current.type == EventType.MouseDrag)
            {
                var steps = (int)((Event.current.mousePosition.x - _sliderClickPos.x) / 5);

                value = _sliderOriginalValue;

                for (var i = 0; i < Mathf.Abs(steps); i++)
                {
                    var absVal = value >= 0 ? value : -value;

                    var step = 0.01f;
                    if (absVal > 0.99f)
                    {
                        step = 0.02f;
                    }

                    if (absVal > 1.99f)
                    {
                        step = 0.1f;
                    }

                    if (absVal > 4.999f)
                    {
                        step = 0.2f;
                    }

                    if (absVal > 9.999f)
                    {
                        step = 0.5f;
                    }

                    if (absVal > 39.999f)
                    {
                        step = 1f;
                    }

                    if (absVal > 99.999f)
                    {
                        step = 2f;
                    }

                    if (absVal > 199.999f)
                    {
                        step = 5f;
                    }

                    if (absVal > 499.999f)
                    {
                        step = 10f;
                    }

                    if (step < minStep)
                    {
                        step = minStep;
                    }

                    value = steps > 0 ? value + step : value - step;
                    value = Mathf.Round(value * 10000) / 10000f;

                    if (Mathf.Abs(min) > 0.001f && value < min)
                    {
                        value = min;
                    }

                    if (Mathf.Abs(max) > 0.001f && value > max)
                    {
                        value = max;
                    }
                }

#if UNITY_EDITOR
                if (EditorWindow.focusedWindow != null)
                {
                    EditorWindow.focusedWindow.Repaint();
                }

                EditorGUI.FocusTextInControl("");
#endif
            }

            if (Event.current.isMouse && _sliderDraggingId == controlId)
            {
                Event.current.Use();
            }

            return value;
        }

        public static bool Foldout(Rect rect, string content, bool foldout)
        {
            if (CustomEditorGUILayout.IsInspector)
            {
                rect.x -= 5;
            }

            Color initialColor = GUI.color;

            if (GUI.Button(EditorGUI.IndentedRect(rect), "", GUIStyle.none))
            {
                foldout = !foldout;
            }

            Texture texture;

            if (foldout)
            {
                texture = AssetDatabase.LoadAssetAtPath<Texture>(IMGUIContentPath.FoldoutDownPath);
            }
            else
            {
                texture = AssetDatabase.LoadAssetAtPath<Texture>(IMGUIContentPath.FoldoutRightPath);
            }

            GUI.color = EditorColors.Instance.LabelColor;
            GUI.Label(EditorGUI.IndentedRect(rect), texture);
            GUI.color = initialColor;

            rect.x += EditorGUIUtility.singleLineHeight;
            GUIStyle labelStyle = GetStyle(StyleName.LabelFoldout);
            labelStyle.normal.textColor = EditorColors.Instance.LabelColor;
            EditorGUI.LabelField(rect, content, labelStyle);

            return foldout;
        }

        public static void Label(Rect rect, string text, GUIStyle labelStyle)
        {
            labelStyle.normal.textColor = EditorColors.Instance.LabelColor;
            EditorGUI.LabelField(rect, text, labelStyle);
        }

        public static void Label(Rect rect, string text)
        {
            SetLabelGUIStyle(out GUIStyle labelTextStyle, out _);

            labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
            labelTextStyle.fontStyle = FontStyle.Italic;

            EditorGUI.LabelField(rect, new GUIContent(text), labelTextStyle);

            labelTextStyle.fontStyle = FontStyle.Normal;
        }

        private static GUIStyle GetStyle(string styleName)
        {
            GUIStyle style = Skin.GetStyle(styleName);
            return style;
        }

        public static Rect PrefixLabel(Rect totalPosition, GUIContent label)
        {
            if (!LabelHasContent(label))
            {
                return EditorGUI.IndentedRect(totalPosition);
            }

            SetLabelGUIStyle(out GUIStyle labelTextStyle, out _);
            labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;

            var labelRect = new Rect(totalPosition.x + EditorGUI.indentLevel, totalPosition.y,
                CustomEditorGUILayout.LabelWidth - EditorGUI.indentLevel, EditorGUIUtility.singleLineHeight);
            var rect = new Rect((float)(totalPosition.x + (double)CustomEditorGUILayout.LabelWidth + 2.0),
                totalPosition.y, (float)(totalPosition.width - (double)CustomEditorGUILayout.LabelWidth - 2.0),
                totalPosition.height);
            EditorGUI.LabelField(labelRect, label, labelTextStyle);

            return rect;
        }

        public static void HelpBox(Rect rect, string text) => EditorGUI.HelpBox(rect, text, MessageType.Info);

        public static void WarningBox(Rect rect, string text) => EditorGUI.HelpBox(rect, text, MessageType.Warning);

        public static void Header(Rect rect, string text) => EditorGUI.LabelField(rect, text, EditorStyles.boldLabel);

        private static bool LabelHasContent(GUIContent label) =>
            label == null || label.text != string.Empty ||
            label.image != null;
    }
}
#endif
