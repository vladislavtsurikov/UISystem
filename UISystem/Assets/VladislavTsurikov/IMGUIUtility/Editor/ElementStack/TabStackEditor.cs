#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.UnityUtility.Editor;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack
{
    public class TabStackEditor
    {
        public delegate void AddCallbackDelegate();

        public delegate GenericMenu AddTabMenuCallbackDelegate(int currentTabIndex);

        public delegate void HappenedMoveCallbackDelegate();

        public delegate void SelectCallbackDelegate(int currentTabIndex);

        public delegate void SetTabColorDelegate(object tab, out Color barColor, out Color labelColor);

        private static readonly string s_tabTextFieldName = "TabNameTextField";
        private static readonly int s_TabsWindowHash = "TabsWindow".GetHashCode();

        private readonly DragAndDrop _dragAndDrop = new();

        private readonly IList _elements;
        private bool _onBeginTabRename;

        private int _renameIndexTab = -1;

        //This variable is required because function "EditorGUILayout.GetControl" returns an invalid Rect, which causes an anomaly in the interface
        private Rect _unityRectFix;

        public AddCallbackDelegate AddCallback;
        public AddTabMenuCallbackDelegate AddTabMenuCallback;
        public bool Draggable = true;
        public bool EnableRename;
        public HappenedMoveCallbackDelegate HappenedMoveCallback;
        public SetTabColorDelegate IconColor;
        public int OffsetTabWidth = 30;
        public SelectCallbackDelegate SelectCallback;
        public int TabHeight = 25;
        public int TabPlusWidth = 50;

        public int TabWidth = 130;

        public bool TabWidthFromName = true;

        public TabStackEditor(IList elements, bool draggable, bool enableRename)
        {
            _elements = elements;
            Draggable = draggable;
            EnableRename = enableRename;
        }

        public void OnGUI()
        {
            var initialIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var tabCount = _elements.Count;

            if (!IsValidList())
            {
                EditorGUI.indentLevel = initialIndentLevel;

                CustomEditorGUILayout.Label("This List does not have a base type \"ISelect and IHasName\"");
                return;
            }

            _dragAndDrop.OnBeginGUI();

            Event e = Event.current;

            Color initialGUIColor = GUI.color;

            if (AddCallback != null)
            {
                tabCount += 1;
            }

            var windowWidth = EditorGUIUtility.currentViewWidth;

            var tabUnderCursor = -1;

            object draggingTab = null;
            if (_dragAndDrop.IsDragging() || _dragAndDrop.IsDragPerform())
            {
                if (_dragAndDrop.GetData() is ISelectable and IHasName)
                {
                    draggingTab = _dragAndDrop.GetData();
                }
            }

            Rect lineRect = EditorGUILayout.GetControlRect(GUILayout.Height(TabHeight));
            lineRect.x += initialIndentLevel * 15;

            var dragRect = new Rect(0, 0, 0, 0);

            if (lineRect.y == 0)
            {
                lineRect = _unityRectFix;
            }
            else
            {
                _unityRectFix = lineRect;
            }

            var tabRect = new Rect(lineRect.x, lineRect.y, TabWidth, TabHeight);
            var tabWindowControlID = GUIUtility.GetControlID(s_TabsWindowHash, FocusType.Passive);

            for (var tabIndex = 0; tabIndex < tabCount; tabIndex++)
            {
                if (AddCallback != null)
                {
                    if (tabIndex == tabCount - 1)
                    {
                        Rect tabPlusRect = tabRect;
                        tabPlusRect.width = TabPlusWidth;

                        float inspectorOffset = CustomEditorGUILayout.IsInspector ? 15 : 0;
                        if (tabPlusRect.width + tabPlusRect.x + inspectorOffset > windowWidth)
                        {
                            EditorGUILayout.GetControlRect(GUILayout.Height(TabHeight));

                            tabPlusRect.x = lineRect.x;
                            tabPlusRect.y += TabHeight;
                        }

                        CustomEditorGUILayout.RectTab(tabPlusRect, "+", ButtonStyle.Add, TabHeight, 28);

                        if (tabPlusRect.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 0)
                        {
                            GUIUtility.keyboardControl = tabWindowControlID;
                            GUIUtility.hotControl = 0;

                            AddCallback();
                        }

                        break;
                    }
                }

                var tab = _elements[tabIndex];

                float localTabWidth = TabWidth;

                if (TabWidthFromName)
                {
                    GUIStyle labelStyle = CustomEditorGUILayout.GetStyle(StyleName.LabelButton);
                    localTabWidth = labelStyle.CalcSize(new GUIContent((tab as IHasName).Name)).x + OffsetTabWidth;
                    tabRect.width = localTabWidth;
                }

                float addWidth = CustomEditorGUILayout.IsInspector ? 15 : 0;
                if (tabRect.width + tabRect.x + addWidth > windowWidth)
                {
                    EditorGUILayout.GetControlRect(GUILayout.Height(TabHeight));

                    tabRect.x = lineRect.x;
                    tabRect.y += TabHeight;
                }

                if (IconColor != null)
                {
                    IconColor(tab, out Color barColor, out Color labelColor);
                    CustomEditorGUILayout.RectTab(tabRect, (tab as IHasName).Name, barColor, labelColor, TabHeight);
                }
                else
                {
                    CustomEditorGUILayout.RectTab(tabRect, (tab as IHasName).Name, (tab as ISelectable).Selected,
                        TabHeight);
                }

                if (EnableRename)
                    // Tab Rename
                {
                    if (tabIndex == _renameIndexTab)
                    {
                        // make TextField and set focus to it
                        if (_onBeginTabRename)
                        {
                            GUI.SetNextControlName(s_tabTextFieldName);

                            GUI.color = EditorColors.Instance.orangeNormal;

                            (tab as IHasName).Name =
                                SelectedWindowUtility.DelayedTextField(tabRect, (tab as IHasName).Name);

                            GUI.color = initialGUIColor;

                            var textEditor =
                                (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor),
                                    GUIUtility.keyboardControl);
                            if (textEditor != null)
                            {
                                textEditor.SelectAll();
                            }

                            GUI.FocusControl(s_tabTextFieldName);
                            _onBeginTabRename = false;
                        }
                        else
                        {
                            // if TextField still in focus - continue rename 
                            if (GUI.GetNameOfFocusedControl() == s_tabTextFieldName)
                            {
                                GUI.SetNextControlName(s_tabTextFieldName);
                                EditorGUI.BeginChangeCheck();

                                GUI.color = EditorColors.Instance.orangeNormal;

                                var newTabName =
                                    SelectedWindowUtility.DelayedTextField(tabRect, (tab as IHasName).Name);

                                GUI.color = initialGUIColor;

                                if (EditorGUI.EndChangeCheck())
                                {
                                    (tab as IHasName).Name = newTabName;
                                }

                                // Unfocus TextField - finish rename
                                if (e.isKey && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Escape))
                                {
                                    GUIUtility.keyboardControl = 0;
                                    GUIUtility.hotControl = 0;
                                    _renameIndexTab = -1;
                                    e.Use();
                                }
                            }
                        }
                    }
                }

                // Tab under cursor
                if (tabRect.Contains(e.mousePosition))
                {
                    tabUnderCursor = tabIndex;

                    if (Draggable)
                    {
                        _dragAndDrop.AddDragObject(_elements[tabUnderCursor]);

                        if (draggingTab != null)
                        {
                            SelectedWindowUtility.SetDragRect(e, tabRect, ref dragRect, out var isAfter);

                            EditorGUIUtility.AddCursorRect(tabRect, MouseCursor.MoveArrow);

                            if (_dragAndDrop.IsDragPerform())
                            {
                                Move(_elements, GetSelectedIndex(), tabIndex, isAfter);
                            }
                        }
                    }
                }

                tabRect.x += localTabWidth;
            }

            if (Draggable)
            {
                if (draggingTab != null)
                {
                    EditorGUI.DrawRect(dragRect, Color.white);
                }
            }

            switch (e.type)
            {
                case EventType.MouseDown:
                {
                    if (tabUnderCursor != -1 && e.button == 0)
                    {
                        GUIUtility.keyboardControl = tabWindowControlID;
                        GUIUtility.hotControl = 0;

                        Select(tabUnderCursor);

                        e.Use();
                    }

                    break;
                }
                case EventType.ContextClick:
                {
                    if (AddTabMenuCallback == null)
                    {
                        break;
                    }

                    if (tabUnderCursor != -1)
                    {
                        GUIUtility.keyboardControl = tabWindowControlID;
                        GUIUtility.hotControl = 0;

                        var tab = _elements[tabUnderCursor];

                        if ((tab as ISelectable).Selected == false)
                        {
                            Select(tabUnderCursor);
                        }
                        else
                        {
                            GenericMenu menu = AddTabMenuCallback(tabUnderCursor);

                            if (EnableRename)
                            {
                                RenameMenu(menu, tabUnderCursor);
                            }

                            menu.ShowAsContext();
                        }

                        e.Use();
                    }

                    break;
                }
            }

            EditorGUI.indentLevel = initialIndentLevel;

            _dragAndDrop.OnEndGUI();
        }

        private void RenameMenu(GenericMenu menu, int currentTabIndex) =>
            menu.AddItem(new GUIContent("Rename"), false, ContextMenuUtility.ContextMenuCallback, new Action(() =>
            {
                _onBeginTabRename = true;
                _renameIndexTab = currentTabIndex;
            }));

        private void Select(int index)
        {
            if (SelectCallback != null)
            {
                SelectCallback(index);
                return;
            }

            foreach (var localTab in _elements)
            {
                ((ISelectable)localTab).Selected = false;
            }

            var tab = _elements[index];

            ((ISelectable)tab).Selected = true;
        }

        private int GetSelectedIndex()
        {
            for (var i = 0; i < _elements.Count; i++)
            {
                var tab = _elements[i];
                if (((ISelectable)tab).Selected)
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

            var item = elements[sourceIndex];
            elements.RemoveAt(sourceIndex);
            elements.Insert(destIndex, item);

            if (HappenedMoveCallback != null)
            {
                HappenedMoveCallback();
            }
        }

        private bool IsValidList()
        {
            var tabCount = _elements.Count;
            if (tabCount != 0)
            {
                var obj = _elements[^1];

                if (obj is not IHasName and ISelectable)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
#endif
