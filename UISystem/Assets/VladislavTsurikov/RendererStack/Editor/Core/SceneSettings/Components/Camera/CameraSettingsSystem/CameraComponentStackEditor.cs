#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraSettingsSystem;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.RendererStack.Editor.Core.SceneSettings.Camera.CameraSettingsSystem
{
    public class
        CameraComponentStackEditor : ReorderableListStackEditor<CameraComponent, ReorderableListComponentEditor>
    {
        public CameraComponentStackEditor(ComponentStackOnlyDifferentTypes<CameraComponent> list) : base(
            new GUIContent(""), list, false)
        {
        }

        protected override void OnReorderableListStackGUI(Rect rect)
        {
            foreach (ReorderableListComponentEditor editor in GetCurrentEditors())
            {
                editor.OnGUI(rect, 0);
            }
        }

        public List<ReorderableListComponentEditor> GetCurrentEditors()
        {
            Component renderer = RendererStackManager.Instance.RendererStack.SelectedElement;

            var addCameraComponentsAttribute =
                (AddCameraComponentsAttribute)renderer.GetType().GetAttribute(typeof(AddCameraComponentsAttribute));

            var editors = new List<ReorderableListComponentEditor>();

            if (addCameraComponentsAttribute == null)
            {
                return editors;
            }

            foreach (ReorderableListComponentEditor editor in Editors)
            {
                if (addCameraComponentsAttribute.Types.Contains(editor.Target.GetType()))
                {
                    editors.Add(editor);
                }
            }

            return editors;
        }

        public override float GetElementStackHeight()
        {
            float height = 0;

            Component renderer = RendererStackManager.Instance.RendererStack.SelectedElement;

            var addCameraComponentsAttribute =
                (AddCameraComponentsAttribute)renderer.GetType().GetAttribute(typeof(AddCameraComponentsAttribute));

            if (addCameraComponentsAttribute == null)
            {
                return height;
            }

            foreach (ReorderableListComponentEditor editor in Editors)
            {
                if (addCameraComponentsAttribute.Types.Contains(editor.Target.GetType()))
                {
                    height += editor.GetElementHeight(0);
                }
            }

            return height;
        }
    }
}
#endif
