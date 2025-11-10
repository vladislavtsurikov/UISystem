#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.SceneManagerTool.Editor.SceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.BuildSceneCollectionSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.BuildSceneCollectionSystem
{
    [ElementEditor(typeof(DefaultBuildSceneCollection))]
    public class DefaultBuildSceneCollectionEditor : IMGUIElementEditor
    {
        protected DefaultBuildSceneCollection _buildSceneCollection;
        protected SceneCollectionStackEditor _sceneCollectionStackEditor;

        public override void OnEnable()
        {
            _buildSceneCollection = (DefaultBuildSceneCollection)Target;
            _sceneCollectionStackEditor = new SceneCollectionStackEditor(new GUIContent("Scene Collections"),
                _buildSceneCollection.SceneCollectionStack);
        }

        public override void OnGUI() => _sceneCollectionStackEditor.OnGUI();
    }
}
#endif
