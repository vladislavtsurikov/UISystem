using System;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFromPoolableMemoryPoolValidation
    {
        public class Bar
        {
        }

        public class Foo : IPoolable<IMemoryPool>, IDisposable
        {
            public Foo(Bar bar)
            {
            }

            public IMemoryPool Pool { get; private set; }

            public void Dispose() => Pool.Despawn(this);

            public void OnDespawned()
            {
                Pool = null;
                SetDefaults();
            }

            public void OnSpawned(IMemoryPool pool) => Pool = pool;

            private void SetDefaults() => Pool = null;

            public class Factory : PlaceholderFactory<Foo>
            {
            }
        }

        [Test]
        public void TestFailure()
        {
            var container = new DiContainer(true);
            container.Settings = new ZenjectSettings(
                ValidationErrorResponses.Throw, RootResolveMethods.All);

            container.BindFactory<Foo, Foo.Factory>().FromPoolableMemoryPool(x => x.WithInitialSize(2));

            Assert.Throws(() => container.ResolveRoots());
        }


        [Test]
        public void TestSuccess()
        {
            var container = new DiContainer(true);
            container.Settings = new ZenjectSettings(
                ValidationErrorResponses.Throw, RootResolveMethods.All);

            container.Bind<Bar>().AsSingle();
            container.BindFactory<Foo, Foo.Factory>().FromPoolableMemoryPool(x => x.WithInitialSize(2));

            container.ResolveRoots();
        }
    }
}
