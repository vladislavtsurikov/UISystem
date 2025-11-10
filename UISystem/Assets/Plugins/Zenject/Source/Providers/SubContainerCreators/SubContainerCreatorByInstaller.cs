using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Zenject.Internal;

namespace Zenject
{
    [NoReflectionBaking]
    public class SubContainerCreatorByInstaller : ISubContainerCreator
    {
        private readonly DiContainer _container;
        private readonly SubContainerCreatorBindInfo _containerBindInfo;
        private readonly List<TypeValuePair> _extraArgs;
        private readonly Type _installerType;

        public SubContainerCreatorByInstaller(
            DiContainer container,
            SubContainerCreatorBindInfo containerBindInfo,
            Type installerType,
            IEnumerable<TypeValuePair> extraArgs)
        {
            _installerType = installerType;
            _container = container;
            _extraArgs = extraArgs.ToList();
            _containerBindInfo = containerBindInfo;

            Assert.That(installerType.DerivesFrom<InstallerBase>(),
                "Invalid installer type given during bind command.  Expected type '{0}' to derive from 'Installer<>'",
                installerType);
        }

        public SubContainerCreatorByInstaller(
            DiContainer container,
            SubContainerCreatorBindInfo containerBindInfo,
            Type installerType)
            : this(container, containerBindInfo, installerType, new List<TypeValuePair>())
        {
        }

        public DiContainer CreateSubContainer(List<TypeValuePair> args, InjectContext context, out Action injectAction)
        {
            DiContainer subContainer = _container.CreateSubContainer();

            SubContainerCreatorUtil.ApplyBindSettings(_containerBindInfo, subContainer);

            List<TypeValuePair> extraArgs = ZenPools.SpawnList<TypeValuePair>();

            extraArgs.AllocFreeAddRange(_extraArgs);
            extraArgs.AllocFreeAddRange(args);

            var installer = (InstallerBase)subContainer.InstantiateExplicit(
                _installerType, extraArgs);

            ZenPools.DespawnList(extraArgs);

            installer.InstallBindings();

            injectAction = () => { subContainer.ResolveRoots(); };

            return subContainer;
        }
    }
}
