using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility
{
    public static class StreamingUtility
    {
        public static void LoadScenes(List<Sector> sectors)
        {
            foreach (Sector sector in sectors)
            {
                sector.LoadScene();
            }
        }

        public static void CacheScenes(List<Sector> sectors, float waitForSeconds = 0F, float keepScene = 0F)
        {
            foreach (Sector sector in sectors)
            {
                sector.CacheScene(waitForSeconds, keepScene);
            }
        }

        public static void UnloadScenes(List<Sector> sectors, float waitForSeconds = 0F)
        {
            foreach (Sector sector in sectors)
            {
                sector.UnloadScene(waitForSeconds);
            }
        }

        public static async UniTask UnloadAllScenes(string tag)
        {
            foreach (SectorLayer sceneDataManager in SectorLayerManager.Instance.SectorLayerList)
            {
                if (sceneDataManager.Tag == tag)
                {
                    foreach (Sector sector in sceneDataManager.SectorList)
                    {
                        await sector.SceneReference.UnloadScene();
                    }
                }
            }
        }

        public static async UniTask LoadAllScenes(string tag)
        {
            foreach (SectorLayer sceneDataManager in SectorLayerManager.Instance.SectorLayerList)
            {
                if (sceneDataManager.Tag == tag)
                {
                    foreach (Sector sector in sceneDataManager.SectorList)
                    {
                        await sector.SceneReference.LoadScene();
                    }
                }
            }
        }

        public static List<Sector> GetAllScenes(string tag)
        {
            SectorLayer sectorLayer = SectorLayerManager.Instance.GetSectorLayer(tag);

            if (sectorLayer == null)
            {
                return new List<Sector>();
            }

            return sectorLayer.SectorList;
        }
    }
}
