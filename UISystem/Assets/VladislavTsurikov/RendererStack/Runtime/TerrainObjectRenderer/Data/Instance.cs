using UnityEngine;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data
{
    public class Instance
    {
        private Vector3 _position;
        private Quaternion _rotation;
        private Vector3 _scale;

        protected Instance(Vector3 position, Vector3 scale, Quaternion rotation)
        {
            _position = position;
            _scale = scale;
            _rotation = rotation;
            ID = GetHashCode();
        }

        protected Instance(int id, Vector3 position, Vector3 scale, Quaternion rotation)
        {
            _position = position;
            _scale = scale;
            _rotation = rotation;
            ID = id;
        }

        public int ID { get; }

        public bool IsDestroy { get; private set; }

        public Vector3 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    Reposition();
                }
            }
        }

        public Vector3 Scale
        {
            get
            {
                var newScale = new Vector3(_scale.x * GetMultiplySize().x, _scale.y * GetMultiplySize().y,
                    _scale.z * GetMultiplySize().z);

                return newScale;
            }
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    TransformChanged();
                }
            }
        }

        public Quaternion Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    TransformChanged();
                }
            }
        }

        public Vector3 Right
        {
            get => Rotation * Vector3.right;
            set => Rotation = Quaternion.FromToRotation(Vector3.right, value);
        }

        public Vector3 Up
        {
            get => Rotation * Vector3.up;
            set => Rotation = Quaternion.FromToRotation(Vector3.up, value);
        }

        public Vector3 Forward
        {
            get => Rotation * Vector3.forward;
            set => Rotation = Quaternion.LookRotation(value);
        }

        public Matrix4x4 GetMatrix() => Matrix4x4.TRS(Position, Rotation, Scale);

        public void Destroy()
        {
            IsDestroy = true;
            DestroyInstance();
        }

        protected virtual void DestroyInstance()
        {
        }

        protected virtual void TransformChanged()
        {
        }

        protected virtual void Reposition()
        {
        }

        protected virtual Vector3 GetMultiplySize() => Vector3.one;
    }
}
