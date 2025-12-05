using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.TerrainSpawner.Utility
{
    public static class SpawnPrototype
    {
        public static void SpawnTerrainDetails(Group group, PrototypeTerrainDetail proto,
            TerrainsMaskManager terrainsMaskManager, BoxArea boxArea, Terrain terrain)
        {
            TerrainData terrainData = terrain.terrainData;
            var spawnSize = new Vector2Int(
                UnityTerrainUtility.WorldToDetail(boxArea.Size.x, terrainData.size.x, terrainData),
                UnityTerrainUtility.WorldToDetail(boxArea.Size.z, terrainData.size.z, terrainData));

            Vector2Int halfBrushSize = spawnSize / 2;

            var center = new Vector2Int(
                UnityTerrainUtility.WorldToDetail(boxArea.Center.x - terrain.transform.position.x, terrain.terrainData),
                UnityTerrainUtility.WorldToDetail(boxArea.Center.z - terrain.transform.position.z,
                    terrain.terrainData.size.z, terrain.terrainData));

            Vector2Int position = center - halfBrushSize;
            var startPosition = Vector2Int.Max(position, Vector2Int.zero);

            float detailMapResolution = terrain.terrainData.detailResolution;

            var localData = terrain.terrainData.GetDetailLayer(
                startPosition.x, startPosition.y,
                Mathf.Max(0, Mathf.Min(position.x + spawnSize.x, (int)detailMapResolution) - startPosition.x),
                Mathf.Max(0, Mathf.Min(position.y + spawnSize.y, (int)detailMapResolution) - startPosition.y),
                proto.TerrainProtoId);

            float widthY = localData.GetLength(1);
            float heightX = localData.GetLength(0);

            var maskFilterComponentSettings =
                (MaskFilterComponentSettings)proto.GetElement(typeof(MaskFilterComponentSettings));
            var spawnDetailSettings = (SpawnDetailSettings)proto.GetElement(typeof(SpawnDetailSettings));

            for (var y = 0; y < widthY; y++)
            for (var x = 0; x < heightX; x++)
            {
                var current = new Vector2Int(y, x);

                Vector2 normal = current + startPosition;
                normal /= detailMapResolution;

                Vector2 worldPosition = UnityTerrainUtility.GetTerrainWorldPositionFromRange(normal, terrain);

                var fitness = GetFitness.Get(group, terrainsMaskManager, maskFilterComponentSettings, terrain,
                    new Vector3(worldPosition.x, 0, worldPosition.y));

                int targetStrength;

                if (spawnDetailSettings.UseRandomOpacity)
                {
                    var randomFitness = fitness;
                    randomFitness *= Random.value;

                    targetStrength = Mathf.RoundToInt(Mathf.Lerp(0, spawnDetailSettings.Density, randomFitness));
                }
                else
                {
                    targetStrength = Mathf.RoundToInt(Mathf.Lerp(0, spawnDetailSettings.Density, fitness));
                }

                if (Random.Range(0f, 1f) < 1 - fitness || Random.Range(0f, 1f) < spawnDetailSettings.FailureRate / 100)
                {
                    targetStrength = 0;
                }

                localData[x, y] = targetStrength;
            }

            terrain.terrainData.SetDetailLayer(startPosition.x, startPosition.y, proto.TerrainProtoId, localData);
        }
    }
}
