#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneUtility.Editor.Integration.CustomInspector
{
    public class SceneReferenceFieldDrawer : IMGUIFieldDrawer
    {
        public override bool CanDraw(Type type)
        {
            if (type == typeof(SceneReference))
            {
                return true;
            }

            return false;
        }

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value)
        {
            if (value is not SceneReference sceneReference)
            {
                return value;
            }

            sceneReference.SceneAsset = (SceneAsset)EditorGUI.ObjectField(
                rect,
                label,
                sceneReference.SceneAsset,
                typeof(SceneAsset),
                false);

            return sceneReference;
        }
    }
}
#endif
