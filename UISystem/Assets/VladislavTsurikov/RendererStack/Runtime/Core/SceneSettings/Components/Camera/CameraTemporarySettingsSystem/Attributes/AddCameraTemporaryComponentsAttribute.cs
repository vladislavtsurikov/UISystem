using System;

namespace VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraTemporarySettingsSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AddCameraTemporaryComponentsAttribute : Attribute
    {
        public readonly Type[] Types;

        public AddCameraTemporaryComponentsAttribute(Type[] types) => Types = types;
    }
}
