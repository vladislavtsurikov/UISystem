using System;
using VladislavTsurikov.Core.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Core
{
    [Serializable]
    public class MegaWorldPath
    {
        public static string MegaWorld = "MegaWorld";

        public static string Groups = "Groups";
        public static string PolarisBrushes = "Polaris Brushes";
        public static string MegaWorldTerrainLayers = "Mega World Terrain Layers";

        public static string PathToResourcesMegaWorld = CommonPath.CombinePath(CommonPath.PathToResources, MegaWorld);
        public static string PathToGroup = CommonPath.CombinePath(PathToResourcesMegaWorld, Groups);
        public static string TerrainLayerStoragePath = CommonPath.CombinePath("Assets", MegaWorldTerrainLayers);
    }
}
