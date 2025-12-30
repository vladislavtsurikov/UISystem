#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneUtility.Editor.Integration.CustomInspector
{
    public sealed class SceneReferenceFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(SceneReference);
        public override Type DrawerType => typeof(SceneReferenceFieldDrawer);
    }

    public class SceneReferenceFieldDrawer : IMGUIFieldDrawer
    {
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
