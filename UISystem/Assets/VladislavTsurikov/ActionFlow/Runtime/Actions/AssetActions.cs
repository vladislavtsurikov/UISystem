using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions
{
    [CreateAssetMenu(fileName = "Actions", menuName = "ActionFlow/Actions")]
    public class AssetActions : SerializedScriptableObject
    {
        [OdinSerialize]
        public AssetActions CancellationActions;

        [OdinSerialize]
        public ActionCollection ActionCollection = new();

        public void Setup(bool force = true, object[] setupData = null, HashSet<AssetActions> visited = null)
        {
            visited ??= new HashSet<AssetActions>();

            if (visited.Contains(this))
            {
                Debug.LogError(
                    $"[{nameof(AssetActions)}] Detected recursion loop on {name}. Setup skipped to prevent StackOverflow.");
                return;
            }

            visited.Add(this);

            if (CancellationActions != null)
            {
                CancellationActions.Setup(force, setupData, visited);
            }

            ActionCollection.Setup(force, setupData);
        }

        public async UniTask Run(CancellationToken token)
        {
            var isActionsCompeted = await ActionCollection.Run(token);

            if (!isActionsCompeted)
            {
                if (CancellationActions == null)
                {
                    Debug.LogError($"[{nameof(AssetActions)}] CancellationActions are not set");
                    return;
                }

                await CancellationActions.Run(token);
            }
        }

#if UNITY_EDITOR
        public void SetDirtyAssetActions() => EditorUtility.SetDirty(this);
#endif
    }
}
