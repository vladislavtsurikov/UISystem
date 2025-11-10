using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class SubContainerDependencyProvider : IProvider
    {
        private readonly Type _dependencyType;
        private readonly object _identifier;
        private readonly bool _resolveAll;
        private readonly ISubContainerCreator _subContainerCreator;

        // if concreteType is null we use the contract type from inject context
        public SubContainerDependencyProvider(
            Type dependencyType,
            object identifier,
            ISubContainerCreator subContainerCreator, bool resolveAll)
        {
            _subContainerCreator = subContainerCreator;
            _dependencyType = dependencyType;
            _identifier = identifier;
            _resolveAll = resolveAll;
        }

        public bool IsCached => false;

        public bool TypeVariesBasedOnMemberType => false;

        public Type GetInstanceType(InjectContext context) => _dependencyType;

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsNotNull(context);

            DiContainer subContainer = _subContainerCreator.CreateSubContainer(args, context, out injectAction);

            InjectContext subContext = CreateSubContext(context, subContainer);

            if (_resolveAll)
            {
                subContainer.ResolveAll(subContext, buffer);
                return;
            }

            buffer.Add(subContainer.Resolve(subContext));
        }

        private InjectContext CreateSubContext(
            InjectContext parent, DiContainer subContainer)
        {
            InjectContext subContext = parent.CreateSubContext(_dependencyType, _identifier);

            subContext.Container = subContainer;

            // This is important to avoid infinite loops
            subContext.SourceType = InjectSources.Local;

            return subContext;
        }
    }
}
