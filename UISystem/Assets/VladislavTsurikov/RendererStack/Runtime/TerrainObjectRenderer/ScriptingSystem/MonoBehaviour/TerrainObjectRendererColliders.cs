using UnityEngine;
using VladislavTsurikov.Math.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.MonoBehaviour
{
    [ExecuteInEditMode]
    public class TerrainObjectRendererColliders : UnityEngine.MonoBehaviour
    {
        public float MaxDistance = 100;

        private void Update()
        {
            var sphere = new Sphere(gameObject.transform.position, MaxDistance);

            ScriptingSystem.SetColliders(sphere, this);
        }

        private void OnDisable() => ScriptingSystem.RemoveColliders(this);
    }
}
