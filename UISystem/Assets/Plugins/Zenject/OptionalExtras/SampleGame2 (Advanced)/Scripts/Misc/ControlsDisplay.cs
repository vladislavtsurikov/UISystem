using UnityEngine;

#pragma warning disable 649

namespace Zenject.SpaceFighter
{
    public class ControlsDisplay : MonoBehaviour
    {
        [SerializeField]
        private float _leftPadding;

        [SerializeField]
        private float _topPadding;

        [SerializeField]
        private float _width;

        [SerializeField]
        private float _height;

        public void OnGUI()
        {
            var bounds = new Rect(_leftPadding, _topPadding, _width, _height);
            GUI.Label(bounds, "CONTROLS:  WASD to move, Mouse to aim, Left Mouse to fire");
        }
    }
}
