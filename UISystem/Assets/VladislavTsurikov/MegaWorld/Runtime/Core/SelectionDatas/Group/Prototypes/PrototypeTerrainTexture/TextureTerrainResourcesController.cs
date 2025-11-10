using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture
{
    public static class TextureTerrainResourcesController
    {
        public enum TerrainResourcesSyncError
        {
            None,
            NotAllProtoAvailable,
            MissingPrototypes,
            MissingTerrainLayer
        }

        public static TerrainResourcesSyncError SyncError = TerrainResourcesSyncError.None;

        public static void RemoveAllPrototypesFromTerrains(Terrain terrain)
        {
            terrain.terrainData.terrainLayers = Array.Empty<TerrainLayer>();

            terrain.Flush();
        }


        public static void AddTerrainTexturesToTerrain(Terrain terrain,
            IReadOnlyList<Prototype> protoTerrainTextureList)
        {
            var currentTerrainLayers = terrain.terrainData.terrainLayers.ToList();
            currentTerrainLayers.RemoveAll(layer => layer == null);
            terrain.terrainData.terrainLayers = currentTerrainLayers.ToArray();

            var terrainLayers = new List<TerrainLayer>(terrain.terrainData.terrainLayers);

            foreach (PrototypeTerrainTexture prototypeTerrainTexture in protoTerrainTextureList)
            {
                var found = false;

                if (prototypeTerrainTexture.TerrainLayer != null)
                {
                    foreach (TerrainLayer layer in currentTerrainLayers)
                    {
                        if (layer.GetInstanceID() == prototypeTerrainTexture.TerrainLayer.GetInstanceID())
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        terrainLayers.Add(prototypeTerrainTexture.TerrainLayer);
                    }
                }
            }

            terrain.terrainData.terrainLayers = terrainLayers.ToArray();
        }

        public static void AddMissingTextureInTerrain(Terrain terrain, Group group)
        {
            var terrainLayers = new List<TerrainLayer>(terrain.terrainData.terrainLayers);

            foreach (TerrainLayer t in terrainLayers)
            {
                var find = false;

                foreach (PrototypeTerrainTexture texture in group.PrototypeList)
                {
                    if (texture.TerrainLayer != null)
                    {
                        if (t.GetInstanceID() == texture.TerrainLayer.GetInstanceID())
                        {
                            find = true;
                        }
                    }
                }

                if (find == false)
                {
                    group.AddMissingPrototype(t);
                }
            }
        }

        public static void RemoveMissingTextureInTerrain(Terrain terrain, Group group)
        {
            var protoTerrainTextureRemoveList = new List<PrototypeTerrainTexture>();

            var terrainLayers = new List<TerrainLayer>(terrain.terrainData.terrainLayers);

            foreach (PrototypeTerrainTexture texture in group.PrototypeList)
            {
                var find = false;

                if (texture.TerrainLayer != null)
                {
                    foreach (TerrainLayer t in terrainLayers)
                    {
                        Debug.Log(texture.TerrainLayer.name + " " + t.name);
                        if (t.GetInstanceID() == texture.TerrainLayer.GetInstanceID())
                        {
                            find = true;
                        }
                    }
                }

                if (find == false)
                {
                    protoTerrainTextureRemoveList.Add(texture);
                }
            }

            foreach (PrototypeTerrainTexture proto in protoTerrainTextureRemoveList)
            {
                group.PrototypeList.Remove(proto);
            }
        }

        public static void UpdatePrototypesFromTerrain(Terrain terrain, Group group)
        {
            if (terrain == null)
            {
                Debug.LogWarning("Missing active terrain.");
                return;
            }

            RemoveMissingTextureInTerrain(terrain, group);
            AddMissingTextureInTerrain(terrain, group);
        }

        public static void DetectSyncError(Group group, Terrain terrain)
        {
            if (terrain == null)
            {
                return;
            }

            if (group.PrototypeList.Count == 0)
            {
                SyncError = TerrainResourcesSyncError.MissingPrototypes;
                return;
            }

            foreach (PrototypeTerrainTexture proto in group.PrototypeList)
            {
                if (proto.TerrainLayer == null)
                {
                    SyncError = TerrainResourcesSyncError.MissingTerrainLayer;
                    break;
                }

                var find = false;

                foreach (TerrainLayer terrainLayer in terrain.terrainData.terrainLayers)
                {
                    if (terrainLayer == null)
                    {
                        continue;
                    }

                    if (terrainLayer.GetInstanceID() == proto.TerrainLayer.GetInstanceID())
                    {
                        find = true;
                        break;
                    }
                }

                if (!find)
                {
                    SyncError = TerrainResourcesSyncError.NotAllProtoAvailable;
                    return;
                }
            }

            SyncError = TerrainResourcesSyncError.None;
        }

        public static bool IsSyncError(Group group, Terrain terrain)
        {
            DetectSyncError(group, terrain);

            if (TerrainResourcesSyncError.NotAllProtoAvailable == SyncError)
            {
                if (group.PrototypeType == typeof(PrototypeTerrainTexture))
                {
                    Debug.LogWarning(
                        "You need all Terrain Textures prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");
                    return true;
                }
            }

            return false;
        }

        public static void RemoveAllNullTerrainData(Terrain terrain)
        {
            var currentTerrainLayers = new List<TerrainLayer>(terrain.terrainData.terrainLayers);
            currentTerrainLayers.RemoveAll(layer => layer == null);
            terrain.terrainData.terrainLayers = currentTerrainLayers.ToArray();
        }

        public static void SyncAllTerrains(Group group, Terrain terrain)
        {
            if (group.PrototypeType == typeof(PrototypeTerrainTexture))
            {
                var terrainTextures = new List<TerrainLayer>(terrain.terrainData.terrainLayers);

                foreach (Terrain item in Terrain.activeTerrains)
                {
                    item.terrainData.terrainLayers = terrainTextures.ToArray();
                }
            }
        }
    }
}
