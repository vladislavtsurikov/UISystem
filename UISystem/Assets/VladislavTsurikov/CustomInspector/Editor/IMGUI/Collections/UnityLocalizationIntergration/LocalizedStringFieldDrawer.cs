#if UNITY_EDITOR
#if UNITY_LOCALIZATION
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public class LocalizedStringFieldDrawer : IMGUIFieldDrawer
    {
        public override bool CanDraw(Type type) => type == typeof(LocalizedString);

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value)
        {
            if (value == null)
            {
                return null;
            }

            var labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            var buttonRect = new Rect(rect.x + EditorGUIUtility.labelWidth + 5, rect.y,
                rect.width - EditorGUIUtility.labelWidth - 5, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(labelRect, label);

            var localizedString = (LocalizedString)value;
            var buttonText = "Edit LocalizedString";

            if (GUI.Button(buttonRect, buttonText))
            {
                LocalizedStringEditorWindow.ShowWindow(localizedString);
            }

            return value;
        }
    }

    public class LocalizedStringEditorWindow : EditorWindow
    {
        private static LocalizedString _localizedString;
        private SerializedProperty _property;
        private Vector2 _scrollPosition;
        private SerializedObject _serializedObject;
        private LocalizeStringEvent _tempComponent;

        private void OnEnable()
        {
            if (_localizedString != null)
            {
                var tempObject = new GameObject("TempLocalizedStringHolder");
                _tempComponent = tempObject.AddComponent<LocalizeStringEvent>();
                _tempComponent.StringReference = _localizedString;

                _serializedObject = new SerializedObject(_tempComponent);
                _property = _serializedObject.FindProperty("StringReference");
            }
        }

        private void OnDisable()
        {
            if (_tempComponent != null)
            {
                DestroyImmediate(_tempComponent.gameObject);
            }
        }

        private void OnGUI()
        {
            if (_localizedString == null || _property == null)
            {
                Close();
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(position.width),
                GUILayout.Height(position.height));

            _serializedObject.Update();
            EditorGUILayout.PropertyField(_property, true);
            _serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Close"))
            {
                _localizedString = _tempComponent.StringReference;
                Close();
            }

            EditorGUILayout.EndScrollView();
        }

        public static void ShowWindow(LocalizedString localizedString)
        {
            _localizedString = localizedString;
            GetWindow<LocalizedStringEditorWindow>("Edit LocalizedString").Show();
        }
    }

    public class LocalizeStringEvent : MonoBehaviour
    {
        public LocalizedString StringReference;
    }
}
#endif
#endif
