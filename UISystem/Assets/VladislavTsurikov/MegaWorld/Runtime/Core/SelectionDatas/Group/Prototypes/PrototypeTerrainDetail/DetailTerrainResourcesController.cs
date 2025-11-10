using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.UnityUtility.Runtime;
using GameObjectUtility = VladislavTsurikov.UnityUtility.Runtime.GameObjectUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail
{
    public static class DetailTerrainResourcesController
    {
        public enum TerrainResourcesSyncError
        {
            None,
            NotAllProtoAvailable,
            MissingPrototypes
        }

        public static TerrainResourcesSyncError SyncError = TerrainResourcesSyncError.None;

        public static void RemoveAllPrototypesFromTerrains(Terrain terrain)
        {
            terrain.terrainData.detailPrototypes = Array.Empty<DetailPrototype>();

            terrain.Flush();
        }

        public static void AddTerrainDetailToTerrain(Terrain terrain, IReadOnlyList<Prototype> protoTerrainDetailList)
        {
            bool found;

            DetailPrototype newDetail;
            var terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);
            foreach (PrototypeTerrainDetail proto in protoTerrainDetailList)
            {
                found = false;
                foreach (DetailPrototype dp in terrainDetails)
                {
                    if (proto.PrefabType == PrefabType.Texture)
                    {
                        if (TextureUtility.IsSameTexture(dp.prototypeTexture, proto.DetailTexture))
                        {
                            found = true;
                        }

                        if (GameObjectUtility.IsSameGameObject(dp.prototype, proto.Prefab))
                        {
                            found = true;
                        }
                    }
                }

                if (!found)
                {
                    newDetail = new DetailPrototype();

                    if (proto.PrefabType == PrefabType.Texture)
                    {
                        newDetail.renderMode = DetailRenderMode.GrassBillboard;
                        newDetail.usePrototypeMesh = false;
                        newDetail.prototypeTexture = proto.DetailTexture;
                    }
                    else
                    {
                        newDetail.renderMode = DetailRenderMode.Grass;
                        newDetail.usePrototypeMesh = true;
                        newDetail.prototype = proto.Prefab;
                    }

                    terrainDetails.Add(newDetail);
                }
            }

            terrain.terrainData.detailPrototypes = terrainDetails.ToArray();
        }

        public static void SyncTerrainID(Terrain terrain, Group group)
        {
            if (group.PrototypeType == typeof(PrototypeTerrainDetail))
            {
                var terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);

                foreach (Prototype prototype in group.PrototypeList)
                {
                    var proto = (PrototypeTerrainDetail)prototype;

                    for (var id = 0; id < terrainDetails.Count; id++)
                    {
                        if (TextureUtility.IsSameTexture(terrainDetails[id].prototypeTexture, proto.DetailTexture))
                        {
                            proto.TerrainProtoId = id;
                        }

                        if (GameObjectUtility.IsSameGameObject(terrainDetails[id].prototype, proto.Prefab))
                        {
                            proto.TerrainProtoId = id;
                        }
                    }
                }
            }
        }

        public static void UpdatePrototypesFromTerrain(Terrain terrain, Group group)
        {
            if (terrain == null)
            {
                Debug.LogWarning("Missing active terrain.");
                return;
            }

            var protoTerrainDetailRemoveList = new List<PrototypeTerrainDetail>();

            var terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);

            foreach (PrototypeTerrainDetail proto in group.PrototypeList)
            {
                var find = false;

                for (var id = 0; id < terrainDetails.Count; id++)
                {
                    if (TextureUtility.IsSameTexture(terrainDetails[id].prototypeTexture, proto.DetailTexture))
                    {
                        find = true;
                    }

                    if (GameObjectUtility.IsSameGameObject(terrainDetails[id].prototype, proto.Prefab))
                    {
                        find = true;
                    }
                }

                if (find == false)
                {
                    protoTerrainDetailRemoveList.Add(proto);
                }
            }

            foreach (PrototypeTerrainDetail proto in protoTerrainDetailRemoveList)
            {
                group.PrototypeList.Remove(proto);
            }

            DetailPrototype unityProto;
            PrototypeTerrainDetail localProto;

            for (var id = 0; id < terrainDetails.Count; id++)
            {
                var find = false;

                foreach (PrototypeTerrainDetail proto in group.PrototypeList)
                {
                    if (TextureUtility.IsSameTexture(terrainDetails[id].prototypeTexture, proto.DetailTexture))
                    {
                        find = true;
                    }

                    if (GameObjectUtility.IsSameGameObject(terrainDetails[id].prototype, proto.Prefab))
                    {
                        find = true;
                    }
                }

                if (find == false)
                {
                    unityProto = terrain.terrainData.detailPrototypes[id];

                    if (unityProto.prototype != null)
                    {
                        localProto = (PrototypeTerrainDetail)group.AddMissingPrototype(unityProto.prototype);
                        localProto.PrefabType = PrefabType.Mesh;
                    }
                    else
                    {
                        localProto = (PrototypeTerrainDetail)group.AddMissingPrototype(unityProto.prototypeTexture);
                        localProto.PrefabType = PrefabType.Texture;
                    }
                }
            }

            SyncTerrainID(terrain, group);
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

            if (group.PrototypeType == typeof(PrototypeTerrainDetail))
            {
                foreach (PrototypeTerrainDetail proto in group.PrototypeList)
                {
                    var find = false;

                    foreach (DetailPrototype unityProto in terrain.terrainData.detailPrototypes)
                    {
                        if (TextureUtility.IsSameTexture(unityProto.prototypeTexture, proto.DetailTexture))
                        {
                            find = true;
                            break;
                        }

                        if (GameObjectUtility.IsSameGameObject(unityProto.prototype, proto.Prefab))
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
            }

            SyncError = TerrainResourcesSyncError.None;
        }

        public static bool IsSyncError(Group group, Terrain terrain)
        {
            DetectSyncError(group, terrain);

            if (TerrainResourcesSyncError.NotAllProtoAvailable == SyncError)
            {
                if (group.PrototypeType == typeof(PrototypeTerrainDetail))
                {
                    Debug.LogWarning(
                        "You need all Terrain Details prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");
                    return true;
                }
            }

            return false;
        }

        public static void RemoveAllNullTerrainData(Terrain terrain)
        {
            var currentDetailPrototypes = terrain.terrainData.detailPrototypes.ToList();
            currentDetailPrototypes.RemoveAll(prototype => prototype == null);
            terrain.terrainData.detailPrototypes = currentDetailPrototypes.ToArray();
        }

        public static void SyncAllTerrains(Group group, Terrain terrain)
        {
            if (group.PrototypeType == typeof(PrototypeTerrainDetail))
            {
                var terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);

                foreach (Terrain item in Terrain.activeTerrains)
                {
                    item.terrainData.detailPrototypes = terrainDetails.ToArray();
                }
            }
        }
    }
}
