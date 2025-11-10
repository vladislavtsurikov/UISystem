using System;
using NUnit.Framework;
using Assert = ModestTree.Assert;

namespace Zenject.Tests.Signals
{
    [TestFixture]
    public class TestSignalIdentifiers : ZenjectUnitTestFixture
    {
        [SetUp]
        public void CommonInstall() => SignalBusInstaller.Install(Container);

        [Test]
        public void TestMissingDeclaration()
        {
            SignalBus signalBus = Container.Resolve<SignalBus>();

            Assert.Throws(() => signalBus.FireId<FooSignal>("asdf"));
        }

        [Test]
        public void TestSubscribeAndUnsubscribe()
        {
            var signalId = "asdf";

            Container.DeclareSignal<FooSignal>().WithId(signalId);

            SignalBus signalBus = Container.Resolve<SignalBus>();

            var received = false;

            Action callback = () => received = true;
            signalBus.SubscribeId<FooSignal>(signalId, callback);

            Assert.Throws(() => signalBus.Subscribe<FooSignal>(callback));

            Assert.That(!received);
            signalBus.FireId<FooSignal>(signalId);
            Assert.That(received);

            Assert.Throws(() => signalBus.Fire<FooSignal>());
            Assert.Throws(() => signalBus.FireId<FooSignal>("asdfz"));

            received = false;
            signalBus.FireId<FooSignal>(signalId);
            Assert.That(received);

            signalBus.UnsubscribeId<FooSignal>(signalId, callback);

            received = false;
            signalBus.FireId<FooSignal>(signalId);
            Assert.That(!received);
        }

        [Test]
        public void TestIncompleteBinding()
        {
            Container.DeclareSignal<FooSignal>().WithId("asdf");
            Container.BindSignal<FooSignal>().WithId("asdf");

            Assert.Throws(() => Container.FlushBindings());
        }

        [Test]
        public void TestBindWithoutDeclaration()
        {
            Container.BindSignal<FooSignal>().WithId("asdf").ToMethod(() => { });

            Assert.Throws(() => Container.ResolveRoots());
        }

        [Test]
        public void TestStaticMethodHandler()
        {
            Container.DeclareSignal<FooSignal>().WithId("asdf");

            var received = false;

            Container.BindSignal<FooSignal>().WithId("asdf").ToMethod(() => received = true);
            Container.ResolveRoots();

            SignalBus signalBus = Container.Resolve<SignalBus>();

            Assert.That(!received);
            signalBus.FireId<FooSignal>("asdf");
            Assert.That(received);
        }

        [Test]
        public void TestStaticMethodHandlerWithArgs()
        {
            Container.DeclareSignal<FooSignal>().WithId("asdf");

            FooSignal received = null;

            Container.BindSignal<FooSignal>().WithId("asdf").ToMethod(x => received = x);
            Container.ResolveRoots();

            SignalBus signalBus = Container.Resolve<SignalBus>();
            var sent = new FooSignal();

            Assert.IsNull(received);
            signalBus.FireId("asdf", sent);
            Assert.IsEqual(received, sent);
        }

        [Test]
        public void TestInstanceMethodHandler()
        {
            Container.DeclareSignal<FooSignal>().WithId("asdf");

            var qux = new Qux();
            Container.BindSignal<FooSignal>().WithId("asdf")
                .ToMethod<Qux>(x => x.OnFoo).From(b => b.FromInstance(qux));
            Container.ResolveRoots();

            SignalBus signalBus = Container.Resolve<SignalBus>();

            Assert.That(!qux.HasRecievedSignal);
            signalBus.FireId<FooSignal>("asdf");
            Assert.That(qux.HasRecievedSignal);
        }

        public class Qux
        {
            public bool HasRecievedSignal { get; private set; }

            public void OnFoo() => HasRecievedSignal = true;
        }

        public class FooSignal
        {
        }

        public class Foo
        {
        }
    }
}
