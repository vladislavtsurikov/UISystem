using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Bindings
{
    [TestFixture]
    public class TestFactoryFromSubContainerMethod0 : ZenjectUnitTestFixture
    {
        private static readonly Foo ConstFoo = new();

        [Test]
        public void TestSelf()
        {
            Container.BindFactory<Foo, Foo.Factory>().FromSubContainerResolve().ByMethod(InstallFoo).NonLazy();

            Assert.IsEqual(Container.Resolve<Foo.Factory>().Create(), ConstFoo);
        }

        [Test]
        public void TestConcrete()
        {
            Container.BindFactory<IFoo, IFooFactory>()
                .To<Foo>().FromSubContainerResolve().ByMethod(InstallFoo).NonLazy();

            Assert.IsEqual(Container.Resolve<IFooFactory>().Create(), ConstFoo);
        }

        private void InstallFoo(DiContainer subContainer) => subContainer.Bind<Foo>().FromInstance(ConstFoo);

        private interface IFoo
        {
        }

        private class IFooFactory : PlaceholderFactory<IFoo>
        {
        }

        private class Foo : IFoo
        {
            public class Factory : PlaceholderFactory<Foo>
            {
            }
        }
    }
}
