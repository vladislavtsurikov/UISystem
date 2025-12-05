using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem
{
    public class SceneTypeComponentStack : ComponentStackSupportSameType<SceneType>
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
