#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UIRootSystem.Runtime.PrefabResourceLoaders;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UnityUIIntegration;
using Zenject;

namespace VladislavTsurikov.UIRootSystem.Runtime
{
    [SceneFilter("TestScene_1", "TestScene_2")]
    public class UIRoot : UnityUIHandler
    {
        public UIRoot(DiContainer container, UIRootLoader loader)
            : base(container, loader) =>
            OnAnyChildAdded += HandleChildAdded;

        protected override async UniTask InitializeUIHandler(CancellationToken cancellationToken,
            CompositeDisposable disposables) => await Show(cancellationToken);

        private void HandleChildAdded(GameObject childObject, UnityUIHandler childHandler) =>
            InsertByLayerIndex(childObject, childHandler);

        private void InsertByLayerIndex(GameObject childObject, UnityUIHandler childHandler)
        {
            if (childHandler is not UILayer layeredChild)
            {
                return;
            }

            if (!IsUnderThisTransform(childObject))
            {
                return;
            }

            var newLayer = layeredChild.GetLayerIndex();
            var insertIndex = SpawnedParentPrefab.transform.childCount;

            for (var i = 0; i < Children.Count; i++)
            {
                if (!TryGetValidSibling(i, layeredChild, out UILayer siblingUnit))
                {
                    continue;
                }

                var siblingLayer = siblingUnit.GetLayerIndex();
                if (newLayer < siblingLayer)
                {
                    insertIndex = i;
                    break;
                }
            }

            childObject.transform.SetSiblingIndex(insertIndex);
        }

        private bool TryGetValidSibling(int index, UILayer self, out UILayer sibling)
        {
            sibling = null;

            if (!TryGetLayeredSibling(index, out UILayer uiUnit))
            {
                return false;
            }

            if (ReferenceEquals(uiUnit, self))
            {
                return false;
            }

            if (!IsUnderThisTransform(uiUnit.SpawnedParentPrefab))
            {
                return false;
            }

            sibling = uiUnit;
            return true;
        }

        private bool TryGetLayeredSibling(int index, out UILayer sibling)
        {
            sibling = null;

            if (index < 0 || index >= Children.Count)
            {
                Debug.LogError($"[UIRoot] Invalid index {index} in Children list (Count: {Children.Count})");
                return false;
            }

            UIHandler child = Children[index];

            if (child is not UILayer casted)
            {
                Debug.LogError(
                    $"[UIRoot] Child at index {index} is not a LayeredUnityUIUnit (actual type: {child.GetType().Name})");
                return false;
            }

            sibling = casted;
            return true;
        }

        private bool IsUnderThisTransform(GameObject obj)
        {
            if (obj == null || obj.transform.parent == SpawnedParentPrefab.transform)
            {
                return true;
            }

            Debug.LogWarning(
                $"[UIRoot] Spawned child '{obj.name}' is not under expected parent '{typeof(UIRoot)}'. Check parenting logic.");
            return false;
        }
    }
}

#endif

#endif

#endif
