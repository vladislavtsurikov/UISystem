using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.MonoBehaviour
{
    public class HierarchyTerrainObjectInstance : UnityEngine.MonoBehaviour
    {
        //What objects cause this GameObject to appear
        internal readonly List<object> UsedObjects = new();
        public TerrainObjectInstance TerrainObjectInstance { get; private set; }

        private void Update()
        {
            foreach (Script script in TerrainObjectInstance.GetScripts())
            {
                script.Update();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            foreach (Script script in TerrainObjectInstance.GetScripts())
            {
                script.OnCollisionEnter(collision);
            }
        }

        internal void Setup(TerrainObjectInstance instance, Scripting scriptingComponent)
        {
            TerrainObjectInstance = instance;
            instance.HierarchyTerrainObjectInstance = this;

            if (scriptingComponent.IsValid())
            {
                if (!Application.isPlaying)
                {
                    return;
                }

                foreach (Script script in scriptingComponent.ScriptStack.ElementList)
                {
                    instance.AddScript(script);
                }

                instance.SetupScripts();
            }
        }
    }
}
