using System.Collections.Generic;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.BindFeatures
{
    [TestFixture]
    public class TestMultipleContractTypes : ZenjectUnitTestFixture
    {
        private class Test1
        {
        }

        private class Test2 : Test1
        {
        }

        private class Test3 : Test1
        {
        }

        private class TestImpl1
        {
            public readonly List<Test1> tests;

            public TestImpl1(List<Test1> tests) => this.tests = tests;
        }

        private class TestImpl2
        {
            [Inject]
            public List<Test1> tests;
        }

        [Test]
        public void TestMultiBind1()
        {
            Container.Bind<Test1>().To<Test2>().AsSingle().NonLazy();
            Container.Bind<Test1>().To<Test3>().AsSingle().NonLazy();
            Container.Bind<TestImpl1>().AsSingle().NonLazy();

            TestImpl1 test1 = Container.Resolve<TestImpl1>();

            Assert.That(test1.tests.Count == 2);
        }

        [Test]
        public void TestMultiBindListInjection()
        {
            Container.Bind<Test1>().To<Test2>().AsSingle().NonLazy();
            Container.Bind<Test1>().To<Test3>().AsSingle().NonLazy();
            Container.Bind<TestImpl2>().AsSingle().NonLazy();

            TestImpl2 test = Container.Resolve<TestImpl2>();
            Assert.That(test.tests.Count == 2);
        }
    }
}
