#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ResourceController;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject
{
    [ResourceController(typeof(Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject.PrototypeGameObject))]
    public class GameObjectControllerEditor : ResourceControllerEditor
    {
        public override void OnGUI(Runtime.Core.SelectionDatas.Group.Group group)
        {
            CustomEditorGUILayout.HelpBox(
                "If you manually changed the position of the GameObject without using MegaWorld, please click on this button, otherwise, for example, Brush Erase will not be able to delete the changed GameObject.");

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                if (CustomEditorGUILayout.ClickButton("Refresh Cells", ButtonStyle.Add))
                {
                    foreach (SceneDataManager item in SceneDataManagerUtility.GetAllSceneDataManager())
                    {
                        var gameObjectCollider =
                            (GameObjectCollider.Editor.GameObjectCollider)item.SceneDataStack.GetElement(
                                typeof(GameObjectCollider.Editor.GameObjectCollider));

                        gameObjectCollider?.RefreshObjectTree();
                    }
                }

                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();
        }

        public override bool HasSyncError(Runtime.Core.SelectionDatas.Group.Group group) => false;
    }
}
#endif
