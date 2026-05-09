#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.Core.Runtime.DependencyInjection;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public abstract class UIToolkitUIPresenter : UIPresenter
    {
        public UIToolkitLayoutLoader Loader { get; }
        public VisualElement SpawnedRoot { get; private set; }

        protected virtual string ParentContainerName => null;

        protected virtual string SpawnedRootName => null;

        protected override bool UsesParentBindingContext => Loader == null;

        protected UIToolkitElementBinder ElementBinder { get; }

        protected UIToolkitUIPresenter(UIToolkitLayoutLoader loader)
        {
            Loader = loader;
            ElementBinder = new UIToolkitElementBinder(this);
        }

        protected UIToolkitUIPresenter()
            : this(null)
        {
        }

        protected virtual void DisposeUIToolkitUIPresenter()
        {
        }

        protected override UniTask EnsurePresenterRoot(CancellationToken cancellationToken)
        {
            return SpawnLayoutIfNeeded(cancellationToken);
        }

        protected override void ShowPresenterRoot()
        {
            if (SpawnedRoot != null)
            {
                SpawnedRoot.style.display = StyleKeyword.Null;
            }
        }

        protected override void HidePresenterRoot()
        {
            if (SpawnedRoot != null)
            {
                SpawnedRoot.style.display = DisplayStyle.None;
            }
        }

        protected override async UniTask DestroyPresenterRoot(bool unload, CancellationToken cancellationToken)
        {
            if (Loader != null && SpawnedRoot != null)
            {
                SpawnedRoot.RemoveFromHierarchy();
            }

            if (Loader != null)
            {
                if (unload)
                {
                    await Loader.Unload(cancellationToken);
                }
            }

            SpawnedRoot = null;
        }

        protected override async UniTask DestroyUIPresenter(
            bool unload,
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            await DestroyUIToolkitUIPresenter(unload, cancellationToken);
        }

        protected virtual UniTask DestroyUIToolkitUIPresenter(bool unload, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public override void DisposeUIPresenter()
        {
            ElementBinder.Dispose();
            SpawnedRoot = null;
            DisposeUIToolkitUIPresenter();
        }

        private async UniTask SpawnLayoutIfNeeded(CancellationToken cancellationToken)
        {
            if (SpawnedRoot != null)
            {
                return;
            }

            if (Loader == null)
            {
                SpawnedRoot = ResolveParentElement();

                return;
            }

            VisualElement parent = ResolveParentElement();
            if (parent == null)
            {
                Debug.LogError(
                    $"[UIToolkitUIPresenter] Cannot resolve parent root for presenter `{GetType().Name}`.");
                return;
            }

            VisualElement spawnedRoot = await EnsureSpawnedRoot(parent, cancellationToken);

            if (spawnedRoot == null)
            {
                Debug.LogError(
                    $"[UIToolkitUIPresenter] Failed to spawn root layout for presenter `{GetType().Name}`.");
            }
        }

        private VisualElement ResolveParentElement()
        {
            if (Parent == null)
            {
                return ResolveTopLevelRoot();
            }

            if (Parent is not UIToolkitUIPresenter parentPresenter)
            {
                throw new InvalidOperationException(
                    $"Invalid parent type: {Parent.GetType().Name}. Expected {nameof(UIToolkitUIPresenter)}.");
            }

            string parentContainerName = ResolveParentContainerName();
            if (string.IsNullOrEmpty(parentContainerName))
            {
                return parentPresenter.SpawnedRoot;
            }

            return parentPresenter.ViewResolver.GetView<VisualElement>(parentContainerName);
        }

        internal string ResolveParentContainerName()
        {
            string parentContainerName = ParentContainerName;
            if (!string.IsNullOrEmpty(parentContainerName))
            {
                return parentContainerName;
            }

            UIParentAttribute attribute = (UIParentAttribute)Attribute.GetCustomAttribute(
                GetType(),
                typeof(UIParentAttribute));

            return attribute?.ContainerId;
        }

        internal VisualElement ResolveTopLevelRoot()
        {
            if (Dependencies.TryResolve(typeof(UIDocument), out object instance) &&
                instance is UIDocument document)
            {
                return document.rootVisualElement;
            }

            return null;
        }

        private async UniTask<VisualElement> EnsureSpawnedRoot(
            VisualElement parent,
            CancellationToken cancellationToken)
        {
            if (SpawnedRoot != null)
            {
                return SpawnedRoot;
            }

            SpawnedRoot = await UIToolkitSpawnOperation.Spawn()
                .WithParent(parent)
                .Visible(true)
                .WithName(SpawnedRootName)
                .Execute(Loader, ElementBinder, cancellationToken);

            if (SpawnedRoot != null)
            {
                StretchToParent(SpawnedRoot);
            }

            return SpawnedRoot;
        }

        private static void StretchToParent(VisualElement element)
        {
            element.style.position = Position.Absolute;
            element.style.left = 0;
            element.style.top = 0;
            element.style.right = 0;
            element.style.bottom = 0;
            element.style.width = StyleKeyword.Auto;
            element.style.height = StyleKeyword.Auto;
            element.style.flexGrow = 1;
            element.style.flexShrink = 0;
        }
    }
}
#endif
