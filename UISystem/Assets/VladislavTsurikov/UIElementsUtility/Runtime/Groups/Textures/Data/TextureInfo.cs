using System;
using UnityEngine;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.UIElementsUtility.Runtime.Groups.Textures
{
    [Serializable]
    public class TextureInfo
    {
        public string TextureName;
        public Texture2D TextureReference;

        public void ValidateName() => TextureName = TextureName.RemoveWhitespaces().RemoveAllSpecialCharacters();
    }
}
