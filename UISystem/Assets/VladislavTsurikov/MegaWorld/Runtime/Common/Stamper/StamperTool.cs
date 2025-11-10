using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.AutoRespawn;
using VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    public abstract class StamperTool : MonoBehaviourTool
    {
        public float SpawnProgress;

#if UNITY_EDITOR
        public AutoRespawnController AutoRespawnController = new();
#endif

        [field: NonSerialized]
        public bool DisplayProgressBar { get; private set; }

        public bool IsSpawning => SpawnCancellationTokenSource != null;

        public CancellationTokenSource SpawnCancellationTokenSource { get; protected set; }

        private protected override void OnToolEnable() => OnStamperEnable();

        public void CancelSpawn()
        {
            SpawnProgress = 0f;

            SpawnCancellationTokenSource?.Cancel();
            SpawnCancellationTokenSource = null;

#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
            OnCancelSpawn();
        }

        public void SpawnStamper(bool displayProgressBar = false)
        {
            SpawnCancellationTokenSource?.Cancel();
            SpawnCancellationTokenSource = new CancellationTokenSource();
            SpawnStamper(SpawnCancellationTokenSource.Token, displayProgressBar).Forget();
        }

        private async UniTask SpawnStamper(CancellationToken token, bool displayProgressBar = false)
        {
            SpawnProgress = 0;
            DisplayProgressBar = displayProgressBar;

            await Spawn(token, DisplayProgressBar);

            SpawnProgress = 1;
            SpawnCancellationTokenSource = null;

#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
        }

#if UNITY_EDITOR
        public void UpdateDisplayProgressBar(string title, string info)
        {
            if (DisplayProgressBar)
            {
                EditorUtility.DisplayProgressBar(title, info, SpawnProgress);
            }
        }
#endif

        public virtual void OnCancelSpawn()
        {
        }

        private protected virtual void OnStamperEnable()
        {
        }

        protected virtual async UniTask Spawn(CancellationToken token, bool displayProgressBar)
        {
            await UniTask.CompletedTask;
        }
    }
}
