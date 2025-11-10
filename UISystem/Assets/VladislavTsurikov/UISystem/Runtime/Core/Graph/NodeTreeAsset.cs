using OdinSerializer;
using UnityEditor;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;

namespace VladislavTsurikov.UISystem.Runtime.Core.Graph
{
    [LocationAsset("UI/Generated/NodeTree")]
    public class NodeTreeAsset : SerializedScriptableObjectSingleton<NodeTreeAsset>
    {
        [OdinSerialize]
        private NodeTree _tree = new();

        public NodeTree Tree => _tree;

#if UNITY_EDITOR
        public void SetTree(NodeTree newTree)
        {
            _tree = newTree;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
