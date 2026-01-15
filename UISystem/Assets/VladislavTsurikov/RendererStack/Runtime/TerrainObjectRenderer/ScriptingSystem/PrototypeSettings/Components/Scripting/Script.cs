using System;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting
{
    [Serializable]
    public abstract class Script : Node
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
