#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.RendererStack.Editor.Core.RendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.RenderManager.GPUInstancedIndirect;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.UnityUtility.Editor;
using Renderer = VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem.Renderer;

namespace VladislavTsurikov.RendererStack.Editor.TerrainObjectRenderer
{
    public class TerrainObjectRendererMenu : RendererMenu
    {
        public override void ShowGenericMenu(GenericMenu menu, Renderer renderer)
        {
            var terrainObjectRenderer = (Runtime.TerrainObjectRenderer.TerrainObjectRenderer)renderer;

            menu.AddItem(new GUIContent("Regenerate Instanced Indirect Shaders"), false,
                ContextMenuUtility.ContextMenuCallback,
                new Action(() => { GPUInstancedIndirectShaderStack.Instance.RegenerateShaders(); }));

            menu.AddItem(new GUIContent("Terrain Cells Occlusion Culling/Refresh Cells"), false,
                ContextMenuUtility.ContextMenuCallback,
                new Action(TerrainObjectRendererData.RefreshAllCells));

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Debug/Terrain Cells Occlusion Culling/All Cells"),
                terrainObjectRenderer.DebugAllCells, ContextMenuUtility.ContextMenuCallback, new Action(() =>
                    terrainObjectRenderer.DebugAllCells = !terrainObjectRenderer.DebugAllCells));

            menu.AddItem(new GUIContent("Debug/Terrain Cells Occlusion Culling/Visible Cells"),
                terrainObjectRenderer.DebugVisibleCells, ContextMenuUtility.ContextMenuCallback, new Action(() =>
                    terrainObjectRenderer.DebugVisibleCells = !terrainObjectRenderer.DebugVisibleCells));
        }
    }
}
#endif
