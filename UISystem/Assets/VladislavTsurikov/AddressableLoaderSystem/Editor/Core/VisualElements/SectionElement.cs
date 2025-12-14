#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    public class SectionElement : VisualElement
    {
        public Label TitleLabel { get; }

        public SectionElement(string title)
        {
            style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 0.4f);
            style.paddingTop = 8;
            style.paddingBottom = 8;
            style.paddingLeft = 10;
            style.paddingRight = 10;
            style.marginBottom = 10;
            style.borderBottomWidth = 1;
            style.borderTopWidth = 1;
            style.borderLeftWidth = 1;
            style.borderRightWidth = 1;
            style.borderBottomColor = new Color(0.25f, 0.25f, 0.25f);
            style.borderTopColor = new Color(0.25f, 0.25f, 0.25f);
            style.borderLeftColor = new Color(0.25f, 0.25f, 0.25f);
            style.borderRightColor = new Color(0.25f, 0.25f, 0.25f);
            style.flexDirection = FlexDirection.Column;

            TitleLabel = new Label(title);
            TitleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            TitleLabel.style.fontSize = 14;
            TitleLabel.style.marginBottom = 6;
            Add(TitleLabel);
        }
    }
}
#endif
