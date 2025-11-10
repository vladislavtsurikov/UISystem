using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility
{
    public static class FindSector
    {
        public static List<Sector> OverlapSphere(Vector3 center, float radius, string sectorLayerTag = null,
            bool objectBounds = true, bool sort = true)
        {
            var findSectors = new List<Sector>();

            List<SectorLayer> sectorLayers = SectorLayer.GetCurrentSectorLayers(sectorLayerTag);

            if (sectorLayers == null)
            {
                return new List<Sector>();
            }

            foreach (SectorLayer sectorLayer in sectorLayers)
            {
                findSectors.AddRange(objectBounds
                    ? sectorLayer.ObjectBoundsBVHTree.OverlapSphere(center, radius)
                    : sectorLayer.SectorBvhTree.OverlapSphere(center, radius));
            }

            if (sort)
            {
                SortCloserToCenter(findSectors, center);
            }

            return findSectors;
        }

        public static List<Sector> OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation,
            string sectorLayerTag = null, bool objectBounds = true, bool sort = true)
        {
            var findSectors = new List<Sector>();

            List<SectorLayer> sectorLayers = SectorLayer.GetCurrentSectorLayers(sectorLayerTag);

            if (sectorLayers == null)
            {
                return new List<Sector>();
            }

            foreach (SectorLayer sectorLayer in sectorLayers)
            {
                findSectors.AddRange(objectBounds
                    ? sectorLayer.ObjectBoundsBVHTree.OverlapBox(boxCenter, boxSize, boxRotation)
                    : sectorLayer.SectorBvhTree.OverlapBox(boxCenter, boxSize, boxRotation));
            }

            if (sort)
            {
                SortCloserToCenter(findSectors, boxCenter);
            }

            return findSectors;
        }

        public static List<Sector> OverlapPosition(Vector3 position, string sectorLayerTag = null,
            bool objectBounds = true)
        {
            var findSectors = new List<Sector>();

            List<SectorLayer> sectorLayers = SectorLayer.GetCurrentSectorLayers(sectorLayerTag);

            if (sectorLayers == null)
            {
                return new List<Sector>();
            }

            foreach (SectorLayer sectorLayer in sectorLayers)
            {
                findSectors.AddRange(objectBounds
                    ? sectorLayer.ObjectBoundsBVHTree.OverlapPosition(position)
                    : sectorLayer.SectorBvhTree.OverlapPosition(position));
            }

            return findSectors;
        }

        private static void SortCloserToCenter(List<Sector> sectors, Vector3 center)
        {
            if (sectors == null)
            {
                return;
            }

            sectors.Sort(delegate(Sector h0, Sector h1)
            {
                var distance0 = Vector3.Distance(center, h0.Bounds.center);
                var distance1 = Vector3.Distance(center, h1.Bounds.center);
                return distance0.CompareTo(distance1);
            });
        }
    }
}
