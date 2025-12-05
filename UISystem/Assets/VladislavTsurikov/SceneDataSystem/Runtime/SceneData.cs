using System;
using VladislavTsurikov.AttributeUtility.Runtime;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEngine;
#endif

namespace VladislavTsurikov.SceneDataSystem.Runtime
{
    [Serializable]
    public abstract class SceneData : Component
    {
        public delegate void OnSetup();

        public static OnSetup OnSetupEvent;
        public SceneDataManager SceneDataManager;

        protected override void SetupComponent(object[] setupData = null)
        {
            if (setupData != null)
            {
                SceneDataManager = (SceneDataManager)setupData[0];
            }

            SetupSceneData();
            OnSetupEvent?.Invoke();
        }

        protected override void OnCreate()
        {
            if (SceneDataManager == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(SceneDataManager.Scene);
            }
#endif
        }

        protected override void OnDeleteElement()
        {
            if (SceneDataManager == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(SceneDataManager.Scene);
            }
#endif
        }

        public override bool DeleteElement()
        {
            AllowCreateComponentAttribute allowCreateComponentAttribute =
                GetType().GetAttribute<AllowCreateComponentAttribute>();

            if (allowCreateComponentAttribute == null)
            {
                return true;
            }

            return allowCreateComponentAttribute.Allow(SceneDataManager);
        }

        protected virtual void SetupSceneData()
        {
        }

        public virtual void DrawDebug()
        {
        }

        public virtual void LateUpdate()
        {
        }
    }
}
