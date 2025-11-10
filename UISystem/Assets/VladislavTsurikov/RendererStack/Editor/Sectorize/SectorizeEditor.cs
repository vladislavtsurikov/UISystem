#if UNITY_EDITOR
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.RendererStack.Editor.Core.RendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Sectorize;
using VladislavTsurikov.SceneDataSystem.Editor.StreamingUtility;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;
using GameObjectUtility = VladislavTsurikov.UnityUtility.Runtime.GameObjectUtility;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize
{
    [ElementEditor(typeof(Runtime.Sectorize.Sectorize))]
    public class SectorizeEditor : RendererEditor
    {
        public static SettingsType SettingsType = SettingsType.Sectors;

        private readonly SectorizeMenu _menu = new();
        private Vector2 _windowScrollPos;
        private Runtime.Sectorize.Sectorize _sectorize => (Runtime.Sectorize.Sectorize)Target;

        public override void OnGUI()
        {
            List<Sector> sectors = StreamingUtility.GetAllScenes(Runtime.Sectorize.Sectorize.GetSectorLayerTag());

            if (sectors.Count == 0)
            {
                CustomEditorGUILayout.Label("No Sectors found");

                List<Object> terrains = GameObjectUtility.FindObjectsOfType(typeof(Terrain), true);

                if (terrains.Count > 0)
                {
                    CustomEditorGUILayout.Label("Terrains found in scene: " + terrains.Count);

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                        if (CustomEditorGUILayout.ClickButton("Create Sectors"))
                        {
                            Runtime.Sectorize.Sectorize.Instance.CreateSectors().Forget();
                        }

                        GUILayout.Space(3);
                    }
                    GUILayout.EndHorizontal();
                }
                else if (terrains.Count == 1)
                {
                    CustomEditorGUILayout.Label(
                        "Only one Terrain Found in scene, Please either create more tiles or split current terrain");
                }
                else
                {
                    CustomEditorGUILayout.Label("No Terrains Found in scene, please add some terrains");
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(3);
                    if (CustomEditorGUILayout.ToggleButton("Sectors", SettingsType == SettingsType.Sectors,
                            ButtonStyle.General))
                    {
                        SettingsType = SettingsType.Sectors;
                    }

                    GUILayout.Space(3);
                    if (CustomEditorGUILayout.ToggleButton("Scene Settings", SettingsType == SettingsType.ActiveScene,
                            ButtonStyle.General))
                    {
                        SettingsType = SettingsType.ActiveScene;
                    }

                    GUILayout.Space(3);
                    if (CustomEditorGUILayout.ToggleButton("Global Settings",
                            SettingsType == SettingsType.GlobalSettings, ButtonStyle.General))
                    {
                        SettingsType = SettingsType.GlobalSettings;
                    }

                    GUILayout.Space(5);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(3);

                switch (SettingsType)
                {
                    case SettingsType.Sectors:
                    {
                        GUILayout.Space(3);
                        SectorsSettings();
                        break;
                    }
                    case SettingsType.ActiveScene:
                    {
                        RendererStackManager.Instance.SceneComponentStackEditor.OnGUI();
                        break;
                    }
                    case SettingsType.GlobalSettings:
                    {
                        Runtime.Core.GlobalSettings.GlobalSettings.Instance.RenderersGlobalComponentStackEditor.OnGUI();
                        break;
                    }
                }
            }
        }

        public void SectorsSettings()
        {
            List<Sector> sectors = StreamingUtility.GetAllScenes(Runtime.Sectorize.Sectorize.GetSectorLayerTag());

            if (sectors.Count > 0)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                    if (CustomEditorGUILayout.ClickButton("Delete Sectors", ButtonStyle.Remove))
                    {
                        if (EditorUtility.DisplayDialog("WARNING!",
                                "This will delete your scenes and bring the terrains into the active scene",
                                "OK", "Cancel"))
                        {
                            StreamingUtilityEditor.DeleteAllAdditiveScenes();
                        }
                    }

                    GUILayout.Space(3);
                }
                GUILayout.EndHorizontal();

                if (_sectorize.EnableManualSceneControl)
                {
                    GUILayout.Space(3);

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());

                        if (CustomEditorGUILayout.ClickButton("Load All Scenes"))
                        {
                            StreamingUtility.LoadAllScenes(Runtime.Sectorize.Sectorize.GetSectorLayerTag()).Forget();
                        }

                        GUILayout.Space(3);

                        if (CustomEditorGUILayout.ClickButton("Unload All Scenes"))
                        {
                            StreamingUtility.UnloadAllScenes(Runtime.Sectorize.Sectorize.GetSectorLayerTag()).Forget();
                        }

                        GUILayout.Space(3);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(6);
                }

                CustomEditorGUILayout.Label("Sectors:");
                EditorGUILayout.BeginHorizontal();
                _windowScrollPos = EditorGUILayout.BeginScrollView(_windowScrollPos);

                foreach (Sector sceneData in sectors)
                {
                    CustomEditorGUILayout.Label(sceneData.SceneReference.SceneName);
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndHorizontal();
            }
        }

        public override RendererMenu GetRendererMenu() => _menu;
    }
}
#endif
