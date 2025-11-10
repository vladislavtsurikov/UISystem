using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneManagerTool.Runtime.Callbacks.SceneOperation;
using VladislavTsurikov.SceneUtility.Runtime;
using GameObjectUtility = VladislavTsurikov.UnityUtility.Runtime.GameObjectUtility;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem
{
    [Name("Progress Bar")]
    [SceneCollectionComponent]
    public class ProgressBar : SettingsComponent
    {
        public bool DisableFade = false;
        public SceneReference SceneReference = new();

#if UNITY_EDITOR
        protected override void OnCreate()
        {
            SceneAsset sceneAsset = Resources.Load<SceneAsset>("Progress Bar");

            if (sceneAsset != null)
            {
                SceneReference = new SceneReference(sceneAsset);
            }
        }
#endif

        internal static async UniTask LoadProgressBarIfNecessary(
            ComponentStackOnlyDifferentTypes<SettingsComponent> settingsList)
        {
            var progressBar = (ProgressBar)settingsList.GetElement(typeof(ProgressBar));

            if (progressBar != null)
            {
                await progressBar.LoadFade();
            }
        }

        internal static async UniTask UnloadProgressBarIfNecessary(
            ComponentStackOnlyDifferentTypes<SettingsComponent> settingsList)
        {
            var progressBar = (ProgressBar)settingsList.GetElement(typeof(ProgressBar));

            if (progressBar != null)
            {
                await progressBar.UnloadFade();
            }
        }

        private async UniTask LoadFade()
        {
            await SceneReference.LoadScene();

            var sceneOperation =
                (SceneOperation)GameObjectUtility.FindObjectsOfType(typeof(SceneOperation), SceneReference.Scene)[0];

            await sceneOperation.OnLoad();
        }

        private async UniTask UnloadFade()
        {
            var sceneOperation =
                (SceneOperation)GameObjectUtility.FindObjectsOfType(typeof(SceneOperation), SceneReference.Scene)[0];

            await sceneOperation.OnUnload();

            await SceneReference.UnloadScene();
        }

        public override List<SceneReference> GetSceneReferences() => new() { SceneReference };
    }
}
