using OdinSerializer;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem
{
    [Name("Extension")]
    public class ExtensionSystem : GlobalComponent
    {
        [OdinSerialize]
        public ComponentStackOnlyDifferentTypes<Extension> ExtensionStack = new();

        protected override void SetupComponent(object[] setupData = null) => ExtensionStack.Setup();
    }
}
