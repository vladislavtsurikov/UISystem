#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.UnityUtility.Runtime
{
    public static class GizmosEx
    {
        private static readonly Stack<Color> _colorStack = new();
        private static readonly Stack<Matrix4x4> _matrixStack = new();

        static GizmosEx()
        {
            _colorStack.Push(Color.white);
            _matrixStack.Push(Matrix4x4.identity);
        }

        public static void PushColor(Color color)
        {
            _colorStack.Push(color);
            Gizmos.color = _colorStack.Peek();
        }

        public static void PopColor()
        {
            if (_colorStack.Count > 1)
            {
                _colorStack.Pop();
            }

            Gizmos.color = _colorStack.Peek();
        }

        public static void PushMatrix(Matrix4x4 matrix)
        {
            _matrixStack.Push(matrix);
            Gizmos.matrix = _matrixStack.Peek();
        }

        public static void PopMatrix()
        {
            if (_matrixStack.Count > 1)
            {
                _matrixStack.Pop();
            }

            Gizmos.matrix = _matrixStack.Peek();
        }
    }
}
#endif
