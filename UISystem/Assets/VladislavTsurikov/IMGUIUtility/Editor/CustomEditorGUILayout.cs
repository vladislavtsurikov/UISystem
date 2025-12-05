#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.IMGUIUtility.Editor
{
    public static class CustomEditorGUILayout
    {
        public static float LabelWidth = 230;

        private static Vector2 _sliderClickPos;
        private static int _sliderDraggingId = -20000000;
        private static int _sliderOriginalIntValue;
        private static float _sliderOriginalFloatValue;

        public static bool IsInspector = true;
        public static Rect ScreenRect;
        private static GUISkin Skin => AssetDatabase.LoadAssetAtPath<GUISkin>(IMGUIContentPath.SkinPath);

        public static GUIStyle GetStyle(StyleName styleName) => GetStyle(styleName.ToString());

        public static float GetCurrentSpace()
        {
            if (EditorGUI.indentLevel == 0)
            {
                if (IsInspector)
                {
                    return 0;
                }

                return 5;
            }

            return 15 * EditorGUI.indentLevel;
        }

        public static float GetCurrentSpace(int indentLevel)
        {
            if (indentLevel == 0)
            {
                if (IsInspector)
                {
                    return 0;
                }

                return 5;
            }

            return 15 * indentLevel;
        }

        private static void GeneralDrawGUIParamater(GUIContent text, Action drawEditorGUILayout)
        {
            SetLabelGUIStyle(out GUIStyle labelTextStyle, out _);

            GUILayout.BeginHorizontal();
            {
                Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(LabelWidth));
                labelRect.width += 25;

                if (IsInspector)
                {
                    labelRect.x -= 3;
                }
                else
                {
                    labelRect.x += 1;
                }

                labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
                EditorGUI.LabelField(labelRect, new GUIContent(text), labelTextStyle);

                drawEditorGUILayout.Invoke();

                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
        }

        public static bool Toggle(GUIContent text, bool value)
        {
            var initialValue = value;

            GeneralDrawGUIParamater(text, () => value = EditorGUILayout.Toggle(value));

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static Enum EnumPopup(GUIContent text, Enum value)
        {
            Enum initialValue = value;

            GeneralDrawGUIParamater(text, () => value = EditorGUILayout.EnumPopup(value));

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static int Popup(GUIContent text, int value, string[] displayedOptions)
        {
            var initialValue = value;

            GeneralDrawGUIParamater(text, () => value = EditorGUILayout.Popup(value, displayedOptions));

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static float Slider(GUIContent text, float value, float min, float max)
        {
            var initialValue = value;

            GeneralDrawGUIParamater(text, () => value = EditorGUILayout.Slider(value, min, max));

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static int IntSlider(GUIContent text, int value, int min, int max)
        {
            var initialValue = value;

            GeneralDrawGUIParamater(text, () => value = EditorGUILayout.IntSlider(value, min, max));

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static Bounds BoundsField(GUIContent text, Bounds value)
        {
            Bounds initialValue = value;

            GeneralDrawGUIParamater(text, () => value = EditorGUILayout.BoundsField(value));

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static float FloatField(GUIContent text, float value)
        {
            var initialValue = value;

            SetLabelGUIStyle(out GUIStyle labelTextStyle, out _);

            GUILayout.BeginHorizontal();
            {
                Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(LabelWidth));

                if (IsInspector)
                {
                    labelRect.x -= 3;
                }
                else
                {
                    labelRect.x += 1;
                }

                EditorGUI.LabelField(labelRect, new GUIContent(text), labelTextStyle);

                value = DragChangeField(value, labelRect);
                value = EditorGUILayout.FloatField(value);

                if (!initialValue.Equals(value))
                {
                    GUI.changed = true;
                }

                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(2);

            return value;
        }

        public static int IntField(GUIContent text, int value)
        {
            var initialValue = value;

            SetLabelGUIStyle(out GUIStyle labelTextStyle, out _);

            GUILayout.BeginHorizontal();
            {
                labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;

                Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(LabelWidth));

                if (IsInspector)
                {
                    labelRect.x -= 3;
                }
                else
                {
                    labelRect.x += 1;
                }

                EditorGUI.LabelField(labelRect, new GUIContent(text), labelTextStyle);

                value = DragChangeField(value, labelRect);
                value = EditorGUILayout.IntField(value);
                if (!initialValue.Equals(value))
                {
                    GUI.changed = true;
                }

                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(2);

            return value;
        }

        public static Color ColorField(GUIContent text, Color value)
        {
            Color initialValue = value;

            GeneralDrawGUIParamater(text, () => value = EditorGUILayout.ColorField(value));

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static Vector3 Vector3Field(GUIContent text, Vector3 value)
        {
            Vector3 initialValue = value;

            GeneralDrawGUIParamater(text, () => value = EditorGUILayout.Vector3Field(GUIContent.none, value));

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static Vector2 Vector2Field(GUIContent text, Vector2 value)
        {
            Vector2 initialValue = value;

            GeneralDrawGUIParamater(text, () => value = EditorGUILayout.Vector2Field(GUIContent.none, value));

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static string TextField(GUIContent text, string value)
        {
            var initialValue = value;

            GeneralDrawGUIParamater(text, () => value = EditorGUILayout.TextField(GUIContent.none, value));

            if (initialValue != null)
            {
                if (!initialValue.Equals(value))
                {
                    GUI.changed = true;
                }
            }

            return value;
        }

        public static LayerMask LayerField(GUIContent text, LayerMask value)
        {
            LayerMask initialValue = value;

            GeneralDrawGUIParamater(text, () => value = LayerMaskField(value));

            if (!initialValue.Equals(value))
            {
                GUI.changed = true;
            }

            return value;
        }

        public static Object ObjectField(GUIContent text, bool isObjectNull, Object value, Type objType,
            int endHorizontalSpace = 5)
        {
            Object initialValue = value;

            Color initialGUIColor = GUI.color;

            SetCurrentColorForObjectGUIParameter(isObjectNull, out Color GUIcolor);
            SetLabelGUIStyle(out GUIStyle labelTextStyle, out GUIStyle barStyle);

            var localBarStyle = new GUIStyle(barStyle) { fixedHeight = 22 };

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(GetCurrentSpace());

                labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
                GUI.color = GUIcolor;
                GUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal(localBarStyle);
                    {
                        GUILayout.Space(5);

                        GUI.color = initialGUIColor;
                        labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
                        GUILayout.Label(new GUIContent(text), labelTextStyle);

                        value = EditorGUILayout.ObjectField(value, objType, true);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                GUILayout.Space(endHorizontalSpace);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(2);

            if (initialValue != value)
            {
                GUI.changed = true;
            }

            return value;
        }

        public static Object ObjectField(GUIContent text, Object value, Type objType, int endHorizontalSpace = 5)
        {
            Object initialValue = value;

            Color initialGUIColor = GUI.color;

            SetLabelGUIStyle(out GUIStyle labelTextStyle, out _);

            GUILayout.BeginHorizontal();
            {
                labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;

                Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(LabelWidth));

                GUI.color = initialGUIColor;
                labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
                EditorGUI.LabelField(labelRect, new GUIContent(text), labelTextStyle);
                GUILayout.Space(2);
                value = EditorGUILayout.ObjectField(value, objType, true);

                GUILayout.Space(endHorizontalSpace);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(2);

            if (initialValue != value)
            {
                GUI.changed = true;
            }

            return value;
        }

        public static void PropertyField(GUIContent text, SerializedProperty property)
        {
            SetLabelGUIStyle(out GUIStyle labelTextStyle, out _);

            GUILayout.BeginHorizontal();
            {
                Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(LabelWidth));
                labelRect.width += 25;
                labelRect.x += 1;
                labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
                EditorGUI.LabelField(labelRect, new GUIContent(text), labelTextStyle);

                EditorGUILayout.PropertyField(property, GUIContent.none);

                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
        }

        public static void MinMaxSlider(GUIContent text, ref float min, ref float max, float minimumValue,
            float maximumValue)
        {
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var initialMinValue = min;
            var initialMaxValue = max;

            var localMinValue = min;
            var localMaxValue = max;

            SetLabelGUIStyle(out GUIStyle labelTextStyle, out _);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(GetCurrentSpace(indentLevel));
                Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(LabelWidth));
                labelRect.width += 25;
                labelRect.x += 4;
                labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
                EditorGUI.LabelField(labelRect, new GUIContent(text), labelTextStyle);

                GUILayout.Space(2);
                MinMaxSlider(ref localMinValue, ref localMaxValue, minimumValue, maximumValue);

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
                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            EditorGUI.indentLevel = indentLevel;
        }

        public static void MinMaxIntSlider(GUIContent text, ref int min, ref int max, int minimumValue,
            int maximumValue)
        {
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var initialMinValue = min;
            var initialMaxValue = max;

            var localMinValue = min;
            var localMaxValue = max;

            SetLabelGUIStyle(out GUIStyle labelTextStyle, out _);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(GetCurrentSpace(indentLevel));
                Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(LabelWidth));
                labelRect.width += 25;
                labelRect.x += 4;
                labelTextStyle.normal.textColor = EditorColors.Instance.LabelColor;
                EditorGUI.LabelField(labelRect, new GUIContent(text), labelTextStyle);

                MinMaxIntSlider(ref localMinValue, ref localMaxValue, minimumValue, maximumValue);

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
                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            EditorGUI.indentLevel = indentLevel;
        }

        public static void MinMaxSlider(ref float min, ref float max, float minimumValue, float maximumValue)
        {
            var minimumTmp = min;
            var maximumTmp = max;

            minimumTmp = EditorGUILayout.DelayedFloatField(minimumTmp, GUILayout.MaxWidth(50), GUILayout.MinWidth(20));
            EditorGUILayout.MinMaxSlider(ref minimumTmp, ref maximumTmp, minimumValue, maximumValue,
                GUILayout.MinWidth(5));
            maximumTmp = Mathf.Max(minimumTmp,
                EditorGUILayout.DelayedFloatField(maximumTmp, GUILayout.MaxWidth(50), GUILayout.MinWidth(20)));

            min = minimumTmp;
            max = maximumTmp;
        }

        private static void MinMaxIntSlider(ref int min, ref int max, int minimumValue, int maximumValue)
        {
            float minimumTmp = min;
            float maximumTmp = max;

            minimumTmp = EditorGUILayout.DelayedFloatField(minimumTmp, GUILayout.MaxWidth(50), GUILayout.MinWidth(20));
            EditorGUILayout.MinMaxSlider(ref minimumTmp, ref maximumTmp, minimumValue, maximumValue,
                GUILayout.MinWidth(5));
            maximumTmp = Mathf.Max(minimumTmp,
                EditorGUILayout.DelayedFloatField(maximumTmp, GUILayout.MaxWidth(50), GUILayout.MinWidth(20)));

            min = (int)minimumTmp;
            max = (int)maximumTmp;
        }

        public static bool DrawIcon(StyleName styleName, Color iconColor, float rowHeight = -1)
        {
            GUIStyle style = GetStyle(styleName);

            Color color = GUI.color;
            if (rowHeight > 0)
            {
                GUILayout.BeginVertical(GUILayout.Width(style.fixedWidth), GUILayout.Height(rowHeight));
                GUILayout.Space((rowHeight - style.fixedHeight) / 2);
            }

            GUI.color = iconColor;
            var buttonClicked = GUILayout.Button(GUIContent.none, style);
            GUI.color = color;
            if (rowHeight > 0)
            {
                GUILayout.EndVertical();
            }

            if (!buttonClicked)
            {
                return false;
            }

            GUIUtility.keyboardControl = 0;
            Event.current.Use();
            return true;
        }

        public static bool ToggleButton(string text, bool included, ButtonStyle colorSpace,
            ButtonSize buttonSize = ButtonSize.General)
        {
            GUIStyle labelStyle = GetStyle(StyleName.LabelButton);
            GUIStyle barStyle = GetStyle(StyleName.ActiveBar);

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

            if (buttonSize == ButtonSize.ToolButton)
            {
                return Button(text, labelStyle, barStyle, labelColor, barColor, 25);
            }

            return Button(text, labelStyle, barStyle, labelColor, barColor, 21);
        }

        public static bool ClickButton(string text) => ClickButton(text, ButtonStyle.ButtonClick);

        public static bool ClickButton(string text, ButtonStyle buttonStyle, ButtonSize buttonSize = ButtonSize.General)
        {
            GUIStyle labelStyle = GetStyle(StyleName.LabelButtonClick);
            GUIStyle barStyle = GetStyle(StyleName.ActiveBar);

            Color labelColor = EditorColors.Instance.LabelColor;
            Color barColor = EditorColors.Instance.ClickButtonColor;

            SetButtonColors(buttonStyle, out barColor, out labelColor);

            if (buttonSize == ButtonSize.DragAndDropButton)
            {
                return Button(text, labelStyle, barStyle, labelColor, barColor, 40);
            }

            return Button(text, labelStyle, barStyle, labelColor, barColor, 20);
        }

        public static bool Button(string text, GUIStyle labelStyle, GUIStyle barStyle, Color labelColor, Color barColor,
            float height)
        {
            var localBarStyle = new GUIStyle(barStyle) { fixedHeight = height };

            var iconPadding = height * 0.1f;

            bool result;

            Color color = GUI.color;

            GUILayout.BeginVertical();
            {
                GUI.color = barColor;
                result = GUILayout.Button(GUIContent.none, localBarStyle);
                GUILayout.Space(-height);
                GUI.color = color;

                labelStyle.normal.textColor = labelColor;

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(new GUIContent(text), labelStyle, GUILayout.ExpandWidth(true),
                        GUILayout.Height(height));
                    GUILayout.Space(iconPadding);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            return result;
        }

        public static void Label(string text)
        {
            SetLabelGUIStyle(out GUIStyle labelTextStyle, out _);
            Label(text, labelTextStyle);
        }

        public static void Label(string text, GUIStyle style)
        {
            GUILayout.BeginHorizontal();
            {
                Rect labelRect =
                    EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(ScreenRect.width));
                labelRect.x += 1;

                style.normal.textColor = EditorColors.Instance.LabelColor;
                EditorGUI.LabelField(labelRect, new GUIContent(text), style);

                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
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

        public static void RectTab(Rect rect, string text, bool included, float height)
        {
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

            RectTab(rect, text, barColor, labelColor, height);
        }

        public static void RectTab(Rect rect, string text, Color barColor, Color labelColor, float height)
        {
            GUIStyle labelStyle = GetStyle(StyleName.LabelButton);
            GUIStyle barStyle = GetStyle(StyleName.ActiveBar);

            var localBarStyle = new GUIStyle(barStyle) { fixedHeight = height };

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

        public static bool HeaderWithMenu(string content, bool foldout, ref bool active, Action menu)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);

            if (IsInspector)
            {
                rect.x -= 5;
            }

            DrawMenu(rect, menu);

            return DrawHeader(rect, content, foldout, ref active, menu);
        }

        public static bool HeaderWithMenu(string content, bool foldout, Action menu)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);

            if (IsInspector)
            {
                rect.x -= 5;
            }

            DrawMenu(rect, menu);

            return DrawHeader(rect, content, foldout);
        }

        private static void DrawMenu(Rect rect, Action menu)
        {
            Rect buttonRect = rect;
            buttonRect.x += rect.width - EditorGUIUtility.singleLineHeight;
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

        public static bool DrawHeader(Rect rect, string content, bool foldout, ref bool active, Action menu)
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

        public static bool Foldout(bool foldout, string content)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);

            if (IsInspector)
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

        public static void Separator()
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(3));
            r = EditorGUI.IndentedRect(r);
            var start = new Vector2(r.min.x, (r.min.y + r.max.y) / 2);
            var end = new Vector2(r.max.x, (r.min.y + r.max.y) / 2);
            Handles.BeginGUI();
            Handles.color = EditorColors.Instance.separatorColor;
            Handles.DrawLine(start, end);
            Handles.EndGUI();
        }

        private static void SetCurrentColorForObjectGUIParameter(bool isObjectNull, out Color GUIcolor)
        {
            if (isObjectNull)
            {
                GUIcolor = EditorColors.Instance.Red.WithAlpha(0.4f);
            }
            else
            {
                GUIcolor = EditorColors.Instance.Green.WithAlpha(0.4f);
            }
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

        private static LayerMask LayerMaskField(LayerMask layerMask)
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

            maskWithoutEmpty = EditorGUILayout.MaskField(maskWithoutEmpty, layers.ToArray());
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

        private static int DragChangeField(int value, Rect sliderRect, int min = 0, int max = 0, int minStep = 1)
        {
            var controlId = GUIUtility.GetControlID(FocusType.Passive);
#if UNITY_EDITOR
            EditorGUIUtility.AddCursorRect(sliderRect, MouseCursor.SlideArrow);
#endif
            if (Event.current.type == EventType.MouseDown && sliderRect.Contains(Event.current.mousePosition))
            {
                _sliderClickPos = Event.current.mousePosition;
                _sliderOriginalIntValue = value;
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

                value = _sliderOriginalIntValue;

                for (var i = 0; i < Mathf.Abs(steps); i++)
                {
                    var step = 1;

                    value = steps > 0 ? value + step : value - step;

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
                _sliderOriginalFloatValue = value;
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

                value = _sliderOriginalFloatValue;

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

        public static void DrawHelpBanner(string helpURL, string title = "Help")
        {
            GUIStyle labelStyle = GetStyle(StyleName.LabelHelp);
            GUIStyle barStyle = GetStyle(StyleName.ActiveBar);

            Color labelColor = EditorColors.Instance.docsLabelColor;
            Color barColor = EditorColors.Instance.docsButtonColor;

            if (Button(title, labelStyle, barStyle, labelColor, barColor, 25))
            {
                Application.OpenURL(helpURL);
            }
        }

        public static void DrawCustomLabel(string text, bool center = true)
        {
            GUIStyle labelStyle = GetStyle(StyleName.LabelFoldout);
            Color labelColor = EditorColors.Instance.LabelColor;

            DrawCustomLabel(text, labelStyle, labelColor, center);
        }

        private static void DrawCustomLabel(string text, GUIStyle style, Color labelColor, bool center = true)
        {
            if (center)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
            }

            style.normal.textColor = labelColor;

            GUILayout.Label(text, style);

            if (center)
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

        public static void HelpBox(string text) => EditorGUILayout.HelpBox(text, MessageType.Info);

        public static void WarningBox(string text) => EditorGUILayout.HelpBox(text, MessageType.Warning);

        public static void Header(string text) => EditorGUILayout.LabelField(text, EditorStyles.boldLabel);

        private static GUIStyle GetStyle(string styleName)
        {
            GUIStyle style = Skin.GetStyle(styleName);
            return style;
        }
    }
}
#endif
