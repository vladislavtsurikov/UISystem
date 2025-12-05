using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.PhysicsSimulator.Runtime
{
    public class SimulatedBody
    {
        protected readonly GameObject _gameObject;

        private readonly List<OnDisableSimulatedBodyEvent> _onDisablePhysicsEvents = new();
        private Collider _addedCollider;
        private List<Collider> _nonConvexColliders;

        protected internal Action OnAddToSimulatedBodyStack;

        public SimulatedBody(GameObject gameObject)
        {
            _gameObject = gameObject;

            var applyPositionDown =
                new ApplyPositionDown(PhysicsSimulatorSettings.Instance.AutoPositionDownSettings)
                {
                    SimulatedBody = this
                };

            _onDisablePhysicsEvents.Insert(0, applyPositionDown);

            AddPhysicsSupport();
        }

        public SimulatedBody(GameObject gameObject, List<OnDisableSimulatedBodyEvent> onDisablePhysicsEvents)
        {
            _gameObject = gameObject;

            if (onDisablePhysicsEvents == null)
            {
                onDisablePhysicsEvents = new List<OnDisableSimulatedBodyEvent>();
            }

            foreach (OnDisableSimulatedBodyEvent onDisablePhysicsAction in onDisablePhysicsEvents)
            {
                onDisablePhysicsAction.SimulatedBody = this;
            }

            _onDisablePhysicsEvents = onDisablePhysicsEvents;

            var applyPositionDown =
                new ApplyPositionDown(PhysicsSimulatorSettings.Instance.AutoPositionDownSettings)
                {
                    SimulatedBody = this
                };

            _onDisablePhysicsEvents.Insert(0, applyPositionDown);

            AddPhysicsSupport();
        }

        public Rigidbody Rigidbody { get; private set; }

        public GameObject GameObject => _gameObject;

        public bool IsRigidbodyStopping()
        {
            if (_gameObject == null)
            {
                return true;
            }

            if (Rigidbody == null || Rigidbody.IsSleeping())
            {
                return true;
            }

            return false;
        }

        private void AddPhysicsSupport()
        {
            _nonConvexColliders = new List<Collider>();

            Collider[] colliders = _gameObject.transform.GetComponentsInChildren<Collider>();

            foreach (Collider collider in colliders)
            {
                collider.hideFlags = HideFlags.HideInHierarchy;

                if (collider is MeshCollider { convex: false } meshCollider)
                {
                    _nonConvexColliders.Add(meshCollider);

                    meshCollider.convex = true;
                }
            }

            AddСomponentsIfNecessary();
        }

        public void DisablePhysicsSupport()
        {
            if (_gameObject == null || _nonConvexColliders == null)
            {
                return;
            }

            RemoveAddedComponents();

            Collider[] colliders = _gameObject.transform.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                if (collider is MeshCollider && _nonConvexColliders.Contains(collider))
                {
                    ((MeshCollider)collider).convex = false;
                }
            }

            _nonConvexColliders = null;

            try
            {
                foreach (OnDisableSimulatedBodyEvent onDisablePhysicsAction in _onDisablePhysicsEvents)
                {
                    onDisablePhysicsAction.OnDisablePhysics();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public bool HasRigidbody()
        {
            if (_gameObject == null || _nonConvexColliders == null)
            {
                return false;
            }

            if (_gameObject.GetComponent<Rigidbody>())
            {
                return true;
            }

            return false;
        }

        private void AddСomponentsIfNecessary()
        {
            if (!_gameObject.GetComponent<Rigidbody>())
            {
                Rigidbody rigidbody = _gameObject.gameObject.AddComponent<Rigidbody>();

                rigidbody.useGravity = true;
                rigidbody.mass = 1;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                Rigidbody = rigidbody;
            }

            if (!_gameObject.GetComponent<Collider>())
            {
                MeshCollider collider = _gameObject.gameObject.AddComponent<MeshCollider>();

                // hide colliders in the hierarchy, they cost performance
                collider.hideFlags = HideFlags.HideInHierarchy;

                collider.convex = true;

                _addedCollider = collider;
            }
        }

        private void RemoveAddedComponents()
        {
            if (Rigidbody != null)
            {
                Object.DestroyImmediate(Rigidbody);
            }

            if (_addedCollider != null)
            {
                Object.DestroyImmediate(_addedCollider);
            }
        }
    }
}
