using System;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    [Name("Mask Filter Settings")]
    public class MaskFilterComponentSettings : Component
    {
        [NonSerialized]
        public MaskFilterContext FilterContext;

        [NonSerialized]
        public Texture2D FilterMaskTexture2D;

        [OdinSerialize]
        public MaskFilterStack MaskFilterStack = new();
    }
}
