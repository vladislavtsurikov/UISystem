#if UNITY_EDITOR
using System;
using System.Linq;
using Assemblies.VladislavTsurikov.ComponentStack.Runtime.SingleElementStack;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.ReflectionUtility.Runtime;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.SingleElementStackEditor
{
    public class SingleElementStackEditor<T, N> : ComponentStackEditor<T, N>
        where T : Component
        where N : IMGUIElementEditor
    {
        private readonly SingleElementStack<T> _singleElementStack;
        private GUIStyle _darkBackgroundStyle;

        public SingleElementStackEditor(SingleElementStack<T> stack) : base(stack) => _singleElementStack = stack;

        public void OnGUI()
        {
            if (Stack.IsDirty)
            {
                Stack.RemoveInvalidElements();
                RefreshEditors();
                InitializeStyles();
                Stack.IsDirty = false;
            }

            Component component = _singleElementStack.GetElement();
            var clickButtonText = component == null ? "Select" : component.Name;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                if (CustomEditorGUILayout.ClickButton(clickButtonText))
                {
                    ShowAddMenu();
                }

                GUILayout.Space(3);
            }
            GUILayout.EndHorizontal();

            if (component != null)
            {
                IMGUIElementEditor editor = GetElement();

                editor.OnGUI();
            }
        }

        private void ShowAddMenu()
        {
            var menu = new GenericMenu();

            Component component = _singleElementStack.GetElement();

            foreach (Type settingsType in AllTypesDerivedFrom<T>.Types.ToList())
            {
                var context = settingsType.GetAttribute<NameAttribute>().Name;

                var exists = component != null && component.GetType() == settingsType;

                if (!exists)
                {
                    menu.AddItem(new GUIContent(context), false,
                        () => _singleElementStack.ReplaceElement(settingsType));
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(context));
                }
            }

            menu.ShowAsContext();
        }

        private void InitializeStyles()
        {
            if (_darkBackgroundStyle == null)
            {
                _darkBackgroundStyle = new GUIStyle(EditorStyles.helpBox);
                _darkBackgroundStyle.normal.background = MakeBackgroundTexture(1, 1, new Color(0.1f, 0.1f, 0.1f));
                _darkBackgroundStyle.padding = new RectOffset(10, 10, 10, 10);
            }
        }

        private IMGUIElementEditor GetElement() => Editors.Count > 0 ? Editors[0] : null;

        private Texture2D MakeBackgroundTexture(int width, int height, Color color)
        {
            var pix = new Color[width * height];
            for (var i = 0; i < pix.Length; i++)
            {
                pix[i] = color;
            }

            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}
#endif
