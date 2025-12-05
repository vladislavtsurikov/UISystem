using System.Collections.Generic;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem
{
    public abstract class SettingsComponent : Component
    {
        public virtual List<SceneReference> GetSceneReferences() => new();
    }
}
