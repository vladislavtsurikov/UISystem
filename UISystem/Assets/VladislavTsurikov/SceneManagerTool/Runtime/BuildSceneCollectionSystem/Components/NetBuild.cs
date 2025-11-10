namespace VladislavTsurikov.SceneManagerTool.Runtime.BuildSceneCollectionSystem
{
    /*[Name("Net Build")]
    public class NetBuild : BuildSceneCollection
    {
        [OdinSerialize]
        public SceneCollectionStack ServerSceneCollectionStack = new SceneCollectionStack();

        public override void OnCreate()
        {
            Name = "Net Build";
        }

        public override List<SceneReference.Scripts.SceneReference> GetSceneReferences()
        {
#if UNITY_SERVER
            return ServerSceneCollectionStack.GetSceneReferences();
#else
            return ServerSceneCollectionStack.GetSceneReferences();
#endif
        }

        public override List<SceneCollection> GetStartupSceneCollections()
        {
            throw new System.NotImplementedException();
        }

        public override List<SceneCollection> GetAllSceneCollections()
        {
            throw new System.NotImplementedException();
        }

        public override void DoBuild()
        {
            base.DoBuild();
        }
    }*/
}
