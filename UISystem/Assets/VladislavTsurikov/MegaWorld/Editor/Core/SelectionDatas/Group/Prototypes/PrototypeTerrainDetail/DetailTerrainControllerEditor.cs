#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ResourceController;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail
{
    [ResourceController(
        typeof(Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail.PrototypeTerrainDetail))]
    public class DetailTerrainControllerEditor : ResourceControllerEditor
    {
        public override void OnGUI(Runtime.Core.SelectionDatas.Group.Group group)
        {
            if (Terrain.activeTerrains.Length != 0)
            {
                DetailTerrainResourcesController.DetectSyncError(group, Terrain.activeTerrain);

                switch (DetailTerrainResourcesController.SyncError)
                {
                    case DetailTerrainResourcesController.TerrainResourcesSyncError.None:
                    {
                        var getResourcesFromTerrain = "Get/Update Resources From Terrain";

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                            GUILayout.BeginVertical();
                            {
                                if (CustomEditorGUILayout.ClickButton(getResourcesFromTerrain, ButtonStyle.ButtonClick,
                                        ButtonSize.ClickButton))
                                {
                                    DetailTerrainResourcesController.UpdatePrototypesFromTerrain(Terrain.activeTerrain,
                                        group);
                                }

                                GUILayout.Space(3);

                                GUILayout.BeginHorizontal();
                                {
                                    if (CustomEditorGUILayout.ClickButton("Add Missing Resources", ButtonStyle.Add))
                                    {
                                        TerrainResourcesController.AddMissingPrototypesToTerrains(group);
                                    }

                                    GUILayout.Space(2);

                                    if (CustomEditorGUILayout.ClickButton("Remove All Resources", ButtonStyle.Remove))
                                    {
                                        if (EditorUtility.DisplayDialog("WARNING!",
                                                "Are you sure you want to remove all Terrain Resources from the scene?",
                                                "OK", "Cancel"))
                                        {
                                            TerrainResourcesController.RemoveAllPrototypesFromTerrains(group);
                                        }
                                    }
                                }
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.EndVertical();
                            GUILayout.Space(5);
                        }
                        GUILayout.EndHorizontal();

                        break;
                    }
                    case DetailTerrainResourcesController.TerrainResourcesSyncError.NotAllProtoAvailable:
                    {
                        CustomEditorGUILayout.WarningBox(
                            "You need all Terrain Details prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");

                        var getResourcesFromTerrain = "Get/Update Resources From Terrain";

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                            GUILayout.BeginVertical();
                            {
                                if (CustomEditorGUILayout.ClickButton(getResourcesFromTerrain, ButtonStyle.ButtonClick,
                                        ButtonSize.ClickButton))
                                {
                                    DetailTerrainResourcesController.UpdatePrototypesFromTerrain(Terrain.activeTerrain,
                                        group);
                                }

                                GUILayout.Space(3);

                                GUILayout.BeginHorizontal();
                                {
                                    if (CustomEditorGUILayout.ClickButton("Add Missing Resources", ButtonStyle.Add))
                                    {
                                        TerrainResourcesController.AddMissingPrototypesToTerrains(group);
                                    }

                                    GUILayout.Space(2);

                                    if (CustomEditorGUILayout.ClickButton("Remove All Resources", ButtonStyle.Remove))
                                    {
                                        if (EditorUtility.DisplayDialog("WARNING!",
                                                "Are you sure you want to remove all Terrain Resources from the scene?",
                                                "OK", "Cancel"))
                                        {
                                            TerrainResourcesController.RemoveAllPrototypesFromTerrains(group);
                                        }
                                    }
                                }
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.EndVertical();
                            GUILayout.Space(5);
                        }
                        GUILayout.EndHorizontal();

                        break;
                    }
                    case DetailTerrainResourcesController.TerrainResourcesSyncError.MissingPrototypes:
                    {
                        var getResourcesFromTerrain = "Get/Update Resources From Terrain";

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                            GUILayout.BeginVertical();
                            {
                                if (CustomEditorGUILayout.ClickButton(getResourcesFromTerrain, ButtonStyle.ButtonClick,
                                        ButtonSize.ClickButton))
                                {
                                    DetailTerrainResourcesController.UpdatePrototypesFromTerrain(Terrain.activeTerrain,
                                        group);
                                }
                            }
                            GUILayout.EndVertical();
                            GUILayout.Space(5);
                        }
                        GUILayout.EndHorizontal();

                        break;
                    }
                }

                GUILayout.Space(3);
            }
            else
            {
                CustomEditorGUILayout.WarningBox("There is no active terrain in the scene.");
            }
        }

        public override bool HasSyncError(Runtime.Core.SelectionDatas.Group.Group group)
        {
            if (group.PrototypeType ==
                typeof(Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail.PrototypeTerrainDetail))
            {
                TerrainResourcesController.DetectSyncError(group, Terrain.activeTerrain);

                if (DetailTerrainResourcesController.SyncError !=
                    DetailTerrainResourcesController.TerrainResourcesSyncError.None)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
#endif
