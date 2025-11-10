using System;
using OdinSerializer;
using UnityEngine;

namespace VladislavTsurikov.MegaWorld.Runtime.Common
{
    [Serializable]
    public class MouseSensitivitySettings
    {
        [OdinSerialize]
        private float _mouseSensitivity = 0.5f;

        public static float MinMouseSensitivity => 0.01f;
        public static float MaxMouseSensitivity => 1.0f;

        public float MouseSensitivity
        {
            get => _mouseSensitivity;
            set => _mouseSensitivity = Mathf.Clamp(value, MinMouseSensitivity, MaxMouseSensitivity);
        }
    }
}
