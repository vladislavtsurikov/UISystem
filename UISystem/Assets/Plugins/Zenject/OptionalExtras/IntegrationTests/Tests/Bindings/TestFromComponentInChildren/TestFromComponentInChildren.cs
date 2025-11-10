using System.Collections;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;
using UnityEngine.TestTools;

namespace Zenject.Tests.Bindings
{
    public class TestFromComponentInChildren : ZenjectIntegrationTestFixture
    {
        private Child _child1;
        private Child _child2;
        private Grandchild _grandchild;
        private Root _root;

        public void Setup1()
        {
            _root = new GameObject("root").AddComponent<Root>();

            _child1 = new GameObject("child1").AddComponent<Child>();
            _child1.transform.SetParent(_root.transform);

            _child2 = new GameObject("child2").AddComponent<Child>();
            _child2.transform.SetParent(_root.transform);

            _grandchild = new GameObject("grandchild").AddComponent<Grandchild>();
            _grandchild.transform.SetParent(_child1.transform);
        }

        [UnityTest]
        public IEnumerator RunMatchSingleChild()
        {
            Setup1();
            PreInstall();
            Container.Bind<Grandchild>().FromComponentInChildren();
            Container.Bind<Child>().FromComponentInChildren();

            PostInstall();

            Assert.IsEqual(_root.Grandchild, _grandchild);
            Assert.IsEqual(_root.Childs.Count, 1);
            Assert.IsEqual(_root.Childs[0], _child1);
            yield break;
        }

        [UnityTest]
        public IEnumerator RunMatchAllChildren()
        {
            Setup1();
            PreInstall();
            Container.Bind<Grandchild>().FromComponentInChildren();
            Container.Bind<Child>().FromComponentsInChildren();

            PostInstall();

            Assert.IsEqual(_root.Grandchild, _grandchild);
            Assert.IsEqual(_root.Childs.Count, 2);
            Assert.IsEqual(_root.Childs[0], _child1);
            Assert.IsEqual(_root.Childs[1], _child2);
            yield break;
        }

        [UnityTest]
        public IEnumerator RunMissingChildrenFailure()
        {
            new GameObject("root").AddComponent<Root>();

            PreInstall();
            Container.Bind<Grandchild>().FromComponentInChildren();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator RunMissingChildrenSuccess()
        {
            Root root = new GameObject("root").AddComponent<Root>();

            Grandchild grandchild = new GameObject("grandchild").AddComponent<Grandchild>();
            grandchild.transform.SetParent(root.transform);

            PreInstall();
            Container.Bind<Grandchild>().FromComponentInChildren();

            PostInstall();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestOptional()
        {
            RootWithOptional root = new GameObject("root").AddComponent<RootWithOptional>();

            PreInstall();

            Container.Bind<Child>().FromComponentInChildren();

            PostInstall();

            Assert.IsNull(root.Child);

            yield break;
        }

        [UnityTest]
        public IEnumerator TestOptional2()
        {
            Root root = new GameObject("root").AddComponent<Root>();

            Grandchild grandChild = new GameObject("grandchild").AddComponent<Grandchild>();
            grandChild.transform.SetParent(root.transform, false);

            PreInstall();

            Container.Bind<Grandchild>().FromComponentsInChildren();
            Container.Bind<Child>().FromComponentInChildren();

            PostInstall();

            // The FromComponentInChildren call should match nothing when optional like in
            // list bindings
            Assert.That(root.Childs.IsEmpty());

            yield break;
        }

        [UnityTest]
        public IEnumerator RunMatchSingleChildNonGeneric()
        {
            Setup1();
            PreInstall();
            Container.Bind(typeof(Grandchild)).FromComponentInChildren();
            Container.Bind(typeof(Child)).FromComponentInChildren();

            PostInstall();

            Assert.IsEqual(_root.Grandchild, _grandchild);
            Assert.IsEqual(_root.Childs.Count, 1);
            Assert.IsEqual(_root.Childs[0], _child1);
            yield break;
        }

        [UnityTest]
        public IEnumerator RunMatchAllChildrenNonGeneric()
        {
            Setup1();
            PreInstall();
            Container.Bind(typeof(Grandchild)).FromComponentInChildren();
            Container.Bind<Child>().FromComponentsInChildren();

            PostInstall();

            Assert.IsEqual(_root.Grandchild, _grandchild);
            Assert.IsEqual(_root.Childs.Count, 2);
            Assert.IsEqual(_root.Childs[0], _child1);
            Assert.IsEqual(_root.Childs[1], _child2);
            yield break;
        }

        [UnityTest]
        public IEnumerator RunMissingChildrenFailureNonGeneric()
        {
            new GameObject("root").AddComponent<Root>();

            PreInstall();
            Container.Bind(typeof(Grandchild)).FromComponentInChildren();

            Assert.Throws(() => PostInstall());
            yield break;
        }

        [UnityTest]
        public IEnumerator RunMissingChildrenSuccessNonGeneric()
        {
            Root root = new GameObject("root").AddComponent<Root>();

            Grandchild grandchild = new GameObject("grandchild").AddComponent<Grandchild>();
            grandchild.transform.SetParent(root.transform);

            PreInstall();
            Container.Bind(typeof(Grandchild)).FromComponentInChildren();

            PostInstall();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestOptionalNonGeneric()
        {
            RootWithOptional root = new GameObject("root").AddComponent<RootWithOptional>();

            PreInstall();

            Container.Bind(typeof(Child)).FromComponentInChildren();

            PostInstall();

            Assert.IsNull(root.Child);

            yield break;
        }

        public class Root : MonoBehaviour
        {
            [Inject]
            public Grandchild Grandchild;

            [Inject]
            public List<Child> Childs;
        }

        public class Child : MonoBehaviour
        {
        }

        public class Grandchild : MonoBehaviour
        {
        }

        public class RootWithOptional : MonoBehaviour
        {
            [InjectOptional]
            public Child Child;
        }
    }
}
