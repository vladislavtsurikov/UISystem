using System.Collections.Generic;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem
{
    public class SceneCollectionStack : ComponentStackSupportSameType<SceneCollection>
    {
        public List<SceneReference> GetSceneReferences()
        {
            var sceneReferences = new List<SceneReference>();

            foreach (SceneCollection sceneCollection in ElementList)
            {
                sceneReferences.AddRange(sceneCollection.GetSceneReferences());
            }

            return sceneReferences;
        }
    }
}
