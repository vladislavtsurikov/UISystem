using System;

namespace VladislavTsurikov.SceneDataSystem.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class AllowCreateComponentAttribute : Attribute
    {
        public abstract bool Allow(SceneDataManager sceneDataManager);
    }
}
