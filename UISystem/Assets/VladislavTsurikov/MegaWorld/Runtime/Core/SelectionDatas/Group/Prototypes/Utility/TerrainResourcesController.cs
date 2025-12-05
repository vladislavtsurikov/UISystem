using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility
{
    public static class TerrainResourcesController
    {
        public static void AddMissingPrototypesToTerrains(Group group)
        {
            RemoveAllNullTerrainData();

            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                AddMissingPrototypesToTerrain(terrain, group);
            }
        }

        public static void RemoveAllPrototypesFromTerrains(Group group)
        {
            RemoveAllNullTerrainData();

            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                RemoveAllPrototypesFromTerrains(terrain, group);
            }
        }

        public static void AddMissingPrototypesToTerrain(Terrain terrain, Group group)
        {
            if (terrain == null)
            {
                Debug.LogWarning("Can not add resources to the terrain as no terrain has been supplied.");
                return;
            }

            if (group.PrototypeType == typeof(PrototypeTerrainDetail.PrototypeTerrainDetail))
            {
                DetailTerrainResourcesController.AddTerrainDetailToTerrain(terrain, group.PrototypeList);
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainTexture.PrototypeTerrainTexture))
            {
                TextureTerrainResourcesController.AddTerrainTexturesToTerrain(terrain, group.PrototypeList);
            }

            terrain.Flush();

            SyncTerrainID(terrain, group);
        }

        public static void RemoveAllPrototypesFromTerrains(Terrain terrain, Group group)
        {
            if (terrain == null)
            {
                Debug.LogWarning("Can not add resources to the terrain as no terrain has been supplied.");
                return;
            }

            if (group.PrototypeType == typeof(PrototypeTerrainDetail.PrototypeTerrainDetail))
            {
                DetailTerrainResourcesController.RemoveAllPrototypesFromTerrains(terrain);
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainTexture.PrototypeTerrainTexture))
            {
                TextureTerrainResourcesController.RemoveAllPrototypesFromTerrains(terrain);
            }

            terrain.Flush();

            SyncTerrainID(terrain, group);
        }

        public static void SyncTerrainID(Terrain terrain, Group group) =>
            DetailTerrainResourcesController.SyncTerrainID(terrain, group);

        public static void DetectSyncError(Group group, Terrain terrain)
        {
            if (terrain == null)
            {
                return;
            }

            if (group.PrototypeType == typeof(PrototypeTerrainDetail.PrototypeTerrainDetail))
            {
                DetailTerrainResourcesController.DetectSyncError(group, terrain);
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainTexture.PrototypeTerrainTexture))
            {
                TextureTerrainResourcesController.DetectSyncError(group, terrain);
            }
        }

        public static bool IsSyncError(Group group, Terrain terrain)
        {
            DetectSyncError(group, terrain);

            if (group.PrototypeType == typeof(PrototypeTerrainDetail.PrototypeTerrainDetail))
            {
                return DetailTerrainResourcesController.IsSyncError(group, terrain);
            }

            if (group.PrototypeType == typeof(PrototypeTerrainTexture.PrototypeTerrainTexture))
            {
                return TextureTerrainResourcesController.IsSyncError(group, terrain);
            }

            return false;
        }

        public static void RemoveAllNullTerrainData()
        {
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                TextureTerrainResourcesController.RemoveAllNullTerrainData(terrain);
                DetailTerrainResourcesController.RemoveAllNullTerrainData(terrain);
            }
        }

        public static void SyncAllTerrains(Group group, Terrain terrain)
        {
            if (group == null || terrain == null)
            {
                return;
            }

            if (Terrain.activeTerrains.Length == 0)
            {
                return;
            }

            if (group.PrototypeType == typeof(PrototypeTerrainDetail.PrototypeTerrainDetail))
            {
                DetailTerrainResourcesController.SyncAllTerrains(group, terrain);
            }

            if (group.PrototypeType == typeof(PrototypeTerrainTexture.PrototypeTerrainTexture))
            {
                TextureTerrainResourcesController.SyncAllTerrains(group, terrain);
            }
        }
    }
}
