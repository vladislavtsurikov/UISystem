#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.UnityUtility.Editor;
using DragAndDrop = UnityEditor.DragAndDrop;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings
{
    [Serializable]
    public class CustomMasksEditor
    {
        public Vector2 AlphaBrushesWindowsScroll = Vector2.zero;
        public float AlphaBrushesWindowHeight = 100.0f;
        private readonly int _alphaBrushesIconHeight = 60;

        private readonly int _alphaBrushesIconWidth = 60;
        private readonly CustomMasks _target;

        public CustomMasksEditor(CustomMasks target) => _target = target;

        public void OnGUI()
        {
            Event e = Event.current;

            // Settings
            Color initialGUIColor = GUI.backgroundColor;

            Rect windowRect =
                EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, AlphaBrushesWindowHeight)));
            windowRect = EditorGUI.IndentedRect(windowRect);

            var virtualRect = new Rect(windowRect);

            if (IsNecessaryToDrawIconsForCustomBrushes(windowRect, initialGUIColor))
            {
                SelectedWindowUtility.DrawLabelForIcons(initialGUIColor, windowRect);
                DrawBrushIcons(e, windowRect);
            }

            SelectedWindowUtility.DrawResizeBar(e, _alphaBrushesIconHeight, ref AlphaBrushesWindowHeight);

            DropOperationForBrush(e, virtualRect);

            switch (e.type)
            {
                case EventType.ContextClick:
                {
                    if (virtualRect.Contains(e.mousePosition))
                    {
                        BrushesWindowMenu().ShowAsContext();
                        e.Use();
                    }

                    break;
                }
            }

            GUILayout.Space(3);
        }

        private void DrawBrushIcons(Event e, Rect windowRect)
        {
            Rect virtualRect = SelectedWindowUtility.GetVirtualRect(windowRect, _target.Masks.Count,
                _alphaBrushesIconWidth, _alphaBrushesIconHeight);

            Vector2 brushWindowScrollPos = AlphaBrushesWindowsScroll;
            brushWindowScrollPos = GUI.BeginScrollView(windowRect, brushWindowScrollPos, virtualRect, false, true);

            var y = (int)virtualRect.yMin;
            var x = (int)virtualRect.xMin;

            for (var alphaBrushesIndex = 0; alphaBrushesIndex < _target.Masks.Count; alphaBrushesIndex++)
            {
                var brushIconRect = new Rect(x, y, _alphaBrushesIconWidth, _alphaBrushesIconHeight);

                Color rectColor;

                if (alphaBrushesIndex == _target.SelectedCustomBrush)
                {
                    rectColor = EditorColors.Instance.ToggleButtonActiveColor;
                }
                else
                {
                    rectColor = Color.white;
                }

                DrawIconRectForBrush(brushIconRect, _target.Masks[alphaBrushesIndex], rectColor, e, windowRect,
                    brushWindowScrollPos, () => { HandleSelectBrush(alphaBrushesIndex, e); });

                SelectedWindowUtility.SetNextXYIcon(virtualRect, _alphaBrushesIconWidth, _alphaBrushesIconHeight, ref y,
                    ref x);
            }

            AlphaBrushesWindowsScroll = brushWindowScrollPos;

            GUI.EndScrollView();
        }

        public void HandleSelectBrush(int brushIndex, Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                {
                    if (e.button == 0)
                    {
                        _target.SelectedCustomBrush = brushIndex;
                        e.Use();
                    }

                    break;
                }
                case EventType.ContextClick:
                {
                    BrushesMenu(brushIndex).ShowAsContext();
                    e.Use();

                    break;
                }
            }
        }

        private bool IsNecessaryToDrawIconsForCustomBrushes(Rect brushWindowRect, Color initialGUIColor)
        {
            if (_target.Masks.Count == 0)
            {
                SelectedWindowUtility.DrawLabelForIcons(initialGUIColor, brushWindowRect, "Drag & Drop Textures Here");
                return false;
            }

            return true;
        }

        private void DrawIconRectForBrush(Rect brushIconRect, Texture2D preview, Color rectColor, Event e,
            Rect brushWindowRect, Vector2 brushWindowScrollPos, UnityAction clickAction)
        {
            var brushIconRectScrolled = new Rect(brushIconRect);
            brushIconRectScrolled.position -= brushWindowScrollPos;

            // only visible incons
            if (brushIconRectScrolled.Overlaps(brushWindowRect))
            {
                if (brushIconRect.Contains(e.mousePosition))
                {
                    clickAction.Invoke();
                }

                EditorGUI.DrawRect(brushIconRect, rectColor);

                // Prefab preview 
                if (e.type == EventType.Repaint)
                {
                    var previewRect = new Rect(brushIconRect.x + 2, brushIconRect.y + 2, brushIconRect.width - 4,
                        brushIconRect.width - 4);

                    if (preview != null)
                    {
                        GUI.DrawTexture(previewRect, preview);
                    }
                    else
                    {
                        var dimmedColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);
                        EditorGUI.DrawRect(previewRect, dimmedColor);
                    }
                }
            }
        }

        private void DropOperationForBrush(Event e, Rect virtualRect)
        {
            if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
                // Add Prefab
            {
                if (virtualRect.Contains(e.mousePosition))
                {
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }

                    if (e.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is Texture2D)
                            {
                                _target.Masks.Add((Texture2D)draggedObject);
                            }
                        }
                    }

                    e.Use();
                }
            }
        }

        private GenericMenu BrushesWindowMenu()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Get Polaris Brushes"), false, ContextMenuUtility.ContextMenuCallback,
                new Action(() => _target.GetPolarisBrushes()));

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Delete All"), false, ContextMenuUtility.ContextMenuCallback,
                new Action(() => { _target.Masks.Clear(); }));

            return menu;
        }

        private GenericMenu BrushesMenu(int selectedAlphaBrush)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Delete"), false, ContextMenuUtility.ContextMenuCallback,
                new Action(() => _target.Masks.RemoveAt(selectedAlphaBrush)));

            return menu;
        }
    }
}
#endif
