#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.ComponentStack.Editor.Core
{
    public abstract class ComponentStackEditor<T, N>
        where T : Runtime_Core_Component
        where N : ElementEditor
    {
        protected List<N> Editors;

        protected ComponentStackEditor(ComponentStack<T> stack)
        {
            Stack = stack;
            Editors = new List<N>();
            RefreshEditors();
        }

        public ComponentStack<T> Stack { get; }

        public N SelectedEditor => Editors.FirstOrDefault(t => ((Runtime_Core_Component)t.Target).Selected);

        protected virtual void Create(T settings, int index = -1)
        {
            Type settingsType = settings.GetType();

            if (AllEditorTypes<T>.Types.TryGetValue(settingsType, out Type editorType))
            {
                if (editorType.GetAttribute(typeof(DontDrawAttribute)) != null)
                {
                    return;
                }

                CreateEditorInstance(settings, index, editorType);
            }
            else
            {
                if (!typeof(N).IsAbstract)
                {
                    CreateEditorInstance(settings, index, typeof(N));
                }
            }
        }

        private void CreateEditorInstance(T settings, int index, Type editorType)
        {
            var editor = (N)Activator.CreateInstance(editorType);

            try
            {
                editor.Init(settings);
            }
            catch
            {
                Debug.LogError("Component Editor initialization: " + settings.Name);
            }

            if (index < 0)
            {
                Editors.Add(editor);
            }
            else
            {
                Editors[index] = editor;
            }
        }

        public void RefreshEditors()
        {
            Editors = new List<N>();

            Stack.RemoveInvalidElements();

            foreach (T t in Stack.ElementList)
            {
                Create(t);
            }
        }
    }
}
#endif
