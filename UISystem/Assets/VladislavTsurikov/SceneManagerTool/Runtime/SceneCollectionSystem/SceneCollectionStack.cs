using System.Collections.Generic;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem
{
    public class SceneCollectionStack : NodeStackSupportSameType<SceneCollection>
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
