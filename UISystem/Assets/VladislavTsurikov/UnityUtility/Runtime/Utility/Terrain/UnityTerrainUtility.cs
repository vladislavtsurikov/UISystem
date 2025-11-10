using UnityEngine;

namespace VladislavTsurikov.UnityUtility.Runtime
{
    public static class UnityTerrainUtility
    {
        public static int WorldToDetail(float pos, float terrainSize, TerrainData td) =>
            Mathf.RoundToInt(pos / terrainSize * td.detailResolution);

        public static int WorldToDetail(float pos, TerrainData terrainData) =>
            WorldToDetail(pos, terrainData.size.x, terrainData);

        public static Vector2 GetTerrainWorldPositionFromRange(Vector2 normal, Terrain terrain)
        {
            var localTerrainPosition = new Vector2(Mathf.Lerp(0, terrain.terrainData.size.x, normal.x),
                Mathf.Lerp(0, terrain.terrainData.size.z, normal.y));
            return localTerrainPosition + new Vector2(terrain.GetPosition().x, terrain.GetPosition().z);
        }

        public static Vector2 WorldPointToUV(Vector3 point, Terrain activeTerrain)
        {
            if (activeTerrain == null)
            {
                return Vector2.zero;
            }

            var terrainSize = new Vector3(activeTerrain.terrainData.size.x, activeTerrain.terrainData.size.y,
                activeTerrain.terrainData.size.z);

            var uv = new Vector2(
                Mathf.InverseLerp(0 + activeTerrain.GetPosition().x, terrainSize.x + activeTerrain.GetPosition().x,
                    point.x),
                Mathf.InverseLerp(0 + activeTerrain.GetPosition().z, terrainSize.z + activeTerrain.GetPosition().z,
                    point.z));

            return uv;
        }

        public static Terrain GetTerrain(Vector3 location)
        {
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                Vector3 terrainMin = terrain.GetPosition();
                Vector3 terrainMax = terrainMin + terrain.terrainData.size;
                if (location.x >= terrainMin.x && location.x <= terrainMax.x)
                {
                    if (location.z >= terrainMin.z && location.z <= terrainMax.z)
                    {
                        return terrain;
                    }
                }
            }

            return null;
        }

        public static Vector2 GetTextureCoordFromUnityTerrain(Vector3 origin)
        {
            var ray = new Ray(origin + new Vector3(0, 10, 0), Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 6500f,
                    LayerMask.GetMask(LayerMask.LayerToName(Terrain.activeTerrain.gameObject.layer))))
            {
                return hitInfo.textureCoord;
            }

            return Vector2.zero;
        }
    }
}
