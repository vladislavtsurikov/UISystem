using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Application
{
    [Name("Application/Cursor Texture")]
    public class CursorTexture : Action
    {
        private readonly CursorMode _mode = CursorMode.Auto;

        private readonly Vector2 _tip = Vector2.zero;

        [SerializeField]
        private Texture2D _texture;

        public override string Name => $"Set Cursor Texture to {_texture}";

        protected override UniTask<bool> Run(CancellationToken token)
        {
            Cursor.SetCursor(_texture, _tip, _mode);
            return UniTask.FromResult(true);
        }
    }
}
