using UnityEngine;

namespace Zenject.SpaceFighter
{
    public class Player
    {
        private readonly Rigidbody _rigidBody;

        public Player(
            Rigidbody rigidBody,
            MeshRenderer renderer)
        {
            _rigidBody = rigidBody;
            Renderer = renderer;
        }

        public MeshRenderer Renderer { get; }

        public bool IsDead { get; set; }

        public float Health { get; private set; } = 100.0f;

        public Vector3 LookDir => -_rigidBody.transform.right;

        public Quaternion Rotation
        {
            get => _rigidBody.rotation;
            set => _rigidBody.rotation = value;
        }

        public Vector3 Position
        {
            get => _rigidBody.position;
            set => _rigidBody.position = value;
        }

        public Vector3 Velocity => _rigidBody.velocity;

        public void TakeDamage(float healthLoss) => Health = Mathf.Max(0.0f, Health - healthLoss);

        public void AddForce(Vector3 force) => _rigidBody.AddForce(force);
    }
}
