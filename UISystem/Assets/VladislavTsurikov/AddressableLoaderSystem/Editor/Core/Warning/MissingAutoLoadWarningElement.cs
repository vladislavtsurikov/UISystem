#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.Utility.Editor;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Warning
{
    public class MissingAutoLoadWarningElement : VisualElement
    {
        public MissingAutoLoadWarningElement(List<string> invalidResourceLoaders)
        {
            if (invalidResourceLoaders == null || invalidResourceLoaders.Count == 0)
            {
                Debug.LogWarning("[AddressableLoaderSystem][MissingAutoLoadWarningElement..ctor] Invalid resource loader list is null or empty.");
                return;
            }

            style.backgroundColor = new Color(0.35f, 0.27f, 0.05f, 0.8f);
            style.paddingTop = 8;
            style.paddingBottom = 8;
            style.paddingLeft = 10;
            style.paddingRight = 10;
            style.marginBottom = 6;

            var title = new Label("Warning: ResourceLoaders with fields missing [AutoLoadAttribute] detected");
            title.style.color = new Color(1f, 0.9f, 0.3f);
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.marginBottom = 4;
            Add(title);

            var hint = new Label(
                "Add the [AutoLoad] attribute to fields so the system can validate that Addressable asset references are set correctly."
            );
            hint.style.whiteSpace = WhiteSpace.Normal;
            hint.style.color = new Color(1f, 0.9f, 0.3f);
            hint.style.marginBottom = 6;
            Add(hint);

            foreach (var loaderName in invalidResourceLoaders)
            {
                var line = new VisualElement
                {
                    style =
                    {
                        flexDirection = FlexDirection.Row,
                        marginBottom = 2
                    }
                };

                var label = new Label(loaderName)
                {
                    style =
                    {
                        color = Color.white,
                        minWidth = 240
                    }
                };
                line.Add(label);

                var scriptPath = CsFileUtility.FindPath(loaderName);
                if (!string.IsNullOrEmpty(scriptPath))
                {
                    var openBtn = new Button(() =>
                    {
                        Object asset = AssetDatabase.LoadAssetAtPath<Object>(scriptPath);
                        if (asset != null)
                        {
                            AssetDatabase.OpenAsset(asset);
                        }
                    })
                    {
                        text = "Open"
                    };

                    openBtn.style.marginLeft = 6;
                    line.Add(openBtn);
                }

                Add(line);
            }
        }
    }
}
#endif
