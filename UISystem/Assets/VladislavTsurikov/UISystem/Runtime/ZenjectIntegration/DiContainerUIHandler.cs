using VladislavTsurikov.UISystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime
{
    public abstract class DiContainerUIHandler : UIHandler
    {
        protected readonly DiContainer _container;

        protected DiContainerUIHandler(DiContainer container) => _container = container;
    }
}
