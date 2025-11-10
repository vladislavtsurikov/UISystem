#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.IconStack
{
    public class IconStackEditor
    {
        public delegate IShowIcon AddIconCallbackDelegate(Object obj);

        public delegate void DrawIconRectDelegate(Event e, IShowIcon icon, Rect iconRect, Color textColor,
            Color rectColor);

        public delegate void DrawWindowMenuDelegate();

        public delegate GenericMenu IconMenuCallbackDelegate(IShowIcon icon);

        public delegate void IconSelectedDelegate(IShowIcon icon);

        public delegate void SetIconColorDelegate(IShowIcon icon, out Color textColor, out Color rectColor);

        private readonly DragAndDrop _dragAndDrop = new();
        private readonly bool _draggable;
        private Type _iconType;
        private Vector2 _windowsScroll = Vector2.zero;

        public AddIconCallbackDelegate AddIconCallback;
        public bool DraggedIconType = false;
        public DrawIconRectDelegate DrawIconRect;
        public DrawWindowMenuDelegate DrawWindowMenu;
        public SetIconColorDelegate IconColor;
        public int IconHeight = 95;
        public IconMenuCallbackDelegate IconMenuCallback;
        public IconSelectedDelegate IconSelected;

        public int IconWidth = 80;
        public float WindowHeight = 100f;

        public string ZeroIconsWarning = "Has no icons";

        public IconStackEditor(bool draggable) => _draggable = draggable;

        public void OnGUI(string listMissingWarning = "")
        {
            Color initialGUIColor = GUI.color;

            Event e = Event.current;

            Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, WindowHeight)));
            windowRect = EditorGUI.IndentedRect(windowRect);

            SelectedWindowUtility.DrawLabelForIcons(initialGUIColor, windowRect, listMissingWarning);

            SelectedWindowUtility.DrawResizeBar(e, IconHeight, ref WindowHeight);
        }

        public void OnGUI(List<GameObject> elements)
        {
            _dragAndDrop.OnBeginGUI();

            Color initialGUIColor = GUI.color;

            Event e = Event.current;

            Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, WindowHeight)));
            windowRect = EditorGUI.IndentedRect(windowRect);

            var virtualRect = new Rect(windowRect);

            SelectedWindowUtility.DrawLabelForIcons(initialGUIColor, windowRect);

            DrawIcons(elements, e, windowRect);

            SelectedWindowUtility.DrawResizeBar(e, IconHeight, ref WindowHeight);

            if (DrawWindowMenu != null)
            {
                if (e.type == EventType.ContextClick)
                {
                    if (virtualRect.Contains(e.mousePosition))
                    {
                        DrawWindowMenu();
                        e.Use();
                    }
                }
            }

            _dragAndDrop.OnEndGUI();
        }

        public void OnGUI(IList elements, Type iconType)
        {
            MissingIconsWarningAttribute missingIconsWarningAttribute =
                iconType.GetAttribute<MissingIconsWarningAttribute>();

            if (missingIconsWarningAttribute != null)
            {
                ZeroIconsWarning = missingIconsWarningAttribute.Text;
            }

            _dragAndDrop.OnBeginGUI();

            _iconType = iconType;

            var dragAndDrop = false;

            Color initialGUIColor = GUI.color;

            Event e = Event.current;

            Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, WindowHeight)));
            windowRect = EditorGUI.IndentedRect(windowRect);

            var virtualRect = new Rect(windowRect);

            if (IsNecessaryToDrawIcons(elements, windowRect, initialGUIColor, ref dragAndDrop))
            {
                SelectedWindowUtility.DrawLabelForIcons(initialGUIColor, windowRect);

                DrawIcons(elements, e, windowRect);
            }

            SelectedWindowUtility.DrawResizeBar(e, IconHeight, ref WindowHeight);

            if (AddIconCallback != null && dragAndDrop)
            {
                DropOperation(e, virtualRect);
            }

            if (DrawWindowMenu != null)
            {
                if (e.type == EventType.ContextClick)
                {
                    if (virtualRect.Contains(e.mousePosition))
                    {
                        DrawWindowMenu();
                        e.Use();
                    }
                }
            }

            _dragAndDrop.OnEndGUI();
        }

        private bool IsNecessaryToDrawIcons(IList iconList, Rect windowRect, Color initialGUIColor,
            ref bool dragAndDrop)
        {
            if (iconList.Count == 0)
            {
                SelectedWindowUtility.DrawLabelForIcons(initialGUIColor, windowRect, ZeroIconsWarning);
                dragAndDrop = true;
                return false;
            }

            if (iconList[^1] is not IShowIcon)
            {
                SelectedWindowUtility.DrawLabelForIcons(initialGUIColor, windowRect,
                    "This List does not have a base type \"icon\"");
                dragAndDrop = false;
                return false;
            }

            dragAndDrop = true;
            return true;
        }

        private void DropOperation(Event e, Rect virtualRect)
        {
            if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
            {
                if (virtualRect.Contains(e.mousePosition))
                {
                    if (UnityEditor.DragAndDrop.objectReferences.Length > 0)
                    {
                        UnityEditor.DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }

                    if (e.type == EventType.DragPerform)
                    {
                        UnityEditor.DragAndDrop.AcceptDrag();

                        DropObjectsAttribute dropObjectsAttribute = _iconType.GetAttribute<DropObjectsAttribute>();

                        foreach (Object draggedObject in UnityEditor.DragAndDrop.objectReferences)
                        {
                            if (DraggedIconType)
                            {
                                if (_iconType.IsInstanceOfType(draggedObject))
                                {
                                    AddIconCallback(draggedObject);
                                }
                            }
                            else
                            {
                                foreach (Type type in dropObjectsAttribute.ObjectsTypes)
                                {
                                    if (type == typeof(GameObject))
                                    {
                                        if (draggedObject is GameObject &&
                                            PrefabUtility.GetPrefabAssetType(draggedObject as GameObject) !=
                                            PrefabAssetType.NotAPrefab &&
                                            AssetDatabase.Contains(draggedObject))
                                        {
                                            AddIconCallback(draggedObject);
                                        }
                                    }
                                    else
                                    {
                                        if (type == draggedObject.GetType())
                                        {
                                            AddIconCallback(draggedObject);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    e.Use();
                }
            }
        }

        private void DrawIcons(IList iconList, Event e, Rect windowRect)
        {
            IShowIcon draggingIcon = null;
            if (_dragAndDrop.IsDragging() || _dragAndDrop.IsDragPerform())
            {
                if (_dragAndDrop.GetData().GetType() == _iconType)
                {
                    draggingIcon = (IShowIcon)_dragAndDrop.GetData();
                }
            }

            Rect virtualRect = SelectedWindowUtility.GetVirtualRect(windowRect, iconList.Count, IconWidth, IconHeight);

            Vector2 windowScrollPos = _windowsScroll;
            windowScrollPos = GUI.BeginScrollView(windowRect, windowScrollPos, virtualRect, false, true);

            var dragRect = new Rect(0, 0, 0, 0);

            var y = (int)virtualRect.yMin;
            var x = (int)virtualRect.xMin;

            for (var index = 0; index < iconList.Count; index++)
            {
                var icon = (IShowIcon)iconList[index];
                var iconRect = new Rect(x, y, IconWidth, IconHeight);

                Color textColor;
                Color rectColor;

                if (IconColor != null)
                {
                    IconColor(icon, out textColor, out rectColor);
                }
                else
                {
                    SetIconColor(icon, out textColor, out rectColor);
                }

                var iconRectScrolled = new Rect(iconRect);
                iconRectScrolled.position -= windowScrollPos;

                // only visible icons
                if (iconRectScrolled.Overlaps(windowRect))
                {
                    if (DrawIconRect != null)
                    {
                        DrawIconRect(e, icon, iconRect, textColor, rectColor);
                    }
                    else
                    {
                        DrawIcon(e, iconRect, icon.PreviewTexture, icon.Name, textColor, rectColor, IconWidth, false);
                    }

                    if (iconRect.Contains(e.mousePosition))
                    {
                        if (_draggable)
                        {
                            _dragAndDrop.AddDragObject(icon);

                            if (draggingIcon != null)
                            {
                                SelectedWindowUtility.SetDragRect(e, iconRect, ref dragRect, out var isAfter);

                                if (_dragAndDrop.IsDragPerform())
                                {
                                    Move(iconList, GetSelectedIndex(iconList), index, isAfter);
                                }
                            }
                        }

                        HandleSelect(iconList, index, e);
                    }
                }

                SelectedWindowUtility.SetNextXYIcon(virtualRect, IconWidth, IconHeight, ref y, ref x);
            }

            if (_draggable)
            {
                if (draggingIcon != null)
                {
                    EditorGUI.DrawRect(dragRect, Color.white);
                }
            }

            _windowsScroll = windowScrollPos;

            GUI.EndScrollView();
        }

        public void DrawIcon(Event e, Rect iconRect, Texture2D preview, string name, Color textColor, Color rectColor,
            int iconWidth, bool drawTextureWithAlphaChannel)
        {
            GUIStyle labelTextForIcon = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForIcon);

            EditorGUI.DrawRect(iconRect, rectColor);
            GUI.Label(iconRect, new GUIContent("", name));

            if (e.type == EventType.Repaint)
            {
                var previewRect = new Rect(iconRect.x + 2, iconRect.y + 2, iconRect.width - 4, iconRect.width - 4);

                if (preview != null)
                {
                    if (drawTextureWithAlphaChannel)
                    {
                        GUI.DrawTexture(previewRect, preview);
                    }
                    else
                    {
                        EditorGUI.DrawPreviewTexture(previewRect, preview);
                    }
                }
                else
                {
                    var dimmedColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);
                    EditorGUI.DrawRect(previewRect, dimmedColor);
                }

                labelTextForIcon.normal.textColor = textColor;
                labelTextForIcon.Draw(iconRect, SelectedWindowUtility.GetShortNameForIcon(name, iconWidth), false,
                    false, false, false);
            }
        }

        public void HandleSelect(IList iconList, int index, Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                {
                    if (e.button == 0)
                    {
                        if (e.control)
                        {
                            SelectAdditive(iconList, index);
                        }
                        else if (e.shift)
                        {
                            SelectRange(iconList, index);
                        }
                        else
                        {
                            Select(iconList, index);
                        }

                        e.Use();
                    }

                    break;
                }
                case EventType.ContextClick:
                {
                    if (IconMenuCallback == null)
                    {
                        break;
                    }

                    var icon = (IShowIcon)iconList[index];
                    IconMenuCallback(icon).ShowAsContext();

                    e.Use();

                    break;
                }
            }
        }

        private void SetIconColor(IShowIcon icon, out Color textColor, out Color rectColor)
        {
            textColor = EditorColors.Instance.LabelColor;

            if (icon.Selected)
            {
                rectColor = icon.IsRedIcon
                    ? EditorColors.Instance.redNormal
                    : EditorColors.Instance.ToggleButtonActiveColor;
            }
            else
            {
                rectColor = icon.IsRedIcon
                    ? EditorColors.Instance.redDark
                    : EditorColors.Instance.ToggleButtonInactiveColor;
            }
        }

        private int GetSelectedIndex(IList elements)
        {
            for (var i = 0; i < elements.Count; i++)
            {
                var icon = (IShowIcon)elements[i];
                if (icon.Selected)
                {
                    return i;
                }
            }

            return 0;
        }

        private void Move(IList elements, int sourceIndex, int currentIndex, bool isAfter)
        {
            if (currentIndex >= elements.Count || sourceIndex == currentIndex)
            {
                return;
            }

            var destIndex = currentIndex;

            if (sourceIndex > currentIndex)
            {
                if (isAfter)
                {
                    destIndex += 1;
                }
            }
            else
            {
                if (!isAfter)
                {
                    destIndex -= 1;
                }
            }

            destIndex = Mathf.Clamp(destIndex, 0, elements.Count);

            var item = (IShowIcon)elements[sourceIndex];
            elements.RemoveAt(sourceIndex);
            elements.Insert(destIndex, item);
        }

        public void Select(IList elements, int index)
        {
            if (index < 0 && index >= elements.Count)
            {
                return;
            }

            foreach (IShowIcon localIcon in elements)
            {
                localIcon.Selected = false;
                if (IconSelected != null)
                {
                    IconSelected(localIcon);
                }
            }

            var icon = (IShowIcon)elements[index];
            icon.Selected = true;
            if (IconSelected != null)
            {
                IconSelected(icon);
            }
        }

        public void SelectAdditive(IList elements, int index)
        {
            if (index < 0 && index >= elements.Count)
            {
                return;
            }

            var icon = (IShowIcon)elements[index];

            icon.Selected = !icon.Selected;
            if (IconSelected != null)
            {
                IconSelected(icon);
            }
        }

        public void SelectRange(IList elements, int index)
        {
            if (index < 0 && index >= elements.Count)
            {
                return;
            }

            var rangeMin = index;
            var rangeMax = index;

            for (var i = 0; i < elements.Count; i++)
            {
                var icon = (IShowIcon)elements[i];

                if (icon.Selected)
                {
                    rangeMin = Mathf.Min(rangeMin, i);
                    rangeMax = Mathf.Max(rangeMax, i);
                }
            }

            for (var i = rangeMin; i <= rangeMax; i++)
            {
                var icon = (IShowIcon)elements[i];

                if (icon.Selected != true)
                {
                    break;
                }
            }

            for (var i = rangeMin; i <= rangeMax; i++)
            {
                var icon = (IShowIcon)elements[i];
                icon.Selected = true;
                if (IconSelected != null)
                {
                    IconSelected(icon);
                }
            }
        }
    }
}
#endif
