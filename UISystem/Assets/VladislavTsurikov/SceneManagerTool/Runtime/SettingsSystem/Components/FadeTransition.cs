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
    [Name("Fade Transition")]
    [SceneCollectionComponent]
    public class FadeTransition : SettingsComponent
    {
        public SceneReference SceneReference = new();

        internal static async UniTask LoadFadeIfNecessary(
            ComponentStackOnlyDifferentTypes<SettingsComponent> settingsList)
        {
            var fadeTransition = (FadeTransition)settingsList.GetElement(typeof(FadeTransition));

            if (fadeTransition != null)
            {
                await fadeTransition.LoadFadeIfNecessary();
            }
        }

        internal static async UniTask UnloadFadeIfNecessary(
            ComponentStackOnlyDifferentTypes<SettingsComponent> settingsList)
        {
            var fadeTransition = (FadeTransition)settingsList.GetElement(typeof(FadeTransition));

            if (fadeTransition != null)
            {
                await fadeTransition.UnloadFadeIfNecessary();
            }
        }

#if UNITY_EDITOR
        protected override void OnCreate()
        {
            SceneAsset sceneAsset = Resources.Load<SceneAsset>("Fade");

            if (sceneAsset != null)
            {
                SceneReference = new SceneReference(sceneAsset);
            }
        }
#endif

        private async UniTask LoadFadeIfNecessary()
        {
            await SceneReference.LoadScene();

            var sceneOperation =
                (SceneOperation)GameObjectUtility.FindObjectsOfType(typeof(SceneOperation), SceneReference.Scene)[0];

            await sceneOperation.OnLoad();
        }

        private async UniTask UnloadFadeIfNecessary()
        {
            var sceneOperation =
                (SceneOperation)GameObjectUtility.FindObjectsOfType(typeof(SceneOperation), SceneReference.Scene)[0];

            await sceneOperation.OnUnload();

            await SceneReference.UnloadScene();
        }

        public override List<SceneReference> GetSceneReferences() => new() { SceneReference };
    }
}
