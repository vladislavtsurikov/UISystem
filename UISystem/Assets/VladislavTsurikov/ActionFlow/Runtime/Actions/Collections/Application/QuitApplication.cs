using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Application
{
    [Name("Application/Quit Application")]
    public class QuitApplication : Action
    {
        public override string Name => "Quit Application";

        protected override UniTask<bool> Run(CancellationToken token)
        {
            UnityEngine.Application.Quit();
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            return UniTask.FromResult(true);
        }
    }
}
