using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.MegaWorld.Runtime.TerrainSpawner;
using VladislavTsurikov.UnityUtility.Runtime;
using Group = VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Group;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    public class TerrainsMaskItem
    {
        private readonly Dictionary<Object, Texture2D> _terrainsMasks = new();

        public readonly MaskFilterComponentSettings MaskFilterComponentSettings;

        public TerrainsMaskItem(MaskFilterComponentSettings maskFilterComponentSettings) =>
            MaskFilterComponentSettings = maskFilterComponentSettings;

        public float GetFitness(Group group, Vector3 point) =>
            GetFitness(group, UnityTerrainUtility.GetTerrain(point), point);

        public float GetFitness(Group group, Terrain terrain, Vector3 point)
        {
            if (terrain == null)
            {
                return 0;
            }

            TerrainData terrainData = terrain.terrainData;
            var terrainBounds = new Bounds(terrainData.bounds.center + terrain.transform.position,
                terrainData.bounds.size);

            TerrainMask[] terrainMasks = terrain.GetComponents<TerrainMask>();

            float maskFitness = 1;

            foreach (TerrainMask terrainMask in terrainMasks)
            {
                if (terrainMask != null && terrainMask.IsFit() && terrainMask.Group == group)
                {
                    maskFitness = TextureUtility.GetFromWorldPosition(terrainBounds, point, terrainMask.Mask);
                    break;
                }
            }

            if (_terrainsMasks.TryGetValue(terrain, out Texture2D mask))
            {
                return Utility.GetFitness.GetFromMaskFilter(terrainBounds, MaskFilterComponentSettings.MaskFilterStack,
                    mask, point) * maskFitness;
            }

            RayHit terrainCenterRayHit = RaycastUtility.Raycast(
                RayUtility.GetRayDown(terrainBounds.center + new Vector3(0, 20, 0)),
                LayerMask.GetMask(LayerMask.LayerToName(terrain.gameObject.layer)));

            if (terrainCenterRayHit == null)
            {
                return 0;
            }

            var area = new BoxArea(terrainCenterRayHit, terrainBounds.size.x);
            Texture2D texture2D = FilterMaskOperation.UpdateMaskTexture(MaskFilterComponentSettings, area);
            _terrainsMasks.Add(terrain, texture2D);

            return Utility.GetFitness.GetFromMaskFilter(terrainBounds, MaskFilterComponentSettings.MaskFilterStack,
                texture2D,
                point) * maskFitness;
        }
    }
}
