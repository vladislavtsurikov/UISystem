using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem
{
    public abstract class Renderer : Component
    {
        public bool ForceUpdateRendererData = false;

        protected override void SetupComponent(object[] setupData = null)
        {
            SetupSceneData();
            SetupRenderer();
        }

        private void SetupSceneData()
        {
            AddSceneDataAttribute[] addSceneDataAttributes = GetType().GetAttributes<AddSceneDataAttribute>().ToArray();

            List<SceneDataManager> sceneDataManagers = SceneDataManagerUtility.GetAllSceneDataManager();

            foreach (AddSceneDataAttribute addSceneDataAttribute in addSceneDataAttributes)
            foreach (Type type in addSceneDataAttribute.Types)
            {
                RequiredSceneData.AddRequiredType(type);

                foreach (SceneDataManager sceneDataManager in sceneDataManagers)
                {
                    sceneDataManager.SceneDataStack.SetupElement(type, true);
                }
            }
        }

        protected override void OnCreate() => GlobalSettings.GlobalSettings.Instance.Setup();

        protected virtual void SetupRenderer()
        {
        }

        public virtual void Render()
        {
        }

        public virtual void DrawDebug()
        {
        }

        public virtual void CheckChanges()
        {
        }
    }
}
