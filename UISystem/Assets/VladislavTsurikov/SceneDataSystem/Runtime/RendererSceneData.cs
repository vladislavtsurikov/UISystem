using VladislavTsurikov.Math.Runtime;

namespace VladislavTsurikov.SceneDataSystem.Runtime
{
    public class RendererSceneData : SceneData
    {
        public virtual AABB GetAABB() => new();
    }
}
