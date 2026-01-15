using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem
{
    public class SceneTypeComponentStack : NodeStackSupportSameType<SceneType>
    {
        public bool HasScene(SceneReference sceneReference)
        {
            foreach (SceneType scene in ElementList)
            {
                if (scene.HasScene(sceneReference))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
