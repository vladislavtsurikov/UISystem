using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UIRootSystem.Runtime.PrefabResourceLoaders;
using VladislavTsurikov.UISystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.UIRootSystem.Runtime.Layers
{
    [ParentUIHandler(typeof(UIRoot))]
    [SceneFilter("TestScene_1", "TestScene_2")]
    public class LayerOverScreens : UILayer
    {
        public LayerOverScreens(DiContainer container, LayerOverScreensLoader loader)
            : base(container, loader)
        {
        }

        public override int GetLayerIndex() => 3;

        protected override async UniTask InitializeUIHandler(CancellationToken cancellationToken,
            CompositeDisposable disposables) => await Show(cancellationToken);
    }
}
