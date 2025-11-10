#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.SceneManagerTool.Runtime;
using VladislavTsurikov.SceneManagerTool.Runtime.BuildSceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.UnityUtility.Editor;

namespace VladislavTsurikov.SceneManagerTool.Editor.SceneCollectionSystem
{
    [CustomPropertyDrawer(typeof(SceneCollectionReference))]
    public class SceneCollectionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            {
                SceneCollectionReferenceField(position, new GUIContent("SceneCollection"),
                    property.GetTarget<SceneCollectionReference>());
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUIUtility.singleLineHeight;

        public static void SceneCollectionReferenceField(Rect totalRect, GUIContent text,
            SceneCollectionReference sceneCollectionReference)
        {
            Rect rectField = CustomEditorGUI.PrefixLabel(totalRect, text);

            var clickButtonText = sceneCollectionReference.IsValid()
                ? sceneCollectionReference.SceneCollection.Name
                : "Click to set value";

            if (CustomEditorGUI.ClickButton(rectField, clickButtonText))
            {
                ShowClickManu(sceneCollectionReference);
            }
        }

        private static void ShowClickManu(SceneCollectionReference sceneCollectionReference)
        {
            SceneManagerEditorUtility.SetAllScenesToDirty();

            var menu = new GenericMenu();

            foreach (BuildSceneCollection buildSceneCollection in
                     SceneManagerData.Instance.Profile.BuildSceneCollectionStack.ElementList)
            foreach (SceneCollection sceneCollection in buildSceneCollection.GetAllSceneCollections())
            {
                if (sceneCollectionReference.SceneCollection != sceneCollection)
                {
                    menu.AddItem(new GUIContent(buildSceneCollection.Name + "/" + sceneCollection.Name), false,
                        () => sceneCollectionReference.SceneCollection = sceneCollection);
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(buildSceneCollection.Name + "/" + sceneCollection.Name));
                }
            }

            menu.ShowAsContext();
        }
    }
}
#endif
