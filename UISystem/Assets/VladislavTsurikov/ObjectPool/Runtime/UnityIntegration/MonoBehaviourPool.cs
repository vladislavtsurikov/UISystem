using UnityEngine;

namespace VladislavTsurikov.ObjectPool.Runtime
{
    public class MonoBehaviourPool<T> : ObjectPool<T> where T : MonoBehaviour
    {
        private readonly Transform _container;
        private readonly T _prefab;

        public MonoBehaviourPool(T prefab, Transform container, int maxSize = 100, bool collectionCheck = false)
            : base(new StackPoolCollection<T>(), maxSize, collectionCheck)
        {
            _prefab = prefab;
            _container = container;
        }

        public MonoBehaviourPool(T prefab, Transform container, PoolCollection<T> poolCollection, int maxSize = 100,
            bool collectionCheck = false)
            : base(poolCollection, maxSize, collectionCheck)
        {
            _prefab = prefab;
            _container = container;
        }

        protected override T CreateInstance() => Object.Instantiate(_prefab, _container);

        protected override void OnGet(T obj) => obj.gameObject.SetActive(true);

        protected override void OnRelease(T obj)
        {
            obj.transform.SetAsFirstSibling();
            obj.gameObject.SetActive(false);
        }

        protected override void OnDestroy(T obj) => Object.Destroy(obj.gameObject);
    }
}
