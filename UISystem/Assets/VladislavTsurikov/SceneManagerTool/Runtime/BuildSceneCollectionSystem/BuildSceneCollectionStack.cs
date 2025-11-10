using System.Collections.Generic;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.BuildSceneCollectionSystem
{
    public class BuildSceneCollectionStack : ComponentStackSupportSameType<BuildSceneCollection>
    {
        public BuildSceneCollection ActiveBuildSceneCollection;

        public virtual List<SceneReference> GetSceneReferences()
        {
            if (ActiveBuildSceneCollection == null)
            {
                return new List<SceneReference>();
            }

            return ActiveBuildSceneCollection.GetSceneReferences();
        }
    }
}
