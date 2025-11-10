using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.UIBackSystem.Runtime
{
    public static class UIBackStack
    {
        private static readonly Stack<UIHandler> _stack = new();

        static UIBackStack()
        {
            UIHandler.OnUIHandlerOnShow += Push;
            UIHandler.OnUIHandlerHide += _ => Peek();
            UIHandler.OnUIHandlerDestroyed += unit => _stack.Remove(unit);
        }

        private static void Push(UIHandler handler)
        {
            if (!_stack.Contains(handler))
            {
                _stack.Push(handler);
            }
        }

        internal static UIHandler Peek() => _stack.Count > 0 ? _stack.Peek() : null;

        internal static async UniTask PopAndHide(CancellationToken cancellationToken)
        {
            if (_stack.Count > 0)
            {
                UIHandler top = _stack.Pop();
                await top.Hide(cancellationToken);
            }
        }
    }
}
