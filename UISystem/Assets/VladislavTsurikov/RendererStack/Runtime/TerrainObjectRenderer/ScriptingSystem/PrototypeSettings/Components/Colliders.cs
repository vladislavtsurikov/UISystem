using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings
{
    [Name("Colliders")]
    public class Colliders : PrototypeComponent
    {
        [SerializeField]
        private float _maxDistance = 50;

        public float MaxDistance
        {
            get => _maxDistance;
            set
            {
                if (value < 1)
                {
                    _maxDistance = 1;
                }
                else
                {
                    _maxDistance = value;
                }
            }
        }

        protected override void OnDeleteElement() => TerrainObjectRenderer.Instance.ScriptingSystem.Setup();

        protected override void OnCreate() => TerrainObjectRenderer.Instance.ScriptingSystem.Setup();

        protected override void OnChangeActive() => TerrainObjectRenderer.Instance.ScriptingSystem.Setup();
    }
}
