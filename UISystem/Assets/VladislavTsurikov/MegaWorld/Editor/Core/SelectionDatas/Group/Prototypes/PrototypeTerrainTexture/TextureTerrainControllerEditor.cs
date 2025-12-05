#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ResourceController;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;
using VladislavTsurikov.UnityUtility.Editor;
using IMGUIUtility_Editor_DragAndDrop = VladislavTsurikov.IMGUIUtility.Editor.DragAndDrop;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture
{
    [ResourceController(
        typeof(Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture.PrototypeTerrainTexture))]
    public class TextureTerrainControllerEditor : ResourceControllerEditor
    {
        private static readonly List<Layer> _paletteLayers = new();

        private static readonly int _protoIconWidth = 64;
        private static readonly int _protoIconHeight = 76;
        private static float _prototypeWindowHeight = 100f;

        private static Vector2 _prototypeWindowsScroll = Vector2.zero;
        private static readonly IMGUIUtility_Editor_DragAndDrop _dragAndDrop = new();

        private static bool _terrainLayerSettingsFoldout;

        public override void OnGUI(Runtime.Core.SelectionDatas.Group.Group group)
        {
            if (Terrain.activeTerrains.Length != 0)
            {
                TextureTerrainResourcesController.DetectSyncError(group, Terrain.activeTerrain);

                switch (TextureTerrainResourcesController.SyncError)
                {
                    case TextureTerrainResourcesController.TerrainResourcesSyncError.None:
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
                                    TextureTerrainResourcesController.UpdatePrototypesFromTerrain(Terrain.activeTerrain,
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
                    case TextureTerrainResourcesController.TerrainResourcesSyncError.MissingTerrainLayer:
                    {
                        List<Prototype> selectedPrototypes = group.GetAllSelectedPrototypes();

                        if (selectedPrototypes.Count == 1)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                                GUILayout.BeginVertical();
                                {
                                    _terrainLayerSettingsFoldout =
                                        CustomEditorGUILayout.Foldout(_terrainLayerSettingsFoldout,
                                            "Terrain Layer Settings");

                                    if (_terrainLayerSettingsFoldout)
                                    {
                                    }
                                }
                                GUILayout.EndVertical();
                                GUILayout.Space(5);
                            }
                            GUILayout.EndHorizontal();
                        }

                        break;
                    }
                    case TextureTerrainResourcesController.TerrainResourcesSyncError.NotAllProtoAvailable:
                    {
                        CustomEditorGUILayout.WarningBox(
                            "You need all Terrain Textures prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");

                        var getResourcesFromTerrain = "Get/Update Resources From Terrain";

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                            GUILayout.BeginVertical();
                            {
                                if (CustomEditorGUILayout.ClickButton(getResourcesFromTerrain, ButtonStyle.ButtonClick,
                                        ButtonSize.ClickButton))
                                {
                                    TextureTerrainResourcesController.UpdatePrototypesFromTerrain(Terrain.activeTerrain,
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
                    case TextureTerrainResourcesController.TerrainResourcesSyncError.MissingPrototypes:
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
                                    TextureTerrainResourcesController.UpdatePrototypesFromTerrain(Terrain.activeTerrain,
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

                CustomEditorGUILayout.Header("Active Terrain: Layer Palette");

                if (Terrain.activeTerrain != null)
                {
                    DrawSelectedWindowForTerrainTextureResources(Terrain.activeTerrain, group);
                }

                GUILayout.Space(3);
            }
            else
            {
                CustomEditorGUILayout.WarningBox("There is no active terrain in the scene.");
            }

            ChangeTerrainLayer(group);
        }

        public override bool HasSyncError(Runtime.Core.SelectionDatas.Group.Group group)
        {
            if (group.PrototypeType ==
                typeof(Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture.PrototypeTerrainTexture))
            {
                TerrainResourcesController.DetectSyncError(group, Terrain.activeTerrain);

                if (TextureTerrainResourcesController.SyncError !=
                    TextureTerrainResourcesController.TerrainResourcesSyncError.None)
                {
                    return true;
                }
            }

            return false;
        }

        private static void UpdateLayerPalette(Terrain terrain)
        {
            if (terrain == null)
            {
                return;
            }

            var selectedList = new bool[_paletteLayers.Count];
            for (var i = 0; i < _paletteLayers.Count; i++)
            {
                if (_paletteLayers[i] != null)
                {
                    selectedList[i] = _paletteLayers[i].Selected;
                }
            }

            _paletteLayers.Clear();

            var index = 0;
            foreach (TerrainLayer layer in terrain.terrainData.terrainLayers)
            {
                if (layer != null)
                {
                    var paletteLayer = new Layer
                    {
                        AssignedLayer = layer, Selected = selectedList.ElementAtOrDefault(index)
                    };
                    _paletteLayers.Add(paletteLayer);
                    index++;
                }
            }
        }

        private static void ChangeTerrainLayer(Runtime.Core.SelectionDatas.Group.Group group)
        {
            List<Prototype> selectedPrototypes = group.GetAllSelectedPrototypes();

            if (selectedPrototypes.Count == 1)
            {
                var prototypeTerrainTexture =
                    (Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture.PrototypeTerrainTexture)
                    selectedPrototypes[0];

                prototypeTerrainTexture.TerrainLayer = (TerrainLayer)CustomEditorGUILayout.ObjectField(
                    new GUIContent("Terrain Layer"), prototypeTerrainTexture.TerrainLayer == null,
                    prototypeTerrainTexture.TerrainLayer, typeof(TerrainLayer));
            }
        }

        private static void DrawSelectedWindowForTerrainTextureResources(Terrain terrain,
            Runtime.Core.SelectionDatas.Group.Group group)
        {
            var dragAndDrop = false;

            Color initialGUIColor = GUI.color;

            Event e = Event.current;

            Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, _prototypeWindowHeight)));
            windowRect = EditorGUI.IndentedRect(windowRect);

            if (IsNecessaryToDrawIconsForPrototypes(windowRect, initialGUIColor, terrain, ref dragAndDrop))
            {
                SelectedWindowUtility.DrawLabelForIcons(initialGUIColor, windowRect);

                UpdateLayerPalette(terrain);
                DrawProtoTerrainTextureIcons(e, group, _paletteLayers, windowRect);

                SelectedWindowUtility.DrawResizeBar(e, _protoIconHeight, ref _prototypeWindowHeight);
            }
            else
            {
                SelectedWindowUtility.DrawResizeBar(e, _protoIconHeight, ref _prototypeWindowHeight);
            }
        }

        private static bool IsNecessaryToDrawIconsForPrototypes(Rect brushWindowRect, Color initialGUIColor,
            Terrain terrain, ref bool dragAndDrop)
        {
            if (terrain.terrainData.terrainLayers.Length == 0)
            {
                SelectedWindowUtility.DrawLabelForIcons(initialGUIColor, brushWindowRect,
                    "Missing \"Terrain Layers\" on terrain");
                dragAndDrop = true;
                return false;
            }

            dragAndDrop = true;
            return true;
        }

        private static void DrawProtoTerrainTextureIcons(Event e, Runtime.Core.SelectionDatas.Group.Group group,
            List<Layer> paletteLayers, Rect windowRect)
        {
            Layer draggingPrototypeTerrainTexture = null;
            if (_dragAndDrop.IsDragging() || _dragAndDrop.IsDragPerform())
            {
                if (_dragAndDrop.GetData() is Layer)
                {
                    draggingPrototypeTerrainTexture = (Layer)_dragAndDrop.GetData();
                }
            }

            Rect virtualRect =
                SelectedWindowUtility.GetVirtualRect(windowRect, paletteLayers.Count, _protoIconWidth,
                    _protoIconHeight);

            Vector2 windowScrollPos = _prototypeWindowsScroll;
            windowScrollPos = GUI.BeginScrollView(windowRect, windowScrollPos, virtualRect, false, true);

            var y = (int)virtualRect.yMin;
            var x = (int)virtualRect.xMin;

            for (var protoIndexForGUI = 0; protoIndexForGUI < paletteLayers.Count; protoIndexForGUI++)
            {
                var iconRect = new Rect(x, y, _protoIconWidth, _protoIconHeight);

                SetColorForIcon(paletteLayers[protoIndexForGUI].Selected, out Color textColor, out Color rectColor);
                SetIconInfoForTexture(paletteLayers[protoIndexForGUI].AssignedLayer, out Texture2D preview,
                    out var name);

                var iconRectScrolled = new Rect(iconRect);
                iconRectScrolled.position -= windowScrollPos;

                // only visible icons
                if (iconRectScrolled.Overlaps(windowRect))
                {
                    if (iconRect.Contains(e.mousePosition))
                    {
                        _dragAndDrop.AddDragObject(paletteLayers[protoIndexForGUI]);

                        if (draggingPrototypeTerrainTexture != null && e.type == EventType.Repaint)
                        {
                            if (_dragAndDrop.IsDragPerform())
                            {
                                Move(paletteLayers, GetSelectedIndexLayer(), protoIndexForGUI);
                                SetToTerrainLayers(group);
                                TerrainResourcesController.SyncTerrainID(Terrain.activeTerrain, group);
                            }

                            EditorGUI.DrawRect(iconRect, Color.white.WithAlpha(0.3f));
                        }

                        HandleSelectLayer(protoIndexForGUI, group, e);
                    }

                    DrawIcon(e, iconRect, preview, name, textColor, rectColor, _protoIconWidth, false);
                }

                SelectedWindowUtility.SetNextXYIcon(virtualRect, _protoIconWidth, _protoIconHeight, ref y, ref x);
            }

            _prototypeWindowsScroll = windowScrollPos;

            GUI.EndScrollView();
        }

        private static void DrawIcon(Event e, Rect iconRect, Texture2D preview, string name, Color textColor,
            Color rectColor,
            int iconWidth, bool drawTextureWithAlphaChannel)
        {
            GUIStyle labelTextForIcon = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForIcon);

            EditorGUI.DrawRect(iconRect, rectColor);

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

        private static void SetColorForIcon(bool selected, out Color textColor, out Color rectColor)
        {
            textColor = EditorColors.Instance.LabelColor;

            if (selected)
            {
                rectColor = EditorColors.Instance.ToggleButtonActiveColor;
            }
            else
            {
                rectColor = EditorColors.Instance.ToggleButtonInactiveColor;
            }
        }

        private static void SetIconInfoForTexture(TerrainLayer protoTerrainTexture, out Texture2D preview,
            out string name)
        {
            if (protoTerrainTexture.diffuseTexture != null)
            {
                preview = protoTerrainTexture.diffuseTexture;
                name = protoTerrainTexture.name;
            }
            else
            {
                preview = null;
                name = "Missing Texture";
            }
        }

        private static void HandleSelectLayer(int prototypeIndex, Runtime.Core.SelectionDatas.Group.Group group,
            Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                {
                    if (e.button == 0)
                    {
                        SelectLayer(prototypeIndex);

                        e.Use();
                    }

                    break;
                }
                case EventType.ContextClick:
                {
                    PrototypeTerrainTextureMenu(group).ShowAsContext();

                    e.Use();

                    break;
                }
            }
        }

        private static void SelectLayer(int prototypeIndex)
        {
            SetSelectedAllLayer(false);

            if (prototypeIndex < 0 && prototypeIndex >= _paletteLayers.Count)
            {
                return;
            }

            _paletteLayers[prototypeIndex].Selected = true;
        }

        private static void SetSelectedAllLayer(bool select)
        {
            foreach (Layer proto in _paletteLayers)
            {
                proto.Selected = select;
            }
        }

        private static int GetSelectedIndexLayer()
        {
            for (var i = 0; i < _paletteLayers.Count; i++)
            {
                if (_paletteLayers[i].Selected)
                {
                    return i;
                }
            }

            return 0;
        }

        private static void SetToTerrainLayers(Runtime.Core.SelectionDatas.Group.Group group)
        {
            var layers = new List<TerrainLayer>();

            foreach (Layer item in _paletteLayers)
            {
                layers.Add(item.AssignedLayer);
            }

#if UNITY_2019_2_OR_NEWER
            Terrain.activeTerrain.terrainData.SetTerrainLayersRegisterUndo(layers.ToArray(), "Reorder Terrain Layers");
#else
			Terrain.activeTerrain.terrainData.terrainLayers = layers.ToArray();
#endif

            TerrainResourcesController.SyncAllTerrains(group, Terrain.activeTerrain);
        }

        private static GenericMenu PrototypeTerrainTextureMenu(Runtime.Core.SelectionDatas.Group.Group group)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Delete"), false, ContextMenuUtility.ContextMenuCallback,
                new Action(() => DeleteSelectedProtoTerrainTexture(group)));

            return menu;
        }

        private static void DeleteSelectedProtoTerrainTexture(Runtime.Core.SelectionDatas.Group.Group group)
        {
            _paletteLayers.RemoveAll(prototype => prototype.Selected);
            SetToTerrainLayers(group);
        }

        private static void Move(IList elements, int sourceIndex, int destIndex)
        {
            if (destIndex >= elements.Count)
            {
                return;
            }

            var item = (IShowIcon)elements[sourceIndex];
            elements.RemoveAt(sourceIndex);
            elements.Insert(destIndex, item);
        }

        private class Layer
        {
            public TerrainLayer AssignedLayer;
            public bool Selected;
        }
    }
}
#endif
