using System;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;

namespace VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem
{
    [Serializable]
    public class RendererStack : NodeStackOnlyDifferentTypes<Renderer>
    {
        public void CheckChanges()
        {
            if (!Application.isPlaying)
            {
                foreach (Renderer setting in _elementList)
                {
                    setting?.CheckChanges();
                }
            }
        }

        public void ForceUpdateRendererData()
        {
            foreach (Renderer customRenderer in _elementList)
            {
                customRenderer.ForceUpdateRendererData = true;
            }
        }

        public void Render()
        {
            if (!RendererStackManager.Instance.IsSetup)
            {
                return;
            }

            foreach (Renderer setting in _elementList)
            {
                if (!setting.IsSetup)
                {
                    continue;
                }

                if (setting.Active)
                {
                    setting.Render();
                }
            }
        }
    }
}
