#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public abstract class UIToolkitLayoutLoader : AddressableAssetLoader<VisualTreeAsset>
    {
        protected override string AssetAddress => LayoutAddress;

        public abstract string LayoutAddress { get; }

        public VisualTreeAsset LoadedLayout => LoadedAsset;

        public UniTask<VisualTreeAsset> LoadLayoutIfNotLoaded(CancellationToken cancellationToken)
        {
            return LoadAssetIfNotLoaded(cancellationToken);
        }
    }
}
#endif
