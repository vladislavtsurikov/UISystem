#if UNITY_EDITOR
using System;
using OdinSerializer;
using VladislavTsurikov.SceneManagerTool.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Editor
{
    [Serializable]
    internal class SceneManagerEditorData
    {
        public SceneSetupManager SceneSetupManager = new();

        public StartupScene StartupScene = new();

        [OdinSerialize]
        private bool _runAsRunAsBuildMode;

        public bool RunAsBuildMode
        {
            get => _runAsRunAsBuildMode;
            set
            {
                _runAsRunAsBuildMode = value;
                SceneManagerData.MaskAsDirty();
            }
        }

        public void Setup()
        {
            StartupScene.Setup();
            SceneSetupManager.Setup();
        }
    }
}
#endif
