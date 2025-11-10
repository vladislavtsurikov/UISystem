#if UNITY_EDITOR
using System;
using UnityEngine;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Window
{
    public class SpacingMouseMove : MouseMove
    {
        private float _dragDistance;
        public float Spacing;

        protected override void OnPointDetectionFromMouseDrag(Func<Vector3, bool> func)
        {
            if (Spacing == 0)
            {
                Debug.LogError("Spacing cannot be 0");
            }

            Vector3 moveVector = Raycast.Point - _prevRaycast.Point;
            Vector3 moveDirection = moveVector.normalized;
            var moveLenght = moveVector.magnitude;

            if (_dragDistance + moveLenght >= Spacing)
            {
                var d = Spacing - _dragDistance;
                Vector3 dragPoint = _prevRaycast.Point + moveDirection * d;
                _dragDistance = 0;
                moveLenght -= d;

                func.Invoke(dragPoint);

                while (moveLenght >= Spacing)
                {
                    moveLenght -= Spacing;
                    dragPoint += moveDirection * Spacing;

                    func.Invoke(dragPoint);
                }
            }

            _dragDistance += moveLenght;
        }

        protected override void OnStartDrag() => _dragDistance = 0;
    }
}
#endif
