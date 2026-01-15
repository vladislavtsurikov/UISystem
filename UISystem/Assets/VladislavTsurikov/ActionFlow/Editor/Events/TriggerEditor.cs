#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Actions;
using VladislavTsurikov.ActionFlow.Runtime.Events;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.SingleElementStackEditor;
using Event = VladislavTsurikov.ActionFlow.Runtime.Events.Event;

namespace VladislavTsurikov.ActionFlow.Editor.Events
{
    [CustomEditor(typeof(Trigger))]
    public class TriggerEditor : UnityEditor.Editor
    {
        private ReorderableListStackEditor<Action, ReorderableListComponentEditor> _actionsCollectionEditor;
        private SingleElementStackEditor<Event, IMGUIElementEditor> _singleElementStackEditor;
        private Trigger Trigger => (Trigger)target;

        private void OnEnable()
        {
            _actionsCollectionEditor = new ReorderableListStackEditor<Action, ReorderableListComponentEditor>(
                new GUIContent("Actions"), Trigger.ActionCollection, true);

            _singleElementStackEditor =
                new SingleElementStackEditor<Event, IMGUIElementEditor>(Trigger.SingleElementStack);
        }

        public override void OnInspectorGUI()
        {
            _singleElementStackEditor.OnGUI();

            GUILayout.Space(3);

            _actionsCollectionEditor.OnGUI();
        }
    }
}
#endif
