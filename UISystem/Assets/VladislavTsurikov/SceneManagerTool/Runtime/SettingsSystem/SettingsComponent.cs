using System.Collections.Generic;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem
{
    public abstract class SettingsComponent : Node
    {
        public virtual List<SceneReference> GetSceneReferences() => new();
    }
}
