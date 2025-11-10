using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings
{
    public abstract class PrototypeComponent : Component
    {
        public Prototype Prototype { get; private set; }

        protected override void SetupComponent(object[] setupData = null)
        {
            Prototype = (Prototype)setupData[1];
            SetupPrototypeComponent();
        }

        protected virtual void SetupPrototypeComponent()
        {
        }
    }
}
