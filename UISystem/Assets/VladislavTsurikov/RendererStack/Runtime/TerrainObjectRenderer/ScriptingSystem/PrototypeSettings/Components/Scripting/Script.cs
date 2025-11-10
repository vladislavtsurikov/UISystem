using System;
using UnityEngine;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting
{
    [Serializable]
    public abstract class Script : Component
    {
        [NonSerialized]
        protected TerrainObjectInstance TerrainObjectInstance;

        protected override void SetupComponent(object[] setupData = null)
        {
            if (setupData != null && setupData.Length != 0)
            {
                TerrainObjectInstance = (TerrainObjectInstance)setupData[0];
            }

            OnEnable();
        }

        public virtual void OnEnable()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void OnCollisionEnter(Collision collision)
        {
        }
    }
}
