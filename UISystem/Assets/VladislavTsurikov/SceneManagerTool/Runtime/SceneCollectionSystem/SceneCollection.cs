using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using VladislavTsurikov.SceneUtility.Runtime;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using ProgressBar = VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.ProgressBar;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem
{
    public class SceneCollection : Component
    {
        [OdinSerialize]
        private string _name;

        [OdinSerialize]
        public SceneTypeComponentStack SceneTypeComponentStack = new();

        [OdinSerialize]
        public ComponentStackOnlyDifferentTypes<SettingsComponent> SettingsStack = new();

        public bool Startup = true;

        [field: OdinSerialize]
        public int ID { get; private set; }

        public override string Name
        {
            get => _name;
            set => _name = value;
        }

        public static SceneCollection Current { get; private set; }

        public float LoadingProgress
        {
            get
            {
                var sceneComponents = new List<SceneType>();

                foreach (SceneType sceneComponent in SceneTypeComponentStack.ElementList)
                {
                    var sceneBehavior = (SceneBehavior)sceneComponent.SettingsStack.GetElement(typeof(SceneBehavior));

                    if (sceneBehavior != null)
                    {
                        if (sceneBehavior.SceneOpenBehavior == SceneOpenBehavior.DoNotOpen)
                        {
                            continue;
                        }
                    }

                    sceneComponents.Add(sceneComponent);
                }

                return !sceneComponents.Any()
                    ? 1
                    : sceneComponents.Sum(a => a.LoadingProgress()) / sceneComponents.Count;
            }
        }

        protected override void SetupComponent(object[] setupData = null)
        {
            SceneTypeComponentStack.Setup(true, new object[] { this });
            SettingsStack.Setup();
        }

        protected override void OnCreate()
        {
            ID = GetHashCode();
            _name = "Scene Collection";
        }

        public async UniTask Load()
        {
            SceneCollection pastSceneCollection = Current;

            if (Current != null)
            {
                await FadeTransition.LoadFadeIfNecessary(Current.SettingsStack);
                await Current.Unload(this);
            }

            if (this == Current)
            {
                return;
            }

            Current = this;

            await ProgressBar.LoadProgressBarIfNecessary(SettingsStack);

            var beforeLoadOperationsSettings =
                (BeforeLoadOperationsSettings)SettingsStack.GetElement(typeof(BeforeLoadOperationsSettings));

            if (beforeLoadOperationsSettings != null)
            {
                await beforeLoadOperationsSettings.DoOperations();
            }

            await ActiveScene.LoadActiveSceneIfNecessary(SettingsStack);

            foreach (SceneType sceneComponent in SceneTypeComponentStack.ElementList)
            {
                await sceneComponent.LoadInternal();
            }

            await UniTask.WaitWhile(() => LoadingProgress != 1);

            var afterLoadOperationsSettings =
                (AfterLoadOperationsSettings)SettingsStack.GetElement(typeof(AfterLoadOperationsSettings));

            if (afterLoadOperationsSettings != null)
            {
                await afterLoadOperationsSettings.DoOperations();
            }

            await ProgressBar.UnloadProgressBarIfNecessary(SettingsStack);

            if (pastSceneCollection != null)
            {
                await FadeTransition.UnloadFadeIfNecessary(pastSceneCollection.SettingsStack);
            }
        }

        public async UniTask Unload(SceneCollection nextLoadSceneCollection)
        {
            Current = null;

            var beforeUnloadOperationsSettings =
                (BeforeUnloadOperationsSettings)SettingsStack.GetElement(typeof(BeforeUnloadOperationsSettings));

            if (beforeUnloadOperationsSettings != null)
            {
                await beforeUnloadOperationsSettings.DoOperations();
            }

            foreach (SceneType sceneComponent in SceneTypeComponentStack.ElementList)
            {
                await sceneComponent.UnloadInternal(nextLoadSceneCollection);
            }

            await ActiveScene.UnloadActiveSceneIfNecessary(SettingsStack);

            var afterUnloadOperationsSettings =
                (AfterUnloadOperationsSettings)SettingsStack.GetElement(typeof(AfterUnloadOperationsSettings));

            if (afterUnloadOperationsSettings != null)
            {
                await afterUnloadOperationsSettings.DoOperations();
            }
        }

        public bool HasScene(SceneReference sceneReference) => SceneTypeComponentStack.HasScene(sceneReference);

        public List<SceneReference> GetSceneReferences()
        {
            var sceneReferences = new List<SceneReference>();

            foreach (SceneType sceneComponent in SceneTypeComponentStack.ElementList)
            {
                foreach (SettingsComponent sceneManagerComponent in SettingsStack.ElementList)
                {
                    sceneReferences.AddRange(sceneManagerComponent.GetSceneReferences());
                }

                sceneReferences.AddRange(sceneComponent.GetSceneReferencesInternal());
            }

            return sceneReferences;
        }

        public bool IsLoaded() => Current == this;
    }
}
