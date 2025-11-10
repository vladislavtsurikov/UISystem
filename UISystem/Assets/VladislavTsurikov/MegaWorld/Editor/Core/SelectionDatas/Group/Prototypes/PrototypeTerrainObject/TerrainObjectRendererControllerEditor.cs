#if UNITY_EDITOR
#if RENDERER_STACK
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ResourceController;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject
{
    [ResourceController(
        typeof(Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject.PrototypeTerrainObject))]
    public class TerrainObjectRendererControllerEditor : ResourceControllerEditor
    {
        public override void OnGUI(Runtime.Core.SelectionDatas.Group.Group group)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                if (CustomEditorGUILayout.ClickButton("Get Resources", ButtonStyle.General, ButtonSize.ClickButton))
                {
                    CreatePrototypeFromTerrainObjectRenderer(group);
                }

                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();
        }

        public override bool HasSyncError(Runtime.Core.SelectionDatas.Group.Group group) => false;

        private static void CreatePrototypeFromTerrainObjectRenderer(Runtime.Core.SelectionDatas.Group.Group group)
        {
            foreach (RendererStack.Runtime.TerrainObjectRenderer.PrototypeTerrainObject prototype in
                     TerrainObjectRenderer.Instance.SelectionData.PrototypeList)
            {
                group.AddMissingPrototype(prototype.Prefab);
            }
        }
    }
}
#endif
#endif
