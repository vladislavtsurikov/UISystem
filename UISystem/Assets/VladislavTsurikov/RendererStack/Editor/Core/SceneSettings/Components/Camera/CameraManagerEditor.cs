#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;
using CameraType = VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraType;

namespace VladislavTsurikov.RendererStack.Editor.Core.SceneSettings.Camera
{
    [ElementEditor(typeof(CameraManager))]
    public class CameraManagerEditor : IMGUIElementEditor
    {
        private CameraManager _cameraManager;
        private bool _dragging;
        private ReorderableList _reorderableList;

        public override void OnEnable()
        {
            _cameraManager = (CameraManager)Target;

            _reorderableList = new ReorderableList(_cameraManager.VirtualCameraList, typeof(VirtualCamera), true, false,
                true, false);

            SetupCallbacks();
        }

        public override void OnGUI()
        {
            Rect rect = EditorGUILayout.GetControlRect(true, _reorderableList.GetHeight());
            rect = EditorGUI.IndentedRect(rect);

            _reorderableList.DoList(rect);
        }

        private void SetupCallbacks()
        {
            _reorderableList.drawElementCallback = DrawElementCb;
            _reorderableList.elementHeightCallback = ElementHeightCb;
            _reorderableList.onRemoveCallback = RemoveFilter;
            _reorderableList.onAddCallback = AddCb;
        }

        private void RemoveFilter(ReorderableList list) => _cameraManager.RemoveCamera(list.index);

        private float ElementHeightCb(int index)
        {
            VirtualCamera virtualCamera = _cameraManager.VirtualCameraList[index];

            var height = EditorGUIUtility.singleLineHeight * 1.5f;

            if (virtualCamera == null)
            {
                return EditorGUIUtility.singleLineHeight * 2;
            }

            if (!virtualCamera.FoldoutGUI)
            {
                return CustomEditorGUI.SingleLineHeight;
            }

            height += GetCameraElementHeight(index);
            return height;
        }

        private float GetCameraElementHeight(int index)
        {
            float height = 0;

            VirtualCamera virtualCamera = _cameraManager.VirtualCameraList[index];

            if (virtualCamera.CameraType == CameraType.SceneView)
            {
                if (virtualCamera.Camera == null)
                {
                    height += CustomEditorGUI.SingleLineHeight;
                }
                else
                {
                    height += CustomEditorGUI.SingleLineHeight;
                    height += CustomEditorGUI.SingleLineHeight;
                }
            }
            else if (virtualCamera.Camera == null)
            {
                height += CustomEditorGUI.SingleLineHeight;
            }
            else
            {
                height += CustomEditorGUI.SingleLineHeight;

                height += virtualCamera.CameraComponentStackEditor.GetElementStackHeight();
            }

            return height;
        }


        private void DrawElementCb(Rect totalRect, int index, bool isActive, bool isFocused)
        {
            VirtualCamera virtualCamera = _cameraManager.VirtualCameraList[index];

            if (virtualCamera == null)
            {
                return;
            }

            var cameraEnabled = !virtualCamera.Ignored;

            string foldoutText;

            if (virtualCamera.Camera == null)
            {
                foldoutText = "Camera: NULL";
            }
            else
            {
                var cameraName = virtualCamera.Camera.gameObject.name;

                foldoutText = string.Format("Camera: {0} {1}",
                    !string.IsNullOrEmpty(cameraName) ? cameraName : index.ToString(),
                    cameraEnabled ? string.Empty : "[Ignored]");
            }

            var dividerSize = 1f;
            var paddingV = 6f;
            var paddingH = 4f;
            var iconSize = 14f;

            var isSelected = _reorderableList.index == index;

            Color bgColor;

            if (EditorGUIUtility.isProSkin)
            {
                if (isSelected)
                {
                    UnityEngine.ColorUtility.TryParseHtmlString("#424242", out bgColor);
                }
                else
                {
                    UnityEngine.ColorUtility.TryParseHtmlString("#383838", out bgColor);
                }
            }
            else
            {
                if (isSelected)
                {
                    UnityEngine.ColorUtility.TryParseHtmlString("#b4b4b4", out bgColor);
                }
                else
                {
                    UnityEngine.ColorUtility.TryParseHtmlString("#c2c2c2", out bgColor);
                }
            }

            Color dividerColor;

            if (isSelected)
            {
                dividerColor = EditorColors.Instance.ToggleButtonActiveColor;
            }
            else
            {
                if (EditorGUIUtility.isProSkin)
                {
                    UnityEngine.ColorUtility.TryParseHtmlString("#202020", out dividerColor);
                }
                else
                {
                    UnityEngine.ColorUtility.TryParseHtmlString("#a8a8a8", out dividerColor);
                }
            }

            Color prevColor = GUI.color;

            // modify total rect so it hides the builtin list UI
            totalRect.xMin -= 20f;
            totalRect.xMax += 4f;

            var containsMouse = totalRect.Contains(Event.current.mousePosition);

            // modify currently selected element if mouse down in this elements UnityEngine.GUI rect
            if (containsMouse && Event.current.type == EventType.MouseDown)
            {
                _reorderableList.index = index;
            }

            // draw list element separator
            Rect separatorRect = totalRect;
            // separatorRect.height = dividerSize;
            GUI.color = dividerColor;
            GUI.DrawTexture(separatorRect, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.color = prevColor;

            // Draw BG texture to hide ReorderableList highlight
            totalRect.yMin += dividerSize;
            totalRect.xMin += dividerSize;
            totalRect.xMax -= dividerSize;
            totalRect.yMax -= dividerSize;

            GUI.color = bgColor;
            GUI.DrawTexture(totalRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false);

            GUI.color = new Color(.7f, .7f, .7f, 1f);

            var moveRect = new Rect(totalRect.xMin + paddingH, totalRect.yMin + paddingV, iconSize, iconSize);

            // draw move handle rect
            EditorGUIUtility.AddCursorRect(moveRect, MouseCursor.Pan);
            GUI.DrawTexture(moveRect, Styles.Move, ScaleMode.StretchToFill);

            Rect headerRect = totalRect;
            headerRect.x += 15;
            headerRect.height = EditorGUIUtility.singleLineHeight * 1.3f;

            GUI.color = new Color(1f, 1f, 1f, 1f);

            if (virtualCamera.CameraType != CameraType.SceneView)
            {
                var temporaryActive = virtualCamera.Active;

                virtualCamera.FoldoutGUI = CustomEditorGUI.HeaderWithMenu(headerRect, foldoutText,
                    virtualCamera.FoldoutGUI, ref temporaryActive,
                    () => Menu(_cameraManager.VirtualCameraList[index], index));

                virtualCamera.Active = temporaryActive;
            }
            else
            {
                virtualCamera.FoldoutGUI = CustomEditorGUI.Foldout(headerRect, foldoutText, virtualCamera.FoldoutGUI);
            }

            // update dragging state
            if (containsMouse && isSelected)
            {
                if (Event.current.type == EventType.MouseDrag && !_dragging && isFocused)
                {
                    _dragging = true;
                    _reorderableList.index = index;
                }
            }

            if (_dragging)
            {
                if (Event.current.type == EventType.MouseUp)
                {
                    _dragging = false;
                }
            }

            using (new EditorGUI.DisabledScope(!virtualCamera.Active))
            {
                float rectX = 35;

                totalRect.x += rectX;
                totalRect.y += EditorGUIUtility.singleLineHeight + 3;
                totalRect.width -= rectX + 15;
                totalRect.height = EditorGUIUtility.singleLineHeight;

                GUI.color = prevColor;

                if (virtualCamera.FoldoutGUI)
                {
                    DrawCameraSettings(totalRect, virtualCamera);
                }
            }

            GUI.color = prevColor;
        }

        private void DrawCameraSettings(Rect rect, VirtualCamera virtualCamera)
        {
            if (virtualCamera.CameraType == CameraType.SceneView)
            {
                if (virtualCamera.Camera == null)
                {
                    EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        "The sceneview camera was not found.", MessageType.Info);
                    rect.y += CustomEditorGUI.SingleLineHeight;
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(true);
                    CustomEditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent("Camera"), virtualCamera.Camera, typeof(UnityEngine.Camera));
                    rect.y += CustomEditorGUI.SingleLineHeight;
                    EditorGUI.EndDisabledGroup();

                    EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        "The sceneview camera is used during Edit Mode.", MessageType.Info);

                    rect.y += CustomEditorGUI.SingleLineHeight;
                }
            }
            else if (virtualCamera.Camera == null)
            {
                virtualCamera.Camera = (UnityEngine.Camera)CustomEditorGUI.ObjectField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Camera"),
                    virtualCamera.Camera, typeof(UnityEngine.Camera));
                rect.y += CustomEditorGUI.SingleLineHeight;
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                CustomEditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Camera"), virtualCamera.Camera, typeof(UnityEngine.Camera));
                rect.y += CustomEditorGUI.SingleLineHeight;
                EditorGUI.EndDisabledGroup();

                virtualCamera.CameraComponentStackEditor.OnGUI(rect);
            }
        }

        private void AddCb(ReorderableList list)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Add Camera"), false, () => _cameraManager.AddCamera(null));
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Find Main Camera"), false, () => _cameraManager.FindMainCamera());
            menu.AddItem(new GUIContent("Find All Cameras"), false, () => _cameraManager.FindAllCamera());

            menu.ShowAsContext();
        }

        private void Menu(VirtualCamera virtualCamera, int index)
        {
            var menu = new GenericMenu();

            if (virtualCamera.CameraComponentStackEditor.GetCurrentEditors().Count != 0)
            {
                menu.AddItem(new GUIContent("Reset"), false, () => Reset(virtualCamera, index));
            }

            if (virtualCamera.CameraType != CameraType.SceneView)
            {
                menu.AddItem(new GUIContent("Remove"), false, () => _cameraManager.RemoveCamera(index));
            }

            menu.ShowAsContext();
        }

        private void Reset(VirtualCamera virtualCamera, int index)
        {
            var newVirtualCamera = new VirtualCamera(virtualCamera.Camera);

            _cameraManager.VirtualCameraList[index] = newVirtualCamera;
        }

        private static class Styles
        {
            public static readonly Texture2D Move;

            static Styles() => Move = Resources.Load<Texture2D>("Images/move");
        }
    }
}
#endif
