#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ResourceController
{
    public static class ResourcesControllerEditor
    {
        private static bool _resourcesControllerFoldout;

        private static readonly ResourceControllerEditorStack _resourceControllerEditorStack = new();

        static ResourcesControllerEditor() => _resourceControllerEditorStack.Setup();

        public static void DrawResourceController(SelectionData selectionData, bool drawFoldout = true)
        {
            if (selectionData.SelectedData.HasOneSelectedGroup())
            {
                Runtime.Core.SelectionDatas.Group.Group group = selectionData.SelectedData.SelectedGroup;

                ResourceControllerEditor resourceControllerEditor =
                    _resourceControllerEditorStack.GetResourceControllerEditor(group.PrototypeType);

                if (resourceControllerEditor == null)
                {
                    return;
                }

                if (drawFoldout)
                {
                    _resourcesControllerFoldout = CustomEditorGUILayout.Foldout(_resourcesControllerFoldout,
                        "Resources Controller (" + group.GetPrototypeTypeName() + ")");

                    if (_resourcesControllerFoldout)
                    {
                        EditorGUI.indentLevel++;

                        resourceControllerEditor.OnGUI(group);

                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    resourceControllerEditor.OnGUI(group);
                }
            }
        }

        public static bool HasSyncError(Runtime.Core.SelectionDatas.Group.Group group)
        {
            ResourceControllerEditor resourceControllerEditor =
                _resourceControllerEditorStack.GetResourceControllerEditor(group.PrototypeType);

            if (resourceControllerEditor == null)
            {
                return false;
            }

            return resourceControllerEditor.HasSyncError(group);
        }
    }
}
#endif
