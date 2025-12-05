using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using Group = VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Group;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    public class TerrainsMaskManager
    {
        private readonly List<TerrainsMaskItem> _itemList = new();

        public float GetFitness(Group group, MaskFilterComponentSettings maskFilterComponentSettings, Vector3 point)
        {
            foreach (TerrainsMaskItem item in _itemList)
            {
                if (item.MaskFilterComponentSettings == maskFilterComponentSettings)
                {
                    return item.GetFitness(group, point);
                }
            }

            var localTerrainsMaskItem = new TerrainsMaskItem(maskFilterComponentSettings);
            _itemList.Add(localTerrainsMaskItem);
            return localTerrainsMaskItem.GetFitness(group, point);
        }

        public float GetFitness(Group group, MaskFilterComponentSettings maskFilterComponentSettings, Terrain terrain,
            Vector3 point)
        {
            foreach (TerrainsMaskItem item in _itemList)
            {
                if (item.MaskFilterComponentSettings == maskFilterComponentSettings)
                {
                    return item.GetFitness(group, terrain, point);
                }
            }

            var localTerrainsMaskItem = new TerrainsMaskItem(maskFilterComponentSettings);
            _itemList.Add(localTerrainsMaskItem);
            return localTerrainsMaskItem.GetFitness(group, terrain, point);
        }

        public void Dispose() => _itemList.Clear();
    }
}
