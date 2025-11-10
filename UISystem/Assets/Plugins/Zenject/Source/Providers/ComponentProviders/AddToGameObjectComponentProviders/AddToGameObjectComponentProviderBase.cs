#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject.Internal;

namespace Zenject
{
    [NoReflectionBaking]
    public abstract class AddToGameObjectComponentProviderBase : IProvider
    {
        private readonly object _concreteIdentifier;
        private readonly List<TypeValuePair> _extraArguments;
        private readonly Action<InjectContext, object> _instantiateCallback;

        public AddToGameObjectComponentProviderBase(
            DiContainer container, Type componentType,
            IEnumerable<TypeValuePair> extraArguments, object concreteIdentifier,
            Action<InjectContext, object> instantiateCallback)
        {
            Assert.That(componentType.DerivesFrom<Component>());

            _extraArguments = extraArguments.ToList();
            ComponentType = componentType;
            Container = container;
            _concreteIdentifier = concreteIdentifier;
            _instantiateCallback = instantiateCallback;
        }

        protected DiContainer Container { get; }

        protected Type ComponentType { get; }

        protected abstract bool ShouldToggleActive { get; }

        public bool IsCached => false;

        public bool TypeVariesBasedOnMemberType => false;

        public Type GetInstanceType(InjectContext context) => ComponentType;

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsNotNull(context);

            object instance;

            // We still want to make sure we can get the game object during validation
            GameObject gameObj = GetGameObject(context);

            var wasActive = gameObj.activeSelf;

            if (wasActive && ShouldToggleActive)
            {
                // We need to do this in some cases to ensure that [Inject] always gets
                // called before awake / start
                gameObj.SetActive(false);
            }

            if (!Container.IsValidating || TypeAnalyzer.ShouldAllowDuringValidation(ComponentType))
            {
                if (ComponentType == typeof(Transform))
                    // Treat transform as a special case because it's the one component that's always automatically added
                    // Otherwise, calling AddComponent below will fail and return null
                    // This is nice to allow doing things like
                    //      Container.Bind<Transform>().FromNewComponentOnNewGameObject();
                {
                    instance = gameObj.transform;
                }
                else
                {
                    instance = gameObj.AddComponent(ComponentType);
                }

                Assert.IsNotNull(instance);
            }
            else
            {
                instance = new ValidationMarker(ComponentType);
            }

            injectAction = () =>
            {
                try
                {
                    List<TypeValuePair> extraArgs = ZenPools.SpawnList<TypeValuePair>();

                    extraArgs.AllocFreeAddRange(_extraArguments);
                    extraArgs.AllocFreeAddRange(args);

                    Container.InjectExplicit(instance, ComponentType, extraArgs, context, _concreteIdentifier);

                    Assert.That(extraArgs.Count == 0);

                    ZenPools.DespawnList(extraArgs);

                    if (_instantiateCallback != null)
                    {
                        _instantiateCallback(context, instance);
                    }
                }
                finally
                {
                    if (wasActive && ShouldToggleActive)
                    {
                        gameObj.SetActive(true);
                    }
                }
            };

            buffer.Add(instance);
        }

        protected abstract GameObject GetGameObject(InjectContext context);
    }
}

#endif
