#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem;
using VladislavTsurikov.SceneUtility.Runtime;
using DragAndDrop = UnityEditor.DragAndDrop;

namespace VladislavTsurikov.SceneManagerTool.Editor.SceneTypeSystem
{
    [ElementEditor(typeof(Group))]
    public class GroupEditor : SceneTypeEditor
    {
        private readonly Color _color = new Color().From256(0, 122, 163);
        private readonly float _windowHeight = 50f;
        private Group _group;
        private ReorderableList _reorderableList;

        public override void OnEnable()
        {
            base.OnEnable();

            _group = (Group)Target;
            _reorderableList =
                new ReorderableList(_group.SceneReferences, typeof(SceneReference), true, true, false, false)
                {
                    drawHeaderCallback = DrawHeader, drawElementCallback = DrawElement
                };
        }

        public override void OnGUI(Rect rect, int index)
        {
            _reorderableList.DoList(rect);

            Color initialGUIColor = GUI.backgroundColor;

            rect.y += CustomEditorGUI.SingleLineHeight * 3;
            rect.y += _group.SceneReferences.Count * CustomEditorGUI.SingleLineHeight;

            if (HasDropOperation())
            {
                var windowRect = new Rect(rect.x, rect.y, rect.width, _windowHeight);
                DrawDragAndDropWindow(initialGUIColor, windowRect, "+");
                DropOperation(windowRect);
                rect.y += _windowHeight + CustomEditorGUI.SingleLineHeight;
            }

            base.OnGUI(rect, index);
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += CustomEditorGUI.SingleLineHeight * 3;
            height += _group.SceneReferences.Count * CustomEditorGUI.SingleLineHeight;

            if (HasDropOperation())
            {
                height += _windowHeight + CustomEditorGUI.SingleLineHeight;
            }


            height += base.GetElementHeight(index);

            return height;
        }

        private void DrawElement(Rect totalRect, int index, bool isActive, bool isFocused)
        {
            Rect rectField = totalRect;
            rectField.width -= 14;

            _group.SceneReferences[index].SceneAsset = (SceneAsset)CustomEditorGUI.ObjectField(
                new Rect(rectField.x, rectField.y, rectField.width, EditorGUIUtility.singleLineHeight),
                null, _group.SceneReferences[index].SceneAsset, typeof(SceneAsset));

            var iconRect = new Rect(rectField.x + rectField.width + 2, rectField.y + 2, 14, 14);

            if (CustomEditorGUI.DrawIcon(iconRect, StyleName.IconButtonMinus, EditorColors.Instance.Red) ||
                (Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyUp)) //rename OK button
            {
                EditorApplication.delayCall += () => { _group.SceneReferences.RemoveAt(index); };

                Event.current.Use();
            }
        }

        private void DrawHeader(Rect rect)
        {
            CustomEditorGUI.Label(rect, "Scenes", CustomEditorGUI.GetStyle(StyleName.LabelFoldout));

            DrawPlusButton(rect);
        }

        private void DrawDragAndDropWindow(Color initialGUIColor, Rect windowRect, string text = null)
        {
            GUIStyle labelTextForSelectedArea = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForSelectedArea);
            GUIStyle boxStyle = CustomEditorGUILayout.GetStyle(StyleName.Box);

            GUI.color = _color;
            GUI.Label(windowRect, "", boxStyle);
            GUI.color = initialGUIColor;

            if (text != null)
            {
                EditorGUI.LabelField(windowRect, text, labelTextForSelectedArea);
            }
        }

        private void DropOperation(Rect virtualRect)
        {
            Event e = Event.current;

            if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
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
                            if (draggedObject is SceneAsset sceneAsset)
                            {
                                _group.SceneReferences.Add(new SceneReference(sceneAsset));
                            }
                        }
                    }

                    e.Use();
                }
            }
        }

        private bool HasDropOperation()
        {
            foreach (Object draggedObject in DragAndDrop.objectReferences)
            {
                if (draggedObject is SceneAsset)
                {
                    return true;
                }
            }

            return false;
        }

        private void DrawPlusButton(Rect rect)
        {
            Rect buttonRect = rect;
            buttonRect.x += rect.width - EditorGUIUtility.singleLineHeight - 3;

            Color color = GUI.color;
            var menuRect = new Rect(buttonRect.x, buttonRect.y + 2f, 14, 14);

            if (CustomEditorGUI.DrawIcon(menuRect, StyleName.IconButtonPlus, EditorColors.Instance.Green))
            {
                _group.SceneReferences.Add(new SceneReference());
                Event.current.Use();
            }

            GUI.color = color;
        }
    }
}
#endif
